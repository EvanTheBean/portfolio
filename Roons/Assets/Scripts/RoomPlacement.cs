using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPlacement : MonoBehaviour
{
    public int roomCount;
    public GameObject room;
    public GameObject door;

    public List<GameObject> openDoors = new List<GameObject>();
    public List<GameObject> closedDoors = new List<GameObject>();
    public List<GameObject> rooms = new List<GameObject>();

    float width, height;
    // Start is called before the first frame update
    void Start()
    {
        GameObject start = GameObject.Find("Start");
        rooms.Add(start);
        width = start.GetComponent<SpriteRenderer>().bounds.size.x;
        height = start.GetComponent<SpriteRenderer>().bounds.size.y;

        openDoors.Add(GameObject.Instantiate(door,new Vector3(width/2f, 0,-1),Quaternion.identity));
        openDoors[0].GetComponent<DoorControll>().doorSide = 1;
        openDoors.Add(GameObject.Instantiate(door,new Vector3(-width/2f, 0,-1),Quaternion.identity));
        openDoors[1].GetComponent<DoorControll>().doorSide = 3;
        openDoors.Add(GameObject.Instantiate(door,new Vector3(0, height/2f,-1),Quaternion.identity));
        openDoors[2].GetComponent<DoorControll>().doorSide = 0;
        openDoors.Add(GameObject.Instantiate(door,new Vector3(0, -height/2f,-1),Quaternion.identity));
        openDoors[3].GetComponent<DoorControll>().doorSide = 2;

        while(openDoors.Count > 0 && rooms.Count < roomCount)
        {
            GameObject currentDoor = openDoors[0];
            Vector3 placement = Vector3.zero;
            switch(currentDoor.GetComponent<DoorControll>().doorSide)
            {
                case 1:
                    placement = new Vector3(width, 0,0);
                    //currentDoor.GetComponent<DoorControll>().doorSide = 3;
                    break;
                case 3:
                    placement = new Vector3(-width, 0,0);
                    //currentDoor.GetComponent<DoorControll>().doorSide = 1;
                    break;
                case 0:
                    placement = new Vector3(0, height,0);
                    //currentDoor.GetComponent<DoorControll>().doorSide = 2;
                    break;
                case 2:
                    placement = new Vector3(0, -height,0);
                    //currentDoor.GetComponent<DoorControll>().doorSide = 0;
                    break;
                default:
                    break;
            }
            rooms.Add(GameObject.Instantiate(room,placement, Quaternion.identity));
            //Debug.Log()
            SpawnDoors(currentDoor.GetComponent<DoorControll>().doorSide, placement);
            closedDoors.Add(currentDoor);
            openDoors.Remove(currentDoor);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnDoors(int dir1, Vector3 roomPlace)
    {
        List<int> dirs = new List<int>(dir1);
        int num = Random.Range(0,4);
        //Debug.Log(num);
        Vector3 placement = Vector3.zero;
        for(int i =0; i<num; i++)
        {
            int side = Random.Range(0,4);
            while(dirs.Contains(side))
            {
                side = Random.Range(0,4);
            }
            switch(side)
            {
                case 1:
                    placement = new Vector3(width/2, 0,-1);
                    break;
                case 3:
                    placement = new Vector3(-width/2, 0,-1);
                    break;
                case 0:
                    placement = new Vector3(0, height/2,-1);
                    break;
                case 2:
                    placement = new Vector3(0, -height/2,-1);
                    break;
                default:
                    break;
            }
            GameObject newDoor = GameObject.Instantiate(door,roomPlace+placement,Quaternion.identity);
            openDoors.Add(newDoor);
            newDoor.GetComponent<DoorControll>().doorSide = side;
            dirs.Add(side);
        }
    }
}
