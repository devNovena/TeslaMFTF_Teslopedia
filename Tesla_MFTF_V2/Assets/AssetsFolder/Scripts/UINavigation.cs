using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class UINavigation : MonoBehaviour
{
    public GameObject[] screenObjects;
    private List<Canvas> screens = new List<Canvas>();

    public GameObject splashScreenObject;
    private Canvas splashScreen;

    public GameObject screenPreload;

    public int current = -1;

    public List<int> screenHistory;

    private bool firstTouch = true;
    private Touch touchFirst;
    private Touch touchZero;
    private float deltaMagnitudeDiff = 0f;
    private float prevTouchDeltaMag = 0f;
    private float touchDeltaMag = 0f;
    private float touchZeroPrevPos = 0;

    // Use this for initialization
    void Start()
    {
        

        screenHistory = new List<int>();

        splashScreenObject.SetActive(true);
        splashScreen = splashScreenObject.GetComponent<Canvas>();

        for (int i = 0; i < screenObjects.Length; i++)
        {
            screenObjects[i].SetActive(true);
            screens.Add(screenObjects[i].GetComponent<Canvas>());
            screens[i].enabled = false;
        }
    }


    public void activateFirstScreen()
    {
        splashScreen.enabled = false;
        current = 0;
        screens[current].enabled = true;
        screenHistory.Add(0);
    }


    public void goToScreen(int index)
    {
        screens[current].enabled = false;
        current = index;
        screens[current].enabled = true;
        screenHistory.Add(current);
    }


    public void goBack()
    {
        if (screenHistory.Count > 1)
        {
            screens[screenHistory[screenHistory.Count - 1]].enabled = false;
            screenHistory.RemoveAt(screenHistory.Count - 1);
            screens[screenHistory[screenHistory.Count - 1]].enabled = true;
            current = screenHistory[screenHistory.Count - 1];
        }
    }


    public void goHome()
    {
        screens[current].enabled = false;
        current = 0;
        screenHistory.Clear();
        screenHistory.Add(current);
        screens[current].enabled = true;
    }



    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        /*
        if (current == 4)
        {
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    Application.targetFrameRate = 60;
                    //GameObject.Find("DebugScreen").GetComponent<AppDebug.DebugApp>().showMessage("Application.targetFrameRate = 30");
                }

                if ((Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled))
                {
                    //GameObject.Find("DebugScreen").GetComponent<AppDebug.DebugApp>().showMessage("Application.targetFrameRate = 10");
                    Application.targetFrameRate = 30;
                }
            }

        }
        */

    }


}