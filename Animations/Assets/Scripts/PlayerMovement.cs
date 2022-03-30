using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float inputX, inputY, speed, strafespeed, runspeed, acceleration, deceleration, currentSpeed;
    Rigidbody rb;
    bool running;
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");

        if(Input.GetKey(KeyCode.LeftShift))
        {
            running = true;
            if(currentSpeed < runspeed)
            {
                currentSpeed += acceleration;
            }
            else
            {
                currentSpeed = runspeed;
            }
        }
        else
        {
            running = false;
            if (currentSpeed > speed)
            {
                currentSpeed -= deceleration;
            }
            else
            {
                currentSpeed = speed;
            }
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector3(inputX * strafespeed, rb.velocity.y, inputY * currentSpeed);
        anim.SetFloat("VelX", rb.velocity.z/2f);
        anim.SetFloat("VelZ", rb.velocity.x);
    }
}