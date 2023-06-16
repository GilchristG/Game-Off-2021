using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//DO NOT USE THIS AS THE BASIS FOR THE GAME SYSTEMS. TESTING ONLY

public class BasicMoveTester : MonoBehaviour
{
    // Start is called before the first frame update

    Vector3 moveDirection;
    Animator anim;

    Rigidbody2D rb;

    [SerializeField] float speed = 50f;

    InputFrame processingFrame;
    int currentDirection;

    void Start()
    {

        rb = GetComponent<Rigidbody2D>();

        anim = GetComponentInChildren<Animator>();
    }


    public CharacterInput characterInput = new CharacterInput();


    void Update()
    {
        processingFrame = new InputFrame();

        moveDirection = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            moveDirection = new Vector3(-1, 0, 0);
        }

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            moveDirection = new Vector3(1, 0, 0);
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
            processingFrame.inputs[1] = 1;
        }

        if (Input.GetKey(KeyCode.X))
        {
            processingFrame.inputs[2] = 1;
        }

        if (Input.GetKey(KeyCode.C))
        {
            processingFrame.inputs[3] = 1;
        }

        //No direction input default
        currentDirection = 5;

        if (moveDirection.y == 0)
        {
            if (moveDirection.x == 1)
            {
                currentDirection = 6;
            }
            else if (moveDirection.x == -1)
            {
                currentDirection = 4;
            }
        }
        else if (moveDirection.y == -1)
        {
            if (moveDirection.x == 1)
            {
                currentDirection = 3;
            }
            else if (moveDirection.x == -1)
            {
                currentDirection = 1;
            }
            else
            {
                currentDirection = 2;
            }
        }
        else if (moveDirection.y == 1)
        {
            if (moveDirection.x == 1)
            {
                currentDirection = 9;
            }
            else if (moveDirection.x == -1)
            {
                currentDirection = 7;
            }
            else
            {
                currentDirection = 8;
            }
        }


        InputFrame currentFrame = new InputFrame();

        currentFrame.inputs[0] = currentDirection;
        currentFrame.inputs[1] = processingFrame.inputs[1];
        currentFrame.inputs[2] = processingFrame.inputs[2];
        currentFrame.inputs[3] = processingFrame.inputs[3];

        characterInput.PushIntoBuffer(currentFrame);
        CheckForMoves();
    }

    int[] qcf = new int[] { 2, 3, 6 };

    int[] right = new int[] {6};


    //What we would want to do is have an extensive tree for checking move chains. Check from more complicated to less complicated. 
    //If a move is detected, push into a queue and at the end of frame check if it can be activated (early input buffer, is the player in block or hit stun, etc)
    //otherwise chuck it from the queue and continue to next frame
    public void CheckForMoves()
    {
        if (characterInput.GetCurrentFrame().inputs[1] == 1)
        {
            if(characterInput.CheckSequence(qcf,16))
            {
                anim.SetTrigger("Hit");
            }
            else if (characterInput.CheckSequence(right, 16))
            {
                anim.SetTrigger("1");
            }
        }
    }
}
