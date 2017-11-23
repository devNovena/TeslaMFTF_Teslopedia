using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;
using DG.Tweening;

public class ContentImageController : MonoBehaviour
{
    public RectTransform panelImageRT0;
    public RectTransform panelImageRT1;

    public RectTransform[] panelImageRT;

    public float startWidth;

    private List<Photos> photos = new List<Photos>();

    private bool imageScreenExpanded = false;

    public Image photoContainer0;
    public Image photoContainer1;

    public Image[] photoContainers;

    public CanvasGroup gallery_CavasGroup;

    private DataLoading dataLoading;

    private int panelWidth;

    public GameObject galleryPrefab;
    public GameObject galleryCont;

    private List<GameObject> galleryItems = new List<GameObject>();

    private int imageLoaded = 0;

    private int activSekvence = 0;

    public GameObject backButton;

    public GameObject arrow0;
    public GameObject arrow1;
    public GameObject[] imageArrow;

    //public List<>

    // Use this for initialization
    void Start()
    {
        backButton.SetActive(false);

        startWidth = panelImageRT[0].sizeDelta.x;

        dataLoading = GameObject.Find("DataLoading").GetComponent<DataLoading>();

        panelWidth = 2;

        arrow0.SetActive(false);
        arrow1.SetActive(false);

        showGallery(false);
    }


    private void showGallery(bool show)
    {
        gallery_CavasGroup.interactable = show;
        gallery_CavasGroup.blocksRaycasts = show;

        if (show)
        {
            gallery_CavasGroup.DOFade(1.0f, 1.0f);
            //gallery_CavasGroup.alpha = 1f;
        }
        else
        {

            gallery_CavasGroup.alpha = 0f;
        }
    }


    public void HideImageHolders()
    {
        photoContainer0.DOKill(false);
        photoContainer1.DOKill(false);

        photoContainer0.gameObject.GetComponent<RectTransform>().DOKill(false);
        photoContainer1.gameObject.GetComponent<RectTransform>().DOKill(false);

        photoContainer0.gameObject.GetComponent<RectTransform>().DOAnchorPosX(-3240f, 0.5f).SetEase(Ease.InOutSine);
        photoContainer1.gameObject.GetComponent<RectTransform>().DOAnchorPosX(3240f, 0.5f).SetEase(Ease.InOutSine);

        panelWidth = 1;
        panelImageRT[0].DOSizeDelta(new Vector2(startWidth, panelImageRT[0].sizeDelta.y), 0.3f).SetEase(Ease.OutSine);
        panelImageRT[1].DOSizeDelta(new Vector2(startWidth, panelImageRT[1].sizeDelta.y), 0.3f).SetEase(Ease.OutSine);
        activSekvence = 0;

        imageArrow[0].transform.DOScaleX(1f, 0f);
        imageArrow[1].transform.DOScaleX(1f, 0f);

        imageArrow[0].GetComponent<RectTransform>().DOLocalMoveX(-107f, 0.0f);
        imageArrow[1].GetComponent<RectTransform>().DOLocalMoveX(107f, 0.0f);

        arrow0.SetActive(false);
        arrow1.SetActive(false);
    }


    public void ImageSetup(List<Photos> photos_parm)
    {
        imageLoaded = 0;

        //photoContainer0.DOFade(0.0f, 0.0f);
        //photoContainer1.DOFade(0.0f, 0.0f);

        photos.Clear();
        photos = photos_parm;

        if (photos.Count == 0)
        {
            activSekvence = 0;
            arrow0.SetActive(false);
            arrow1.SetActive(false);

        }

        if (photos.Count == 1)
        {
            StartCoroutine(LoadSprite(photos[0].Path, photoContainers[0]));
            photoContainers[0].GetComponent<Image>().raycastTarget = true;
            photoContainers[1].GetComponent<Image>().raycastTarget = false;
            activSekvence = 0;
            arrow0.SetActive(true);
            arrow1.SetActive(false);

        }

        if (photos.Count == 2)
        {
            StartCoroutine(LoadSprite(photos[0].Path, photoContainers[0]));
            StartCoroutine(LoadSprite(photos[1].Path, photoContainers[1]));

            arrow0.SetActive(true);
            arrow1.SetActive(true);

            photoContainers[0].GetComponent<Image>().raycastTarget = true;
            photoContainers[1].GetComponent<Image>().raycastTarget = true;
            
            activSekvence = 0;
        }

        if (photos.Count > 2)
        {
            createGallery();
            activSekvence = 0;

        }
    }


    public void ExpandImageScreen(int index)
    {
        Debug.Log("ExpandImageScreen panelWidth: " + panelWidth + ": imageScreenExpanded: " + imageScreenExpanded);


        if (imageScreenExpanded)
        {
            if (photos.Count > 1)
            {
                showGallery(false);
            }

            imageArrow[index].transform.DOScaleX(-1f, 0f);

            panelImageRT[index].DOSizeDelta(new Vector2(startWidth, panelImageRT[index].sizeDelta.y), 1f).SetEase(Ease.OutQuint);
            imageScreenExpanded = false;


        }
        else
        {
            imageArrow[index].transform.DOScaleX(-1f, 0f);
            imageScreenExpanded = true;
            panelImageRT[index].DOSizeDelta(new Vector2(startWidth * panelWidth, panelImageRT[index].sizeDelta.y), 1f).SetEase(Ease.OutQuint);
        }

       
    }


    public void closeGallery()
    {
        /*
        activSekvence = 0;
        panelWidth = 1;
        backButton.SetActive(false);
        panelImageRT0.DOSizeDelta(new Vector2(startWidth * panelWidth, panelImageRT0.sizeDelta.y), 1f).SetEase(Ease.OutQuint);
        showGallery(false);
        */
    }


    public void ShowImage(int index)
    {
        Debug.Log("ShowImage photos.Count: " + photos.Count);

        Debug.Log("ShowImage activSekvence: " + activSekvence);


        if (activSekvence == 0 && photos.Count < 3 && photos.Count > 0)
        {
            if (index == 0)
            {
                Analytics.CustomEvent("Kliknuta slika", new Dictionary<string, object>
                {
                    { "Slika", "Desna" }
                });
            }
            if (index == 1)
            {
                Analytics.CustomEvent("Kliknuta slika", new Dictionary<string, object>
                {
                    { "Slika", "Lijeva" }
                });
            }
        }
        
        

        if (activSekvence == 0 && photos.Count < 3 && photos.Count > 0)
        {
            panelImageRT[index].gameObject.transform.SetAsLastSibling();

            if (imageScreenExpanded)
            {
                panelWidth = 1;
                imageArrow[index].transform.DOScaleX(1f, 0f);
                imageScreenExpanded = false;
            }
            else
            {
                panelWidth = 3;
                imageArrow[index].transform.DOScaleX(-1f, 0f);
                imageScreenExpanded = true;
            }


            
            panelImageRT[index].DOSizeDelta(new Vector2(startWidth * panelWidth, panelImageRT[index].sizeDelta.y), 1f).SetEase(Ease.OutSine);
            activSekvence = 0;
        }

        if (activSekvence == 1 && photos.Count < 3 && photos.Count > 0)
        {
            panelWidth = 1;
            panelImageRT[index].DOSizeDelta(new Vector2(startWidth , panelImageRT[index].sizeDelta.y), 1f).SetEase(Ease.OutSine);
            activSekvence = 0;
        }

    }


    public void createGallery()
    {
        int childs = galleryCont.transform.childCount;

        for (int i = childs - 1; i > -1; i--)
        {
            GameObject.Destroy(galleryCont.transform.GetChild(i).gameObject);
        }

        galleryItems.Clear();

        for (int i = 0; i < photos.Count; i++)
        {
            GameObject gridItem = Instantiate(galleryPrefab) as GameObject;
            gridItem.transform.SetParent(galleryCont.transform);
            gridItem.transform.localScale = Vector3.one;

            gridItem.GetComponent<ItemData>().index = i;
            gridItem.GetComponent<ItemData>().id = photos[i].Id;

            Button button = gridItem.GetComponent<Button>();

            UnityEngine.Events.UnityAction action = () => { this.ShowThumbImage(gridItem.GetComponent<ItemData>().index); };
            button.onClick.AddListener(action);

            Image image = gridItem.GetComponent<Image>();

            StartCoroutine(LoadSprite(photos[i].Path, image));


            galleryItems.Add(gridItem);

        }

    }



    private void ShowThumbImage(int index)
    {
        activSekvence = 2;

        photoContainer0.sprite = galleryItems[index].GetComponent<Image>().sprite;
        photoContainer0.gameObject.GetComponent<RectTransform>().DOSizeDelta(new Vector2(3240f, 1920f), 0.0f);
        photoContainer0.DOFade(1.0f, 0.0f);

        showGallery(false);
        panelWidth = 3;

        //panelImageRectTransform.DOSizeDelta(new Vector2(startWidth * panelWidth, panelImageRectTransform.sizeDelta.y), 1f).SetEase(Ease.InOutQuint);

        Debug.Log("ShowThumbImage activSekvence: " + activSekvence);

    }


    public IEnumerator LoadSprite(string image, Image holder)
    {
        yield return new WaitForSeconds(0.5f);
        WWW localFile = null;
        Texture2D texture;
        Texture2D oldTexture = null;
        Sprite sprite;
        string path = "";

        holder.gameObject.GetComponent<RectTransform>().DOKill(false);

        path = Application.persistentDataPath + "/" + dataLoading.guideZipId + image;
        //Debug.Log("<color=yellow>GalleryController LoadSprite Image path:</color> " + path);

        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            path = "file:///" + path;
        }

        if (Application.platform == RuntimePlatform.Android)
        {
            path = "file://" + path;
        }

        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            path = "file://" + path;
        }

        path = path.Replace("~/files", "/files");

        Debug.Log("path: " + path);

        localFile = new WWW(path);
        yield return localFile;

        texture = localFile.texture;
        sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

        if(photos.Count < 3 )
        {
            photoContainers[imageLoaded].gameObject.GetComponent<RectTransform>().DOSizeDelta(new Vector2(3240f, 1920f), 0.0f);

            if(photos.Count == 1)
            {
                arrow0.GetComponent<RectTransform>().DOLocalMoveX(-20f, 1f).SetLoops(2, LoopType.Yoyo).SetDelay(1.5f);
            }
            if (photos.Count > 1)
            {
                arrow0.GetComponent<RectTransform>().DOLocalMoveX(-20f, 1f).SetLoops(2, LoopType.Yoyo).SetDelay(1.5f);
                arrow1.GetComponent<RectTransform>().DOLocalMoveX(20f, 1f).SetLoops(2, LoopType.Yoyo).SetDelay(1.5f);
            }


            //photoContainer0.gameObject.GetComponent<RectTransform>().DOSizeDelta(new Vector2(3240f, 1920f), 0.0f);
            //holder.DOFade(1.0f, 1.5f);
        }
        

        holder.sprite = sprite;

        Debug.Log(imageLoaded + ":" + photos.Count);

        if(imageLoaded == 1 && photos.Count == 2)
        {
            //if (holder.gameObject.name == "ThemeImage1")
            //{
                photoContainer1.gameObject.GetComponent<RectTransform>().gameObject.GetComponent<RectTransform>().DOAnchorPosX(3240f, 0f);
                photoContainer1.DOFade(1.0f, 0.5f);
                photoContainer1.gameObject.GetComponent<RectTransform>().DOAnchorPosX(0f, 2f).SetEase(Ease.InOutQuint); ;
            //}

            //if (holder.gameObject.name == "ThemeImage0")
            //{
                photoContainer0.gameObject.GetComponent<RectTransform>().DOAnchorPosX(-3240f, 0f);
                photoContainer0.DOFade(1.0f, 0.5f);
                photoContainer0.gameObject.GetComponent<RectTransform>().DOAnchorPosX(0f, 2f).SetEase(Ease.InOutQuint);
            //}
        }

        if (imageLoaded == 0 && photos.Count == 1)
        {
            //if (holder.gameObject.name == "ThemeImage0")
            //{
                photoContainer0.gameObject.GetComponent<RectTransform>().DOAnchorPosX(-3240f, 0f);
                photoContainer0.DOFade(1.0f, 0.5f);
                photoContainer0.gameObject.GetComponent<RectTransform>().DOAnchorPosX(0f, 2.5f).SetEase(Ease.InOutQuint);
            //}
        }

        imageLoaded++;


        yield return null;
        localFile.Dispose();
        if (oldTexture)
        {
            DestroyImmediate(oldTexture);
        }
        oldTexture = texture;
        yield return null;


        

        

    }


    // Update is called once per frame
    void Update()
    {

    }
}
