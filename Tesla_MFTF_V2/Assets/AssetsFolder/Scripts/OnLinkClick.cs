using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnLinkClick : MonoBehaviour, IPointerClickHandler
{

    public ContentController contentController;


    // Use this for initialization
    void Start()
    {

    }


    public void OnPointerClick(PointerEventData eventData)
    {
        contentController.onLinkClick();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
