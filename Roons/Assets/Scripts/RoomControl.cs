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
}
