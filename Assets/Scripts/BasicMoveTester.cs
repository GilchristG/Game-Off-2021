using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

//Potentially use this as basis for character controllers
public class BasicMoveTester : MonoBehaviour
{
    //Character events
    //Transform hitLocation for spawning effects, Int damage to determine effect size
    public Action<Vector3, int> onPlayerHit;
    public Action<int> onPlayerHealthLoss;
    public Action onPlayerKnockedOut;
    public Action<Move> onPlayerMoveStart;
    public Action onPlayerMoveFinished;


    public int playerHealth = 100;

    public FighterState currentState;
    public FighterStance currentStance;

    Vector3 moveDirection;
    Animator anim;

    Rigidbody2D rb;

    [SerializeField] float speed = 5f;
    [SerializeField] float jumpMultiplier = 3f;

    public CharacterInput characterInput = new CharacterInput();
    int currentDirection;

    public double frameDuration = 0.016f;
    public double nextFrameTime = 0;

    public Transform opponentTransform;
    [SerializeField] bool touchingOpponent = false;

    HitboxManager hitboxManager;
    HurtboxManager hurtboxManager;

    

    private void Awake()
    {
        currentState = FighterState.Neutral;
        characterInput.SetupBV(GetComponent<BufferVisualizer>());
        characterInput.training = true;
        hitboxManager = GetComponentInChildren<HitboxManager>();
        hitboxManager.SetParent(this);
        hurtboxManager = GetComponentInChildren<HurtboxManager>();
        hurtboxManager.SetSelfHitbox(hitboxManager);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        anim = GetComponent<Animator>();
    }

    //Change this to reieve the input frame from a central synchronizer
    public void FrameUpdate(Vector3 movDir, int[] attackButtons)
    {
        moveDirection = movDir;

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
        currentFrame.inputs[1] = attackButtons[0];
        currentFrame.inputs[2] = attackButtons[1];
        currentFrame.inputs[3] = attackButtons[2];

        characterInput.PushIntoBuffer(currentFrame);

        CheckInput();
        ProcessMoves();
        ProcessFighterState();
        ProcessFighterMovement();
        ProcessAnimations();

        //Reset the inputs
    }

    public MotionChecker motionChecker = new MotionChecker();
    public MoveSet moveSet;

    public List<MoveData> subSection = new List<MoveData>();

    //What we would want to do is have an extensive tree for checking move chains. Check from more complicated to less complicated. 
    //If a move is detected, push into a queue and at the end of frame check if it can be activated (early input buffer, is the player in block or hit stun, etc)
    //otherwise chuck it from the queue and continue to next frame

    [SerializeField] bool facingRight = true;
    bool previousState = true;

    void CheckInput()
    {
        if (moveSet == null)
            return;

        InputFrame currentFrame = characterInput.GetCurrentFrame();

        //This properly detects the most complicated moves first
        List<MotionType> movesDetected = motionChecker.CheckForAllApplicable(characterInput, facingRight);

        //May have to remove this if we add character specific dashes
        if (currentFrame.inputs[1] == 1 || currentFrame.inputs[2] == 1 || currentFrame.inputs[3] == 1)
        {

            foreach (MoveData md in moveSet.moves)
            {
                if (movesDetected.Contains(md.motionSequence) && (currentStance == md.neededStance ))
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
        //Debug.Log("Number in Queue: " + moveQueue.Count);

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


            //Debug.Log("Current move " + currentMove.animationName);

            switch (currentMove.moveState)
            {
                case MoveState.Windup:
                    if (windupFrameTimer <= 0)
                    {
                        windupFrameTimer = 0;
                        activeFrameTimer = currentMove.activeTime;
                        currentMove.moveState = MoveState.Active;
                        hurtboxManager.ActivateHurtbox(currentMove);
                    }
                    windupFrameTimer--;

                    break;
                case MoveState.Active:
                    if (activeFrameTimer <= 0)
                    {
                        activeFrameTimer = 0;
                        recoveryFrameTimer = currentMove.basicRecovery;
                        currentMove.moveState = MoveState.Recovery;
                        hurtboxManager.DeactivateHurtbox();
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
        onPlayerMoveStart?.Invoke(currentMove);
        currentMove.moveState = MoveState.Windup;
        windupFrameTimer = currentMove.windupTime;

        totalMoveFrameTime = currentMove.windupTime + currentMove.activeTime + currentMove.basicRecovery;

        //hurtboxManager.ActivateHurtbox(currentMove);

        //Do all the intial processing for the move here
        anim.SetTrigger(currentMove.animationName);
    }

    void EndMove()
    {
        //Might change this to blocking so the player can immediately block out of a move recovery
        if(currentState != FighterState.Hit || currentState != FighterState.Blocking)
            currentState = FighterState.Neutral;

        onPlayerMoveFinished?.Invoke();
        hurtboxManager.DeactivateHurtbox();
        windupFrameTimer = 0;
        activeFrameTimer = 0;
        recoveryFrameTimer = 0;
        elapsedMoveTime = 0;
        currentMove = null;
        processingMove = false;
    }

    void OnHit(Move hittingMove)
    {
        playerHealth -= hittingMove.damage;
        onPlayerHealthLoss?.Invoke(hittingMove.damage);

        if(playerHealth <= 0)
        {
            onPlayerKnockedOut?.Invoke();
        }

        stunTimer = hittingMove.hitStunTime;

        currentState = FighterState.Hit;
        //Stop all player momentum. 
        //TODO: Add the knockback forces here


        Vector2 newForce = new Vector2(hittingMove.knockbackForce, hittingMove.launchForce);
        newForce.x = newForce.x * (facingRight ? -1f : 1f);

        rb.velocity = newForce;
        //EndMove();
    }

    public void OnContact(Move hittingMove, Vector3 hitLocation)
    {
        if (currentState == FighterState.Neutral || currentState == FighterState.Hit || currentState == FighterState.Attacking)
        {
            //Process normal hit. Can add punishment for being in an attack later
            //Send player into hit stun
            OnHit(hittingMove);

            stunTimer = hittingMove.hitStunTime;

            if (hitLocation != null)
            {
                onPlayerHit?.Invoke(hitLocation, hittingMove.damage);
            }
        }
        else if (currentState == FighterState.Blocking)
        {
            //Send player into block stun
            stunTimer = hittingMove.blockStunTime;

            if(hitLocation!=null)
            {
                onPlayerHit?.Invoke(hitLocation, 0);
            }
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
            if (stunTimer <= 0)
            {
                stunTimer = 0;
                //Let the player get out of a stun into blocking. If holding appropriate buttons, player can block immediately
                if (currentStance == FighterStance.Airborne)
                {
                    currentState = FighterState.Neutral;
                }
                else
                {
                    currentState = FighterState.Blocking;
                }
            }
        }

        //Get opponent position and check if left or right
        if ((opponentTransform.position - transform.position).x > 0)
        {
            facingRight = true;
        }
        else
        {
            facingRight = false;
        }


        if (currentStance != FighterStance.Airborne && !(currentState == FighterState.Attacking || currentState == FighterState.Hit) && !IsStunned())
        {
            if (previousState != facingRight)
            {
                previousState = facingRight;
                gameObject.transform.localScale = new Vector3(facingRight?5:-5, 5, 1f);
            }

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
        //Might want to make sub states for each one. Force blocking
        if (!(currentState == FighterState.Attacking || currentState == FighterState.Hit) && !IsStunned())
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

        if(touchingOpponent)
        {
            //Add a slight back force when touching the opponent.
            Vector2 adjustVelocity = (opponentTransform.position - transform.position) * 0.2f;
            adjustVelocity.y = 0;

            

            rb.velocity += adjustVelocity;

            //Make sure to account for being airborne.
        }

    }

    void ProcessAnimations()
    {
        anim.SetInteger("StunTimer", stunTimer);

        if (currentState == FighterState.Hit && IsStunned())
        {
            anim.SetTrigger("Hit");
        }
        else if (currentState == FighterState.Blocking && IsStunned())
        {
            anim.SetTrigger("Block");
        }
        else if(currentState == FighterState.Attacking)
        {
            anim.SetFloat("VelTowards", 0);
        }
        else
        {
            anim.SetBool("Airborne", currentStance == FighterStance.Airborne);
            anim.SetBool("Crouch", currentStance == FighterStance.Crouching);
            anim.SetFloat("VelTowards", rb.velocity.x);
        }
    }

    public void AddForceX(float forceX)
    {
        rb.velocity = new Vector2(forceX, rb.velocity.y);
    }

    public void AddForceY(float forceY)
    {
        rb.velocity = new Vector2(rb.velocity.x, forceY);
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
        }

        if(collision.gameObject.tag == "Player")
        {
            touchingOpponent = true;
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            touchingOpponent = false;
        }

        if (collision.gameObject.tag == "Floor")
        {
            currentStance = FighterStance.Airborne;
        }
    }
}
