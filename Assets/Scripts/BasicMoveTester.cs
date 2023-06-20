using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//Potentially use this as basis for character controllers
public class BasicMoveTester : MonoBehaviour
{
    public FighterState currentState;
    public FighterStance currentStance;

    Vector3 moveDirection;
    Animator anim;

    Rigidbody2D rb;

    [SerializeField] float speed = 50f;
    [SerializeField] float jumpMultiplier = 1.5f;

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
            ProcessFighterState();
            ProcessFighterMovement();
            ProcessAnimations();

            //Reset the inputs
            processingFrame = new InputFrame();
            nextFrameTime = Time.time + frameDuration;
        }
    }

    public MotionChecker motionChecker = new MotionChecker();
    public MoveSet moveSet;

    public List<MoveData> subSection = new List<MoveData>();

    //What we would want to do is have an extensive tree for checking move chains. Check from more complicated to less complicated. 
    //If a move is detected, push into a queue and at the end of frame check if it can be activated (early input buffer, is the player in block or hit stun, etc)
    //otherwise chuck it from the queue and continue to next frame

    bool facingRight = true;

    void CheckInput()
    {
        InputFrame currentFrame = characterInput.GetCurrentFrame();

        //This properly detects the most complicated moves first
        List<MotionType> movesDetected = motionChecker.CheckForAllApplicable(characterInput, facingRight);

        if (currentFrame.inputs[1] == 1 || currentFrame.inputs[2] == 1 || currentFrame.inputs[3] == 1)
        {

            foreach (MoveData md in moveSet.moves)
            {
                if (movesDetected.Contains(md.motionSequence))
                {
                    subSection.Add(md);
                }
            }

            //TODO: Make sure all potential moves are shown

            //Sorts the moves by motion complexity so more complex actions will activate first if possible
            IComparer<MoveData> ms = new MoveSorter();
            subSection.Sort(ms);

            foreach (MoveData md in subSection)
            {
                //TODO: Add checks for multiple buttons
                if (currentFrame.inputs[(int)md.attackbuttons[0] + 1] == 1)
                {
                    moveQueue.Enqueue(md.CreateMove());
                }
            }

            //Debug.LogError("Pause for checking buffer");
        }

        subSection.Clear();
        movesDetected.Clear();
    }

    //NOTE: THIS KEEPS TRACK OF FRAMES. REMEMBER THAT
    public int elapsedMoveTime = 0;

    public int windupFrameTimer = 0;
    public int activeFrameTimer = 0;
    public int recoveryFrameTimer = 0;
    public int totalMoveFrameTime = 0;
    public int stunTimer = 0;

    public bool processingMove = false;
    public Move currentMove = null;
    public Queue<Move> moveQueue = new Queue<Move>();

    void ProcessMoves()
    {
        Debug.Log("Number in Queue: " + moveQueue.Count);

        //This is put here in case the queue empties in previous loop
        if (moveQueue.Count > 0)
        {
            //Can potnetially put cancelling info here
            if (!processingMove && !IsStunned())
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
                    if (windupFrameTimer <= 0)
                    {
                        windupFrameTimer = 0;
                        activeFrameTimer = currentMove.activeTime;
                        currentMove.moveState = MoveState.Active;
                    }
                    windupFrameTimer--;

                    break;
                case MoveState.Active:
                    if (activeFrameTimer <= 0)
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
                        EndMove();
                    }
                    recoveryFrameTimer--;

                    break;
            }

            //Get the next move in the queue for chaining purposes
            if (moveQueue.Count > 0)
            {
                //This should allow for some stupid buffer windows if needed
                if (currentMove != null && ((totalMoveFrameTime - elapsedMoveTime) < currentMove.bufferWindow))
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

    void StartMove(Move newMove)
    {
        processingMove = true;
        currentState = FighterState.Attacking;

        currentMove = newMove;
        currentMove.moveState = MoveState.Windup;
        windupFrameTimer = currentMove.windupTime;

        totalMoveFrameTime = currentMove.windupTime + currentMove.activeTime + currentMove.basicRecovery;

        //Do all the intial processing for the move here
        anim.SetTrigger(currentMove.animationName);
    }

    void EndMove()
    {
        currentState = FighterState.Neutral;

        windupFrameTimer = 0;
        activeFrameTimer = 0;
        recoveryFrameTimer = 0;
        elapsedMoveTime = 0;
        currentMove = null;
        processingMove = false;
    }

    void OnHit()
    {
        currentState = FighterState.Hit;
        //Stop all player momentum. 
        //NOTE: Potentially add a little back force here
        rb.velocity = new Vector2(0, 0);
        EndMove();
    }

    public void OnContact(Move hittingMove)
    {
        if (currentState == FighterState.Neutral || currentState == FighterState.Hit || currentState == FighterState.Attacking)
        {
            //Process normal hit. Can add punishment for being in an attack later
            //Send player into hit stun
            OnHit();

            stunTimer = hittingMove.hitStunTime;
        }
        else if (currentState == FighterState.Blocking)
        {
            //Send player into block stun
            stunTimer = hittingMove.blockStunTime;
        }
        else
        {
            //Player is invincible. They are uneffected
        }

    }

    void ProcessFighterState()
    {
        InputFrame currentFrame = characterInput.GetCurrentFrame();

        if (stunTimer > 0)
        {
            stunTimer--;
        }

        if (stunTimer <= 0)
        {
            stunTimer = 0;
            //Let the player get out of a stun into blocking. If holding appropriate buttons, player can block immediately
            currentState = FighterState.Blocking;
        }


        //Get opponent position and check if left or right
        //facingRight = true;

        if (currentStance != FighterStance.Airborne && (currentState != FighterState.Attacking || currentState != FighterState.Hit) && !IsStunned())
        {
            if (facingRight)
            {
                if (currentFrame.inputs[0] == 4 || currentFrame.inputs[0] == 1)
                {
                    currentState = FighterState.Blocking;
                }
                else
                {
                    currentState = FighterState.Neutral;
                }
            }
            else
            {
                if (currentFrame.inputs[0] == 6 || currentFrame.inputs[0] == 3)
                {
                    currentState = FighterState.Blocking;
                }
                else
                {
                    currentState = FighterState.Neutral;
                }
            }
        }
    }

    void ProcessFighterMovement()
    {
        InputFrame currentFrame = characterInput.GetCurrentFrame();

        //Might want to make sub states for each one. Force blocking
        if ((currentState != FighterState.Attacking && currentState != FighterState.Hit) && !IsStunned())
        {
            if(currentStance != FighterStance.Crouching && currentStance != FighterStance.Airborne)
            {
                if (moveDirection.y > 0)
                {
                    currentStance = FighterStance.Airborne;
                }
                else if (moveDirection.y < 0)
                {
                    currentStance = FighterStance.Crouching;
                }
                moveDirection.y *= jumpMultiplier;
                rb.velocity = moveDirection * speed;
            }
            else if(currentStance == FighterStance.Crouching)
            {
                if(moveDirection.y == 0)
                {
                    currentStance = FighterStance.Standing;
                }
                else if(moveDirection.y > 0)
                {
                    currentStance = FighterStance.Airborne;
                    rb.velocity = moveDirection * speed;
                }
                
            }
        }
    }

    void ProcessAnimations()
    {
        if (currentState == FighterState.Hit && stunTimer > 0)
        {
            anim.SetInteger("StunTimer",stunTimer);
            anim.SetTrigger("Hit");
        }
        else if (currentState == FighterState.Blocking && stunTimer > 0)
        {
            anim.SetInteger("StunTimer", stunTimer);
            anim.SetTrigger("Block");
        }
        else
        {
            anim.SetBool("Airborne", currentStance == FighterStance.Airborne);
            anim.SetBool("Crouch", currentStance == FighterStance.Crouching);
            anim.SetFloat("VelTowards", rb.velocity.x);
        }
    }


    bool IsStunned()
    {
        if (stunTimer > 0)
        {
            return true;
        }

        return false;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            currentStance = FighterStance.Standing;
            anim.SetBool("Airborne", false);
        }
    }
}
