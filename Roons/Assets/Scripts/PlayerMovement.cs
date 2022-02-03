using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    float horz, vert;
    public float speed;

    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector3(horz * speed, vert * speed, 0);
        //transform.position += new Vector3(horz * speed, vert * speed, 0);
    }

    void GetInput()
    {
        horz = Input.GetAxis("Horizontal");
        vert = Input.GetAxis("Vertical");
    }
}
