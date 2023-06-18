using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Potentially use this as basis for character controllers
public class BasicMoveTester : MonoBehaviour
{
    public FighterState currentState;

    Vector3 moveDirection;
    Animator anim;

    Rigidbody2D rb;

    [SerializeField] float speed = 50f;

    public CharacterInput characterInput = new CharacterInput();
    InputFrame processingFrame;
    int currentDirection;

    public double frameDuration = 0.016f;
    public double nextFrameTime = 0;
    

    private void Awake()
    {
        currentState = FighterState.Neutral;
        characterInput.SetupBV(GetComponent<BufferVisualizer>());
        characterInput.training = true;
    }

    void Start()
    {
        processingFrame = new InputFrame();

        rb = GetComponent<Rigidbody2D>();

        anim = GetComponent<Animator>();
    }

    //Change this to reieve the input frame from a central synchronizer
    void Update()
    {
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

        if (Input.GetKeyDown(KeyCode.Z))
        {
            processingFrame.inputs[1] = 1;
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            processingFrame.inputs[2] = 1;
        }

        if (Input.GetKeyDown(KeyCode.C))
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

        if (nextFrameTime < Time.time)
        {
            InputFrame currentFrame = new InputFrame();

            currentFrame.inputs[0] = currentDirection;
            currentFrame.inputs[1] = processingFrame.inputs[1];
            currentFrame.inputs[2] = processingFrame.inputs[2];
            currentFrame.inputs[3] = processingFrame.inputs[3];

            characterInput.PushIntoBuffer(currentFrame);
            CheckInput();
            ProcessMoves();

            //Reset the inputs
            processingFrame = new InputFrame();
            nextFrameTime = Time.time + frameDuration;
        }
    }

    int[] qcf = new int[] { 2, 3, 6 };

    int[] right = new int[] {6};

    int[] neutral = new int[] {5};

    //What we would want to do is have an extensive tree for checking move chains. Check from more complicated to less complicated. 
    //If a move is detected, push into a queue and at the end of frame check if it can be activated (early input buffer, is the player in block or hit stun, etc)
    //otherwise chuck it from the queue and continue to next frame
    public void CheckInput()
    {

        if (characterInput.CheckSequence(qcf, 16))
        {
            Debug.Log("Found qcf");
        }

        if (characterInput.GetCurrentFrame().inputs[1] == 1)
        {
            Debug.Log("Punch button detected");

            if (characterInput.CheckSequence(qcf,16))
            {
                //anim.SetTrigger("Hit");
                moveQueue.Enqueue(new Move(8, "Hit"));
            }
            else if (characterInput.CheckSequence(neutral, 16))
            {
                //anim.SetTrigger("1");
                moveQueue.Enqueue(new Move(8, "1"));
            }
        }
    }

    //tracked in frames
    public int elapsedMoveTime = 0;
    Move currentMove;
    public Queue<Move> moveQueue = new Queue<Move>();

    public void ProcessMoves()
    {
        Debug.Log("Number in Queue: " + moveQueue.Count);


        //Handles the list of moves currently processing
        if (moveQueue.Count > 0)
        {
            Debug.Log("Move trigger: "+moveQueue.Peek().moveTrigger);

            //THIS KEEPS TRACK OF FRAMES. REMEMBER THAT
            elapsedMoveTime += 1;
            Move current = moveQueue.Peek();
            if (elapsedMoveTime > current.totalDuration)
            {
                elapsedMoveTime -= current.totalDuration;
                moveQueue.Dequeue();
                currentMove = null;
                Debug.Log(current.moveTrigger + " was current move");
            }
        }
        else
        {
            elapsedMoveTime = 0;

            //Process non-move actions like walking, crouching, etc here
        }

        //This is put here in case the queue empties in previous loop
        if (moveQueue.Count > 0)
        {
            if (currentMove != null)
                return;
            StartMove(moveQueue.Peek());
        }
    }

    public void StartMove(Move newMove)
    {
        currentMove = newMove;

        //Do all the processing for the move here
        anim.SetTrigger(currentMove.moveTrigger);
    }

    public void OnContact()
    {
        if (currentState == FighterState.Neutral || currentState == FighterState.Hit || currentState == FighterState.Attacking)
        {
            //Process normal hit. Can add punishment for being in an attack later

            currentState = FighterState.Hit;

            //Send player into hit stun
        }
        else if (currentState == FighterState.Blocking)
        {
            //Send player into block stun
        }
        else
        {
            //Player is invincible. They are uneffected
        }

    }
}
