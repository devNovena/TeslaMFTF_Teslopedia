using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class ScrollDragingHandler : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IScrollHandler
{
    public string scrollName;

    public void OnBeginDrag(PointerEventData data)
    {
        //Debug.Log("OnBeginDrag: " + scrollName);
    }

    public void OnEndDrag(PointerEventData data)
    {
        //Debug.Log("OnEndDrag: " + scrollName);
    }


    public void OnScroll(PointerEventData eventData)
    {

        //Debug.Log("OnScroll: " + scrollName);
    }
}
