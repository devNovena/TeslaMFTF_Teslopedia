using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetResolution : MonoBehaviour
{
    void Awake()
    {
        Screen.SetResolution(3240, 1920, false);
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }


    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
