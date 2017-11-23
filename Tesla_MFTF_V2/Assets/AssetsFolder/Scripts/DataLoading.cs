using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using LargeFileDownloader;



#if !(UNITY_WSA_8_1 ||  UNITY_WP_8_1 || UNITY_WINRT_8_1) || UNITY_EDITOR
using System.Threading;
#endif
using System.IO.Compression;

using System.Runtime.InteropServices;

#if (UNITY_WSA_8_1 || UNITY_WP_8_1 || UNITY_WINRT_8_1) && !UNITY_EDITOR
 using File = UnityEngine.Windows.File;
#else
using File = System.IO.File;
#endif

#if NETFX_CORE
#if UNITY_WSA_10_0
    using System.Threading.Tasks;
    using static System.IO.Directory;
    using static System.IO.File;
    using static System.IO.FileStream;
#endif
#endif


public class DataLoading : MonoBehaviour
{
    public UINavigation navigation;
    public Canvas preloadScreen;
    public PreloadController preloadController;

    public List<Themes> themesList = new List<Themes>();
    public List<Themes> subThemesList = new List<Themes>();

    // Zip
    public string serverURL;
    public string guideZipId = "2";

    private bool isInternet = false;

    private bool downloadDone = true;
    private int downloadPercent = 0;

    private int zres = 0;
    private string zipFile;
    private string zipPath;
    private WWW www;
    private string log;
    private string ppath;

    private int[] progress = new int[1];
    private int[] progress2 = new int[1];

    private float t1, t;

    private DirectoryInfo dirInfo;
    private DirectoryInfo[] dirContentInfo;

    public Guide guide;
    public int activeContIndex = 0;
    public int activThemeId;
    public int activSubThemeId;

    public Themes theme;

    //Big file downloader
    private FileDownloader downloader = new FileDownloader();
    private DownloadEvent evt = new DownloadEvent();
    public bool downloading = false;
    public bool fileDownloaded = false;


    public Text updateCount;
    private float TimeT = 0.0f;
    /// <summary>
    /// 
    /// </summary>
    public MainScreenController mainScreenController;

    public Text debugText;


    void LateUpdate()
    {
        if (downloading)
        {
            if (fileDownloaded)
            {
                downloading = false;
                extractAndDelteZip();
            }
            preloadController.preloadAmount(evt.progress * 0.01f);
        }
    }


    //log for output of results
    void plog(string t)
    {
        log += t + "\n"; ;
        Debug.Log(log);
    }


    // Use this for initialization
    void Start()
    {
        preloadScreen.gameObject.SetActive(true);
        preloadScreen.enabled = true;

        ppath = Application.persistentDataPath;

        // add events listners
        FileDownloader.onComplete += OnDownloadComplete;
        FileDownloader.onProgress += OnProgress;

        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!Ovo vrati
        updateThemes();
        //StartCoroutine(LoadDataFiles());
    }


    private void updateThemes()
    {
        Debug.Log("updateThemes");

        preloadController.showPreload(true);
        TimeT = 5f;
        //updateCount.text = "<size=100>updating</size>";
        StartCoroutine(CheckInternet());
    }


    private void OnDownloadComplete(DownloadEvent e)
    {
        evt = e;
       
        if (evt.status == DownloadStatus.COMPLETED)
        {
            fileDownloaded = true;
        }
        else if (evt.status == DownloadStatus.CANCELLED)
        {
            downloading = false;
        }
        else if (evt.status == DownloadStatus.FAILED)
        {
            downloading = false;
            if (evt.error != null)
                Debug.Log("ERROR: " + evt.error);
        }
    }


    private void OnProgress(DownloadEvent e)
    {
        evt = e;

        if (evt.status == DownloadStatus.PROGRESS)
        {
            //Debug.Log("evt progress: " + e.progress + " : " + e.downloadedBytes + ":" + e.totalBytes);
        }
    }


    IEnumerator CheckInternet()
    {
        yield return new WaitForSeconds(1);

        WWW internet = new WWW("http://mftf.s11.novenaweb.info/admin/index.aspx");

        yield return internet;
        Debug.Log("<color=red>CheckInternet</color> internet : " + string.IsNullOrEmpty(internet.error));

        if (!string.IsNullOrEmpty(internet.error))
        {
            isInternet = false;
            StartCoroutine(LoadDataFiles());
            //StartCoroutine(DownloadBigZipFile());
        }
        else
        {
            isInternet = true;
            StartCoroutine(DownloadBigZipFile());
        }
    }


    // Zip file loading
    public void loadZipFile()
    {
        Debug.Log("loadZipFile: " 
            + Application.persistentDataPath + "/" + guideZipId + " : " 
            + Directory.Exists(Application.persistentDataPath + "/" + guideZipId));


        if (Directory.Exists(Application.persistentDataPath + "/" + guideZipId))
        {
            downloadDone = true;
            StartCoroutine(CheckInternet());
            //StartCoroutine(LoadDataFileFile());
        }
        else
        {
            downloadDone = false;
           StartCoroutine(CheckInternet());
        }
    }


    IEnumerator DownloadBigZipFile()
    {
        downloading = true;
        yield return null;

        downloader.Init();

        zipFile = guideZipId + ".zip";

        string pathToSave = ppath + "/" + zipFile;
        string fileToDownload = serverURL + "data.aspx?gId=" + guideZipId;

        downloader.Download(fileToDownload, pathToSave);

    }

    private void extractAndDelteZip()
    {
        Debug.Log("extractAndDelteZip");

        debugText.text = Application.platform.ToString();

        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            DoDecompression(ppath + "/" + zipFile, ppath + "/");
            StartCoroutine(LoadDataFiles());
        }

        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            DoDecompression_FileBuffer(ppath + "/" + zipFile, ppath + "/");
            StartCoroutine(LoadDataFiles());
        }

        if (Application.platform == RuntimePlatform.Android)
        {
            DoDecompression_FileBuffer(ppath + "/" + zipFile, ppath + "/");
            StartCoroutine(LoadDataFiles());
        }

    }



    void DoDecompression(string file, string folder)
    {
        Debug.Log("DoDecompression: " + folder + ": file: " + file);
        zres = lzip.decompress_File(file, folder, progress, null, progress2);
        plog("DoDecompression decompress: " + zres.ToString());

    }



    void DoDecompression_FileBuffer(string file, string folder)
    {
        Debug.Log("DoDecompression_FileBuffer: " + folder + ": file: " + file);
        var fileBuffer = File.ReadAllBytes(file);
        zres = lzip.decompress_File(null, folder, progress, fileBuffer, progress2);
        plog("DoDecompression_FileBuffer decompress: " + zres.ToString());


    }

    IEnumerator LoadDataFiles()
    {
        Debug.Log("LoadDataFiles");

        string path = Application.persistentDataPath + "/" + guideZipId + "/data.json";
        

        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            path = "file:///" + path;
        }

        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            path = "file://" + path;
        }

        if (Application.platform == RuntimePlatform.WSAPlayerX64)
        {
            path = "file://" + path;
        }

        if (Application.platform == RuntimePlatform.Android)
        {
            path = "file://" + path;
        }

        //Debug.Log("JSON file path: " + path);
        debugText.text += "\n" + path;
        WWW dataWWW = new WWW(path);
        // Debug.Log("loading path: " + path);

        yield return dataWWW;

        //Debug.Log("LoadDataFileFile dataWWW.error: " + dataWWW.error);
        if (string.IsNullOrEmpty(dataWWW.error))
        {
            string jsonData = dataWWW.text;
            jsonData = jsonData.Replace("%20", " ");
            jsonData = jsonData.Replace("~/files", "/files");
            //jsonData = jsonData.Replace("\"", "");
            jsonData = jsonData.Replace("&lt;", "<");
            jsonData = jsonData.Replace("&gt;", ">");


            guide = new Guide();
            guide = JsonUtility.FromJson<Guide>(jsonData);

            Debug.Log("jsonData: " + jsonData);
            navigation.activateFirstScreen();

            guide.TranslatedContents[0].TagCategories[1].Tags = guide.TranslatedContents[0].TagCategories[1].Tags.OrderBy(x => int.Parse(x.Title)).ToArray();
            /*
            for(int  i= 0; i < guide.TranslatedContents[0].TagCategories[1].Tags.Length; i++)
            {
                Debug.Log(guide.TranslatedContents[0].TagCategories[1].Tags[i].Title);
            }
            */
            mainScreenController.CreateMenus(guide.TranslatedContents[0].TagCategories);
            mainScreenController.SetThemes(guide.TranslatedContents[0].Themes);

            if(File.Exists(ppath + "/" + zipFile))
            {
                File.Delete(ppath + "/" + zipFile);
            }
            

            hidePreloadScreen();
        }
        else
        {
            Debug.Log("ERROR: " + dataWWW.error);
        }
    }



    private void hidePreloadScreen()
    {
        preloadController.showPreload(false);
    }



    // Update is called once per frame
    void Update()
    {

    }
}




[Serializable]
public class Guide
{
    public int Id;
    public string Name;
    public TranslatedContents[] TranslatedContents;
}


[Serializable]
public class TranslatedContents
{
    public int Id;
    public int LanguageId;
    public int Rank;
    public Themes[] Themes;
    public TagCategories[] TagCategories;
}


[Serializable]
public class Themes
{
    public int Id;
    public int index;
    public string Name;
    public int Rank;
    public float Longitude;
    public float Latitude;
    public float PositionX;
    public float PositionY;
    public string ImagePath;
    public string Label;
    public Tags[] Tags;
    public Themes[] SubThemes;
    public Media[] Media;
}


[Serializable]
public class SubThemes
{
    public int Id;
    public int index;
    public string Name;
    public int Rank;
    public float Longitude;
    public float Latitude;
    public float PositionX;
    public float PositionY;
    public string ImagePath;
    public string Label;
    public Tags[] Tags;
    public Media[] Media;
}


[Serializable]
public class Media
{
    public int Id;
    public string Name;
    public int Rank;
    public int MediaTypeId;
    public Photos[] Photos;
    public string Text;
    public string ContentPath;
}


[Serializable]
public class Photos
{
    public int Id;
    public int Rank;
    public string Name;
    public string Path;
}


[Serializable]
public class Tags
{
    public int Id;
    public string Title;
    public string TagCategoryId;
}


[Serializable]
public class TagCategories
{
    public int Id;
    public string Title;
    public Tags[] Tags;
}

