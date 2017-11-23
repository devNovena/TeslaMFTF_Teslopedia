using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PreloadController : MonoBehaviour
{
    public Image preloadBar;
    public TMP_Text percent;

    // Use this for initialization
    void Start()
    {
        gameObject.SetActive(true);
        showPreload(false);
    }


    public void showPreload(bool show)
    {
        preloadBar.fillAmount = 0f;
        percent.text = "0";
        gameObject.GetComponent<Canvas>().enabled = show;
    }

    public void preloadAmount(float progress)
    {
        //Debug.Log("preloadAmount progress: " + progress);
        preloadBar.fillAmount = progress;
        percent.text = Mathf.Round(progress * 100).ToString();
    }


    // Update is called once per frame
    void Update()
    {

    }
}
