using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMoveTester : MonoBehaviour
{
    // Start is called before the first frame update

    Vector3 moveDirection;
    Animator anim;

    bool crouched = false;
    bool blocking = false;

    Rigidbody2D rb;

    [SerializeField] float speed = 50f;

    [SerializeField] int pressedButton = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        anim = GetComponentInChildren<Animator>();
        moveDirection = new Vector3(0, 0, 0);
    }



    // Update is called once per frame
    void Update()
    {
        pressedButton = 0;
        moveDirection = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            moveDirection = new Vector3(1, 0, 0);
        }

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            moveDirection = new Vector3(-1, 0, 0);
        }

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            moveDirection += new Vector3(0, 1, 0);
        }

        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            moveDirection += new Vector3(0, -1, 0);
        }

        if (Input.GetKey(KeyCode.Z))
        {
            pressedButton = 1;
        }

        if (Input.GetKey(KeyCode.X))
        {
            pressedButton = 2;
        }

        if (Input.GetKey(KeyCode.C))
        {
            pressedButton = 3;
        }

        ProcessControls(moveDirection);
    }

    void ProcessControls(Vector3 controlDirection)
    {
        if(controlDirection.y > 0)
        {
            //Jump
        }
        else if(controlDirection.y < 0)
        {
            //Crouch
            crouched = true;
        }
        else
        {
            crouched = false;
        }

        anim.SetBool("Crouch", crouched);

        anim.SetTrigger(pressedButton.ToString("0"));

        if(!crouched && (controlDirection.x > 0 || controlDirection.x < 0))
        {
            rb.AddForce(Vector2.left * controlDirection.x * speed * Time.deltaTime);
        }

        //make this dependent on opponent direction
        if(controlDirection.x < 0)
        {
            blocking = true;
        }

    }
}
