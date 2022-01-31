using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    RoomPlacement rp;
    public float width, height;
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        rp = GameObject.Find("Manager").GetComponent<RoomPlacement>();
        player = GameObject.Find("Player");
    }


    // Update is called once per frame
    void Update()
    {
        if(player.transform.hasChanged)
        {
            width = rp.width; height = rp.height;
            Vector3 placePos = new Vector3(player.transform.position.x, player.transform.position.y, -10);
            Vector3 roundPlacePos = new Vector3(placePos.x % width, placePos.y % height, placePos.z);
            if (roundPlacePos.x > width / 2f) 
            {
                placePos.x += width - roundPlacePos.x;
            }
            else if (roundPlacePos.x < width / 2f && roundPlacePos.x > 0)
            {
                placePos.x -= roundPlacePos.x;
            }
            else if (roundPlacePos.x > -width / 2f && roundPlacePos.x < 0)
            {
                placePos.x -= roundPlacePos.x;
            }
            else if (roundPlacePos.x < -width / 2f)
            {
                placePos.x -= width + roundPlacePos.x;
            }

            if (roundPlacePos.y > height / 2f)
            {
                placePos.y += height - roundPlacePos.y;
            }
            else if (roundPlacePos.y < height / 2f && roundPlacePos.y > 0)
            {
                placePos.y -= roundPlacePos.y;
            }
            else if (roundPlacePos.y > -height / 2f && roundPlacePos.y < 0)
            {
                placePos.y -= roundPlacePos.y;
            }
            else if (roundPlacePos.y < -height / 2f)
            {
                placePos.y -= height + roundPlacePos.y;
            }

            transform.position = placePos;
            //Debug.Log(player.transform.position + " " + roundPlacePos + " " + placePos);
            int mask = 2;
            RaycastHit2D raycast = Physics2D.Raycast(transform.position - new Vector3(0,0,transform.position.z), 0.5f, Vector3.zero, mask);
            if(raycast)
            {
                Debug.Log("i hit a thing " + raycast.collider.gameObject.name);
                if(raycast.collider.gameObject.GetComponent<RoomControl>())
                {
                    Debug.Log("adding rooms????");
                    rp.AddRooms(raycast.collider.gameObject);
                }
            }
            player.transform.hasChanged = false;
        }
    }
}
