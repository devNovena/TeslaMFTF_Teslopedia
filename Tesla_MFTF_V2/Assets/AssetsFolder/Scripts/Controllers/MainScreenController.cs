using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainScreenController : MonoBehaviour
{
    public UINavigation navigation;
    public DataLoading dataLoading;

    private MenuController menuController;
    private ContentController contentController;
    private ContentImageController contentImageController;

    private Themes[] themes;
    private List<Themes> themesFound = new List<Themes>();

    private int themeIndex;

    private float closeTimer = 0f;
    private bool startTimer = false;

    private float resetTimer = 0f;
    private bool startResetTimer = false;

    private bool startDownloadTimer;
    private float downloadTimer = 0f;


    // Use this for initialization
    void Start()
    {
        menuController = gameObject.GetComponent<MenuController>();
        contentController = gameObject.GetComponent<ContentController>();
        contentImageController = gameObject.GetComponent<ContentImageController>();

        menuController.mainScreenController = this;
    }


    public void CreateMenus(TagCategories[] tagCategories)
    {
        menuController.CreateMenus(tagCategories);
    }

    public void SetThemes(Themes[] themesParam)
    {
        themes = themesParam;
    }

    public void Findtheme(int tagId)
    {
        int i;
        int j;
        int n;

        themesFound.Clear();
        

        for (i = 0; i < themes.Length; i++)
        {
            if(themes[i].Tags != null)
            {
                /*
                for (n = 0; n < themes[i].Tags.Length; n++)
                {
                    Debug.Log("Tags: " + themes[i].Tags[n].Id + ":" + tagId);
                }
                */

                if (themes[i].Tags.Any<Tags>(x => x.Id == tagId))
                {
                    Debug.Log("themes[i].Label.IndexOf('S'): " + themes[i].Label.IndexOf("S"));

                    if (themes[i].Label.IndexOf("S") == -1)
                    {
                        themesFound.Add(themes[i]);
                    }
                    
                }
            }

            if(themes[i].SubThemes != null)
            {
                for (j = 0; j < themes[i].SubThemes.Length; j++)
                {
                    if(themes[i].SubThemes[j].Tags != null)
                    {
                        //Debug.Log(i + " : " + themes[i].SubThemes[j].Tags.Length + " : " + j + " :: " + themes[i].SubThemes[j].Tags.Any<Tags>(x => x.Id == tagId));
                        if (themes[i].SubThemes[j].Tags.Any<Tags>(x => x.Id == tagId))
                        {
                            themesFound.Add(themes[i].SubThemes[j]);
                        }
                    }
                    
                }
            }
            


        }

        Debug.Log("themesFound.Count: " + themesFound.Count);
        contentController.SetThemeData(themesFound);
    }

    public void CloseApp()
    {
        closeTimer = 0f;
        startTimer = true;
    }


    public void CancelClose()
    {
        closeTimer = 0f;
        startTimer = false;
    }

    public void StartResetTimer(bool reset)
    {
        startResetTimer = reset;
    }



    public void DownloadApp()
    {
        downloadTimer = 0f;
        startDownloadTimer = true;
    }


    public void CancelDownload()
    {
        downloadTimer = 0f;
        startDownloadTimer = false;
    }


    public void NewAppDownload()
    {
        Application.OpenURL("http://wwww.novena.hr/vr/TESLA/Teslopedija.zip");
    }


    // Update is called once per frame
    void Update()
    {
        if(startTimer)
        {
            closeTimer += Time.deltaTime;

            Debug.Log("closeTimer: " + closeTimer);

            if (closeTimer > 5)
            {
                Application.Quit();
            }
        }

   
        if (startDownloadTimer)
        {
            downloadTimer += Time.deltaTime;

            Debug.Log("downloadTimer: " + downloadTimer);

            if (downloadTimer > 5)
            {
                startDownloadTimer = false;
                downloadTimer = 0;
                NewAppDownload();
            }
        }


        if (startResetTimer)
        {
            resetTimer += Time.deltaTime;

            //Debug.Log("resetTimer: " + resetTimer);

            if (resetTimer > (60 * 3))
            {
                contentController.ClearContent();
                menuController.ResetMenues();
                contentImageController.HideImageHolders();
                
                startResetTimer = false;
                resetTimer = 0f;
            }

        }


        if(Input.GetMouseButtonDown(0))
        {
            resetTimer = 0f;
        }
    }
}
