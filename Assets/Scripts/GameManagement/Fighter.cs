using System.IO;
using UnityEngine;
using Unity.Collections;
using System.Collections.Generic;
using SharedGame;
using System;

[Serializable]
public class Fighter
{
    public Vector2 position;
    public Vector2 velocity;
    public int health;
    public int state;
    public int stance;
    public int move;

    public CharacterInput fighterInput = new CharacterInput();

    //Leaving this here as a reminder we might want projectiles
    //public Bullet[] bullets = new Bullet[MAX_BULLETS];

    public void Serialize(BinaryWriter bw)
    {
        bw.Write(position.x);
        bw.Write(position.y);
        bw.Write(velocity.x);
        bw.Write(velocity.y);
        bw.Write(health);
        bw.Write(state);
        bw.Write(stance);
        bw.Write(move);
        /*for (int i = 0; i < MAX_BULLETS; ++i)
        {
            bullets[i].Serialize(bw);
        }*/
    }

    public void Deserialize(BinaryReader br)
    {
        position.x = br.ReadSingle();
        position.y = br.ReadSingle();
        velocity.x = br.ReadSingle();
        velocity.y = br.ReadSingle();
        health = br.ReadInt32();
        state = br.ReadInt32();
        stance = br.ReadInt32();
        move = br.ReadInt32();
        /*for (int i = 0; i < MAX_BULLETS; ++i)
        {
            bullets[i].Deserialize(br);
        }*/
    }

    // @LOOK Not hashing bullets.
    public override int GetHashCode()
    {
        int hashCode = 1858597544;
        hashCode = hashCode * -1521134295 + position.GetHashCode();
        hashCode = hashCode * -1521134295 + velocity.GetHashCode();
        hashCode = hashCode * -1521134295 + health.GetHashCode();
        hashCode = hashCode * -1521134295 + state.GetHashCode();
        hashCode = hashCode * -1521134295 + stance.GetHashCode();
        hashCode = hashCode * -1521134295 + move.GetHashCode();
        return hashCode;
    }

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
    public bool addingVel;

    //Rigidbody2D rb;
    Custom2DRigidbody customRb;

    [SerializeField] float speed = 5f;
    [SerializeField] float jumpMultiplier = 3f;

    public CharacterInput characterInput = new CharacterInput();
    int currentDirection;

    public double frameDuration = 0.016f;
    public double nextFrameTime = 0;

    public Fighter opponent;
    [SerializeField] bool touchingOpponent = false;

    HitboxManager hitboxManager;
    HurtboxManager hurtboxManager;



    public Fighter()
    {
        customRb = new Custom2DRigidbody();
        currentState = FighterState.Neutral;
        //characterInput.SetupBV(GetComponent<BufferVisualizer>());
        characterInput.training = false;
        /*hitboxManager = GetComponentInChildren<HitboxManager>();
        hitboxManager.SetParent(this);
        hurtboxManager = GetComponentInChildren<HurtboxManager>();
        hurtboxManager.SetSelfHitbox(hitboxManager);*/
        //anim = GetComponent<Animator>();
    }

    //This works to load from resources
    public void Initialize()
    {
        moveSet = (MoveSet)Resources.Load("MoveSets/TestHercules/TestMoveSet");
    }

    //Change this to reieve the input frame from a central synchronizer
    public void FrameUpdate(Vector2 movDir, bool[] attackButtons)
    {
        //No direction input default
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
        currentFrame.inputs[1] = attackButtons[0] ? 1 : 0;
        currentFrame.inputs[2] = attackButtons[1] ? 1 : 0;
        currentFrame.inputs[3] = attackButtons[2] ? 1 : 0;

        characterInput.PushIntoBuffer(currentFrame);

        CheckInput();
        ProcessMoves();
        ProcessFighterState();
        ProcessFighterMovement();

        //Reset the inputs
    }

    public MotionChecker motionChecker = new MotionChecker();
    public MoveSet moveSet;

    public List<MoveData> subSection = new List<MoveData>();

    //What we would want to do is have an extensive tree for checking move chains. Check from more complicated to less complicated. 
    //If a move is detected, push into a queue and at the end of frame check if it can be activated (early input buffer, is the player in block or hit stun, etc)
    //otherwise chuck it from the queue and continue to next frame

    public bool facingRight = true;
    bool previousState = true;

    void CheckInput()
    {
        if (moveSet == null)
            return;

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
                        //hurtboxManager.ActivateHurtbox(currentMove);
                    }
                    windupFrameTimer--;

                    break;
                case MoveState.Active:
                    if (activeFrameTimer <= 0)
                    {
                        activeFrameTimer = 0;
                        recoveryFrameTimer = currentMove.basicRecovery;
                        currentMove.moveState = MoveState.Recovery;
                        //hurtboxManager.DeactivateHurtbox();
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
        //anim.SetTrigger(currentMove.animationName);
    }

    void EndMove()
    {
        //Might change this to blocking so the player can immediately block out of a move recovery
        if (currentState != FighterState.Hit || currentState != FighterState.Blocking)
            currentState = FighterState.Neutral;

        onPlayerMoveFinished?.Invoke();
        //hurtboxManager.DeactivateHurtbox();
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

        if (playerHealth <= 0)
        {
            onPlayerKnockedOut?.Invoke();
        }

        stunTimer = hittingMove.hitStunTime;

        currentState = FighterState.Hit;
        //Stop all player momentum. 
        //TODO: Add the knockback forces here
        customRb.velocity = new Vector2(0, 0);
        //EndMove();
    }

    public void OnContact(Move hittingMove, Vector3 hitLocation)
    {
        if (currentState == FighterState.Neutral || currentState == FighterState.Hit || currentState == FighterState.Attacking)
        {
            //Process normal hit. Can add punishment for being in an attack later
            //Send player into hit stun
            OnHit(hittingMove);

            if (hitLocation != null)
            {
                onPlayerHit?.Invoke(hitLocation, hittingMove.damage);
            }
        }
        else if (currentState == FighterState.Blocking)
        {
            //Send player into block stun
            stunTimer = hittingMove.blockStunTime;

            if (hitLocation != null)
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
                currentState = FighterState.Blocking;
            }
        }

        //Get opponent position and check if left or right
        if ((opponent.position - position).x > 0)
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
                //gameObject.transform.localScale = new Vector3(facingRight ? 5 : -5, 5, 1f);
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

        state = (int)currentState;
        stance = (int)currentStance;
    }

    void ProcessFighterMovement()
    {
        //Might want to make sub states for each one. Force blocking
        if (!(currentState == FighterState.Attacking || currentState == FighterState.Hit) && !IsStunned())
        {
            if (currentStance != FighterStance.Crouching && currentStance != FighterStance.Airborne)
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
                customRb.velocity = moveDirection * speed;
            }
            else if (currentStance == FighterStance.Crouching)
            {
                if (moveDirection.y == 0)
                {
                    currentStance = FighterStance.Standing;
                }
                else if (moveDirection.y > 0)
                {
                    currentStance = FighterStance.Airborne;
                    customRb.velocity = moveDirection * speed;
                }

            }
        }

        if (touchingOpponent)
        {
            //Add a slight back force when touching the opponent.
            Vector2 adjustVelocity = (opponent.position - position) * 0.2f;
            adjustVelocity.y = 0;


            velocity += adjustVelocity;
            //Make sure to account for being airborne.
        }

        if(currentStance == FighterStance.Airborne)
        {
            customRb.isGrounded = false;
        }

        customRb.ProcessTick(0.016f);
        velocity = customRb.velocity;
        position = customRb.position;
    }

    bool IsStunned()
    {
        if (stunTimer > 0)
        {
            return true;
        }

        return false;
    }

}

public class Custom2DRigidbody
{
    public Vector2 position;
    public Vector2 velocity;
    public float friction = 1f;
    public float gravity = 9f;
    public float mass = 3f;

    public bool isGrounded = true;

    public Custom2DRigidbody()
    {
        position = new Vector2(0, 0);
        velocity = new Vector2(0, 0);
    }

    public void ProcessTick(float timeDelta)
    {
        position += velocity * timeDelta;

        if(isGrounded)
        {
            velocity.x -= (friction * mass * gravity * timeDelta)* Math.Sign(velocity.x);
        }
        else
        {
            velocity.y -= ((mass * gravity) * timeDelta);
        }
    }

}
