using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomControl : MonoBehaviour
{
    public List<GameObject> doorCovers = new List<GameObject>();
    public List<int> doors = new List<int>();

    public void DestroyWalls()
    {
        foreach (int num in doors)
        {
            if(doorCovers.Count >= num)
            {
                GameObject temp =  doorCovers[num];
                doorCovers[num] = null;
                Destroy(temp);
            }
        }
    }

    public void DisplayOnMiniMap()
    {
        gameObject.layer = 6;
        foreach (Transform child in transform)
        {
            child.gameObject.layer = 6;
        }
    }

    public void PopulateRoom()
    {
        Object[] furnitureSets;
        bool accross = false;
        switch(doors.Count)
        {
            case 2:
                if(Mathf.Abs(doors[0] - doors[1]) == 2)
                {
                    furnitureSets = Resources.LoadAll("2DoorAccross");
                    accross = true;
                }
                else
                {
                    furnitureSets = Resources.LoadAll("2Door");
                    accross = false;
                }
                break;
            default:
                furnitureSets = Resources.LoadAll(doors.Count.ToString() + "Door");
                break;
        }

        GameObject roomStuff = Instantiate(furnitureSets[0],transform.position - new Vector3 (0,0,0.5f),Quaternion.identity) as GameObject;

        switch(doors.Count)
        {
            case 1:
                roomStuff.transform.Rotate(new Vector3(0,0,90 * (2-doors[0])));
                break;
            case 3:
                int type = doors[0] + doors[1] + doors[2];
                switch(type)
                {
                    case 3:
                        roomStuff.transform.Rotate(new Vector3(0,0,180));
                        break;
                    case 4:
                        roomStuff.transform.Rotate(new Vector3(0,0,-90));
                        break;
                    case 6:
                        roomStuff.transform.Rotate(new Vector3(0,0,90));
                        break;
                    default: break;
                }
                break;
            case 2:
                if(accross)
                {
                    if(!doors.Contains(2))
                    {
                        roomStuff.transform.Rotate(new Vector3(0,0,90));
                    }
                }
                else
                {
                    int type2 = doors[0] + doors[1];
                    if(doors.Contains(0))
                    {
                        type2 ++;
                    }
                    switch(type2)
                    {
                        case 2:
                            roomStuff.transform.Rotate(new Vector3(0,0,180));
                            break;
                        case 3:
                            roomStuff.transform.Rotate(new Vector3(0,0,90));
                            break;
                        case 4:
                            roomStuff.transform.Rotate(new Vector3(0,0,-90));
                            break;
                        default:
                            break;
                    }
                }
                break;
            default: break;
        }
    }
}
