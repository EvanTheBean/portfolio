using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorControll : MonoBehaviour
{
    public int doorSide;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("Hit it");
        /*
        GameObject.Find("Manager").GetComponent<RoomPlacement>().openDoors.Remove(this.gameObject);    
        GameObject.Find("Manager").GetComponent<RoomPlacement>().closedDoors.Add(this.gameObject);
        this.gameObject.name = "Closed cause i bumped";
        */
    }
}
