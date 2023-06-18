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

    public MoveData testMove1;
    public MoveData testMove2;

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
                moveQueue.Enqueue(testMove1.CreateMove());
            }
            else if (characterInput.CheckSequence(neutral, 16))
            {
                //anim.SetTrigger("1");
                moveQueue.Enqueue(testMove2.CreateMove());
            }
        }
    }

    //NOTE: THIS KEEPS TRACK OF FRAMES. REMEMBER THAT
    public int elapsedMoveTime = 0;

    public int windupFrameTimer = 0;
    public int activeFrameTimer = 0;
    public int recoveryFrameTimer = 0;
    public int totalMoveFrameTime = 0;

    public bool processingMove = false;
    public Move currentMove = null;
    public Queue<Move> moveQueue = new Queue<Move>();

    public void ProcessMoves()
    {
        Debug.Log("Number in Queue: " + moveQueue.Count);

        //This is put here in case the queue empties in previous loop
        if (moveQueue.Count > 0)
        {
            //Can potnetially put cancelling info here
            if (!processingMove)
            {
                StartMove(moveQueue.Dequeue());
            }
        }


        //Handles the list of moves currently processing
        if (processingMove && currentMove != null)
        {
            elapsedMoveTime += 1;


            Debug.Log("Current move " + currentMove.animationName);

            switch (currentMove.moveState)
            {
                case MoveState.Windup:
                    if(windupFrameTimer <= 0)
                    {
                        windupFrameTimer = 0;
                        activeFrameTimer = currentMove.activeTime;
                        currentMove.moveState = MoveState.Active;
                    }
                    windupFrameTimer--;

                    break;
                case MoveState.Active:
                    if(activeFrameTimer <= 0)
                    {
                        activeFrameTimer = 0;
                        recoveryFrameTimer = currentMove.basicRecovery;
                        currentMove.moveState = MoveState.Recovery;
                    }
                    else
                    {
                        //Add hurtbox activations stuff here
                    }
                    activeFrameTimer--;

                    break;
                case MoveState.Recovery:
                    if (recoveryFrameTimer <= 0)
                    {
                        //Clear out the currentMove
                        Debug.Log(currentMove.animationName + " total frame time: " + elapsedMoveTime);
                        recoveryFrameTimer = 0;
                        elapsedMoveTime = 0;
                        currentMove = null;
                        processingMove = false;
                    }
                    recoveryFrameTimer--;

                    break;
            }

            //Emergency abandon
            /*if(elapsedMoveTime > totalMoveFrameTime)
            {
                Debug.Log(currentMove.animationName + " total frame time: " + elapsedMoveTime);
                windupFrameTimer = 0;
                activeFrameTimer = 0;
                recoveryFrameTimer = 0;
                elapsedMoveTime = 0;
                currentMove = null;
                processingMove = false;
            }*/


            //Get the next move in the queue for chaining purposes
            if (moveQueue.Count > 0)
            {
                if(currentMove != null && (currentMove.moveState != MoveState.Recovery || recoveryFrameTimer > currentMove.bufferWindow))
                {
                    moveQueue.Dequeue();
                }
            }
        }
        else
        {
            elapsedMoveTime = 0;

            //Process non-move actions like walking, crouching, etc here
        }
    }

    public void StartMove(Move newMove)
    {
        processingMove = true;
        currentMove = newMove;
        currentMove.moveState = MoveState.Windup;
        windupFrameTimer = currentMove.windupTime;

        totalMoveFrameTime = currentMove.windupTime + currentMove.activeTime + currentMove.basicRecovery;

        //Do all the intial processing for the move here
        anim.SetTrigger(currentMove.animationName);
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
