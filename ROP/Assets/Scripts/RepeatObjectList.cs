using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatObjectList : MonoBehaviour
{
    //[HideInInspector]
    public bool differentSizes;
    public GameObject[] objectsToRepeat = new GameObject[] {};
 
    //public getter method
    public GameObject[] GetList()
    {
        return objectsToRepeat;
    }
}
