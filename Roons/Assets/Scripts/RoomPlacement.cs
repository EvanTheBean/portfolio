using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPlacement : MonoBehaviour
{
    public int roomCount;
    public GameObject room;
    public GameObject door;

    public List<GameObject> openRooms = new List<GameObject>();
    public List<GameObject> rooms = new List<GameObject>();

    public float width, height;
    // Start is called before the first frame update
    void Start()
    {
        
        GameObject start = GameObject.Find("Start");
        rooms.Add(start);
        openRooms.Add(start);
        width = start.GetComponent<SpriteRenderer>().bounds.size.x;
        height = start.GetComponent<SpriteRenderer>().bounds.size.y;

        for(int i =0; i<4; i++)
        {
            start.GetComponent<RoomControl>().doors.Add(i);
            SpawnDoors(i, start.GetComponent<RoomControl>());
            openRooms.Remove(start);
        }
        
        while (rooms.Count < roomCount && openRooms.Count > 0)
        {
            RoomControl currentRoom = openRooms[0].GetComponent<RoomControl>();
            int fiftyFifty = Random.Range(0, 2);
            int num = Random.Range(fiftyFifty, 4);

            for(int i =0; i < num; i++)
            {
                int place = Random.Range(0, 4);
                while(currentRoom.doors.Contains(place))
                {
                    place = Random.Range(0, 4);
                }
                SpawnDoors(place, currentRoom);
                currentRoom.doors.Add(place);
            }

            openRooms.Remove(currentRoom.gameObject);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnDoors(int dir, RoomControl currentRoom)
    {
        Vector3 placement = Vector3.zero;
        int newDir = dir;
        switch (dir)
        {
            case 1:
                placement = new Vector3(1, 0, 0);
                newDir = 3;
                //currentDoor.GetComponent<DoorControll>().doorSide = 3;
                break;
            case 3:
                placement = new Vector3(-1, 0, 0);
                newDir = 1;
                //currentDoor.GetComponent<DoorControll>().doorSide = 1;
                break;
            case 0:
                placement = new Vector3(0, 1, 0);
                newDir = 2;
                //currentDoor.GetComponent<DoorControll>().doorSide = 2;
                break;
            case 2:
                placement = new Vector3(0, -1, 0);
                newDir = 0;
                //currentDoor.GetComponent<DoorControll>().doorSide = 0;
                break;
            default:
                break;
        }

        GameObject newDoor = GameObject.Instantiate(door, currentRoom.transform.position + new Vector3(width * placement.x * .5f, height * placement.y * .5f, placement.z), Quaternion.identity);

        RaycastHit2D raycast = Physics2D.CircleCast(currentRoom.transform.position + new Vector3(width * placement.x, height * placement.y, placement.z), 0.5f, Vector3.zero);
        if(!raycast || !raycast.collider.gameObject.GetComponent<RoomControl>())
        {
            GameObject newRoom = GameObject.Instantiate(room, currentRoom.transform.position + new Vector3(width * placement.x, height * placement.y, placement.z), Quaternion.identity);
            newRoom.GetComponent<RoomControl>().doors.Add(newDir);
            openRooms.Add(newRoom); rooms.Add(newRoom);
        }
        else
        {
            Debug.Log("Already a room there dumbo :)");
            raycast.collider.gameObject.GetComponent<RoomControl>().doors.Add(newDir);
        }
    }

    public void AddRooms(GameObject nearRoom)
    {
        if(openRooms.Contains(nearRoom))
        {
            RoomControl currentRoom = nearRoom.GetComponent<RoomControl>();
            int num = Random.Range(0, 4);

            for (int i = 0; i < num; i++)
            {
                int place = Random.Range(0, 4);
                while (currentRoom.doors.Contains(place))
                {
                    place = Random.Range(0, 4);
                }
                SpawnDoors(place, currentRoom);
            }

            openRooms.Remove(currentRoom.gameObject);
        }
        else
        {
            Debug.LogError("Room already has attached rooms :)");
        }
    }
}
