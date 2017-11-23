using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class ContentController : MonoBehaviour
{
    public MainScreenController mainScreenController;
    public ContentImageController contentImageController;
    public TMP_Text title;
    public TMP_Text text;

    private List<Themes> activThemes;
    private List<GameObject> activThemesItems = new List<GameObject>();

    public GameObject themeView;
    public GameObject themesListView;

    public GameObject themePrefab;
    public Transform themesCont;

    public TextMeshProUGUI m_TextMeshPro;
    public Canvas m_Canvas;
    private Camera m_Camera;

    private List<Photos> photos;

    public ScrollRect textScrollRect;


    // Use this for initialization
    void Start()
    {

        if (m_Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            m_Camera = null;
        else
            m_Camera = m_Canvas.worldCamera;

        themeView.SetActive(false);
        themesListView.SetActive(false);
    }


    public void ClearContent()
    {
        text.text = "";
        themeView.SetActive(false);
        themesListView.SetActive(false);
    }





    public void SetThemeData(List<Themes> theme)
    {
        activThemes = theme;

        if (activThemes.Count == 1)
        {
            CreateTheme(0);
           
        }
        else
        {
            createThemeList();
        }
    }


    public void createThemeList()
    {
        themeView.SetActive(false);
        themesListView.SetActive(true);

        int childs = themesCont.transform.childCount;

        for (int i = childs - 1; i > -1; i--)
        {
            GameObject.Destroy(themesCont.transform.GetChild(i).gameObject);
        }

        activThemesItems.Clear();

        for (int i = 0; i < activThemes.Count; i++)
        {
            GameObject gridItem = Instantiate(themePrefab) as GameObject;
            gridItem.transform.SetParent(themesCont);
            gridItem.transform.localScale = Vector3.one;

            gridItem.GetComponent<ItemData>().id = activThemes[i].Id;
            gridItem.GetComponent<ItemData>().index = i;

            TMP_Text text = gridItem.transform.Find("Title").GetComponent<TMP_Text>();
            text.text = activThemes[i].Name;

            TMP_Text label = gridItem.transform.Find("Label").GetComponent<TMP_Text>();
            label.text = "";
            //label.text = activThemes[i].Label;

            Button button = gridItem.GetComponent<Button>();

            UnityEngine.Events.UnityAction action = () => { this.CreateTheme(gridItem.GetComponent<ItemData>().index); };
            button.onClick.AddListener(action);

            activThemesItems.Add(gridItem);
        }
    }


    public void CreateTheme(int index)
    {
        themeView.SetActive(true);
        themesListView.SetActive(false);

        title.text = activThemes[index].Name;

        Analytics.CustomEvent("Pokrenuta Tema", new Dictionary<string, object>
        {
            { "Tema", activThemes[index].Label + " : " + activThemes[index].Name }
        });


        if (activThemes[index].Media != null)
        {
            if (activThemes[index].Media.Any<Media>(x => x.MediaTypeId == 3))
            {
                text.text = activThemes[index].Media.First<Media>(x => x.MediaTypeId == 3).Text;
            }


            if (activThemes[index].Media.Any<Media>(x => x.MediaTypeId == 4))
            {
                photos = activThemes[index].Media.First<Media>(x => x.MediaTypeId == 4).Photos.ToList<Photos>();

                //Debug.Log("CreateTheme  photos.Count: " + photos.Count);

                contentImageController.ImageSetup(photos);
            }
            else
            {
                contentImageController.HideImageHolders();
            }
        }
        else
        {
            contentImageController.HideImageHolders();
        }


        textScrollRect.verticalNormalizedPosition = 1;

    }


    public void onLinkClick()
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(m_TextMeshPro, Input.mousePosition, m_Camera);

        Debug.Log("linkIndex: " + linkIndex);

        if (linkIndex != -1)
        {
            
            TMP_LinkInfo linkInfo = m_TextMeshPro.textInfo.linkInfo[linkIndex];
            int linkHashCode = linkInfo.hashCode;

            Debug.Log("onLinkClick id: " + linkInfo.GetLinkID());

            mainScreenController.Findtheme(int.Parse(linkInfo.GetLinkID()));
            
        }
    }


    // Update is called once per frame
    void Update()
    {

    }
}
