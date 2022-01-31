using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    float horz, vert;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
    }

    private void FixedUpdate()
    {
        transform.position += new Vector3(horz * speed, vert * speed, 0);
    }

    void GetInput()
    {
        horz = Input.GetAxis("Horizontal");
        vert = Input.GetAxis("Vertical");
    }
}
