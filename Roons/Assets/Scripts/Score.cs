using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public Text roomCountTxt;
    int roomCount;
    // Start is called before the first frame update
    void Start()
    {
        roomCount = 1;
        roomCountTxt.text = roomCount.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addRoom()
    {
        roomCount ++;
        roomCountTxt.text = roomCount.ToString();
    }
}
