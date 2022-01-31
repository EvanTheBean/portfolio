using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomControl : MonoBehaviour
{
    public List<DoorControll> doors = new List<DoorControll>();
    GameObject manager;
    // Start is called before the first frame update
    void Start()
    {
        //manager = GameObject.Find("Manager");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        /*
        if(other.gameObject.GetComponent<DoorControll>())
        {
            doors.Add(other.gameObject.GetComponent<DoorControll>());
        }
        */
    }

    public void SpawnDoors()
    {
        List<int> currentDoors = new List<int>();
        /*
        foreach(DoorControll door in doors)
        {
            currentDoors.Add((door.doorSide + 2)%4);
        }
        int newDoorCount = Random.Range(0,4-currentDoors.Count);

        for (int i =0; i < newDoorCount; i++)
        {
            int newDoor = Random.Range(0,4);
            while(!currentDoors.Contains(newDoor))
            {

            }
        }
        */
    }
}
