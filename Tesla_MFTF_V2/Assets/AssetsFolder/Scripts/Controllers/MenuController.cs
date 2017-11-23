using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class MenuController : MonoBehaviour
{
    public Camera manCam;

    public MainScreenController mainScreenController;
    private List<GameObject> yearItems = new List<GameObject>();
    private List<GameObject> catItems = new List<GameObject>();
    private List<GameObject> numItems = new List<GameObject>();

    public Transform[] menuContent;

    public GameObject[] menuPrefabs;

    private TagCategories[] tagCategories;

    public ScrollRect godineScroll;
    public ScrollRect katScroll;

    private float godPos = 0;
    private float katPos = 0;
    private float moveByGod = 0;
    private float moveByKat = 0;

    // Use this for initialization
    void Start()
    {

    }


    public void ResetMenues()
    {
        for (int i = 0; i < yearItems.Count; i++)
        {
            yearItems[i].GetComponent<TMP_Text>().color = new Color(1f, 1f, 1f, 1f);
        }

        for (int i = 0; i < catItems.Count; i++)
        {
            catItems[i].GetComponent<TMP_Text>().color = new Color(1f, 1f, 1f, 1f);
        }
    }


    public void CreateMenus(TagCategories[] tagCat)
    {
        tagCategories = tagCat;

        Debug.Log("CreateMenus tagCategories.Length: " + tagCategories.Length);

        for(int i = 0; i < menuContent.Length; i++)
        {
            AddMenuItem(i);
        }

        Invoke("GetContentSize", 2f);
    }


    private void AddMenuItem(int index)
    {
        for (int i = 0; i < tagCategories[index].Tags.Length; i++)
        {
            GameObject gridItem = Instantiate(menuPrefabs[index]) as GameObject;
            gridItem.transform.SetParent(menuContent[index]);
            gridItem.transform.localScale = Vector3.one;

            gridItem.GetComponent<ItemData>().id = tagCategories[index].Tags[i].Id;
            gridItem.GetComponent<ItemData>().index = i;
            gridItem.GetComponent<ItemData>().menuIndex = index;
            gridItem.GetComponent<ItemData>().title = tagCategories[index].Tags[i].Title;

            TMP_Text text = gridItem.GetComponent<TMP_Text>();
            text.text = tagCategories[index].Tags[i].Title;

            //gameObject.GetComponent<AnalyticsTracker>()

            Button button = gridItem.GetComponent<Button>();


            UnityEngine.Events.UnityAction action = () => { this.OnGridItemClick(gridItem.GetComponent<ItemData>().id, gridItem.GetComponent<ItemData>().index, gridItem); };
            button.onClick.AddListener(action);

            
            //bool isFullyVisible = gridItem.GetComponent<RectTransform>().IsFullyVisibleFrom(manCam);

            //Debug.Log("isFullyVisible: " + isFullyVisible);

            switch (index)
            {
                case 0:
                    catItems.Add(gridItem);
                    break;
                case 1:
                    yearItems.Add(gridItem);
                    
                    break;
            }
        }
    }



    private void OnGridItemClick(int id, int index, GameObject gridItem)
    {
        //Debug.Log("onGridItemClick id: " + id + " : index: " + index);

        gameObject.GetComponent<ContentImageController>().HideImageHolders();

        mainScreenController.Findtheme(id);

        //Debug.Log("onGridItemClick menuIndex: " + gridItem.GetComponent<ItemData>().menuIndex);


        if (gridItem.GetComponent<ItemData>().menuIndex == 1)
        {
            Analytics.CustomEvent("Odabrana Godina", new Dictionary<string, object>
            {
                { "Godina", yearItems[index].GetComponent<ItemData>().title }
            });
        }

        if (gridItem.GetComponent<ItemData>().menuIndex == 0)
        {
            Analytics.CustomEvent("Odabrana Kategorija", new Dictionary<string, object>
            {
                { "Kategorija", catItems[index].GetComponent<ItemData>().title }
            });
        }
        


        for (int i = 0; i < yearItems.Count; i++)
        {
            yearItems[i].GetComponent<TMP_Text>().color = new Color(1f, 1f, 1f, 1f);
        }
             
        for (int i = 0; i < catItems.Count; i++)
        {
            catItems[i].GetComponent<TMP_Text>().color = new Color(1f, 1f, 1f, 1f);
        }
                

        TMP_Text text = gridItem.GetComponent<TMP_Text>();

        text.color = new Color(1f, 1f, 1f, 0.3f);

        mainScreenController.StartResetTimer(true);

    }


    private void GetContentSize()
    {
        
        float x0 = menuContent[0].gameObject.GetComponent<RectTransform>().sizeDelta.x + 100;
        float y0 = menuContent[0].parent.gameObject.GetComponent<RectTransform>().sizeDelta.x;
        katPos = y0 / (x0 - y0);
        //Debug.Log("katPos: " + katPos);


        float x1 = menuContent[1].gameObject.GetComponent<RectTransform>().sizeDelta.x + 150;
        float y1 = menuContent[1].parent.gameObject.GetComponent<RectTransform>().sizeDelta.x;
        godPos = y1 / (x1 - y1);
        //Debug.Log("godPos: " + godPos);
    }


    public void moveGodMenu(int dir)
    {
        //godPos += dir;
        moveByGod += (dir * godPos);
        
        if (moveByGod > 1f )
        {
            moveByGod = 1;
        }

        if (moveByGod < 0f)
        {
            moveByGod = 0;
        }
        
        godineScroll.DOHorizontalNormalizedPos(moveByGod, 0.5f).OnUpdate(()=> OnScrollChanged(1)); 
    }


    public void moveKatMenu(int dir)
    {
        moveByKat += (dir * katPos);

        if (moveByKat > 1f)
        {
            moveByKat = 1;
        }

        if (moveByKat < 0f)
        {
            moveByKat = 0;
        }

        katScroll.DOHorizontalNormalizedPos(moveByKat, 0.5f).OnUpdate(() => OnScrollChanged(0));
    }

   

    public void OnScrollFinished(int index)
    {
    }


    public void OnScrollChanged(int index)
    {
        StartCoroutine(CalcItemsPos(index));
    }
   

    IEnumerator CalcItemsPos(int index)
    {
        yield return new WaitForEndOfFrame();
        
        if(index == 1)
        {
            for(int i = 0; i < yearItems.Count; i++)
            {
                float itemPos = yearItems[i].GetComponent<RectTransform>().anchoredPosition.x + 
                                yearItems[i].transform.parent.gameObject.GetComponent<RectTransform>().anchoredPosition.x;

                if (itemPos > -100f && itemPos < 750f)
                {
                    yearItems[i].GetComponent<CanvasRenderer>().SetAlpha(1);
                    yearItems[i].GetComponent<CanvasRenderer>().cull = false;
                }
                else
                {
                    yearItems[i].GetComponent<CanvasRenderer>().SetAlpha(0);
                    yearItems[i].GetComponent<CanvasRenderer>().cull = true;
                }
            }
        
        }

        if (index == 0)
        {
            for (int i = 0; i < catItems.Count; i++)
            {
                float itemPos = catItems[i].GetComponent<RectTransform>().anchoredPosition.x + 
                                catItems[i].transform.parent.gameObject.GetComponent<RectTransform>().anchoredPosition.x;

                if (itemPos > -100f && itemPos < 750f)
                {
                    catItems[i].GetComponent<CanvasRenderer>().SetAlpha(1);
                    catItems[i].GetComponent<CanvasRenderer>().cull = false;
                }
                else
                {
                    catItems[i].GetComponent<CanvasRenderer>().SetAlpha(0);
                    catItems[i].GetComponent<CanvasRenderer>().cull = true;
                }
            }

        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
