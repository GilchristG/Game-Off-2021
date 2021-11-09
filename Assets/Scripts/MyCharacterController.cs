using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// The name has to have "My" on the beginning or Unity complains
public class MyCharacterController : MonoBehaviour
{
    public CharacterStats characterStats; //Configures speeds and the moveset

    public Transform leftFoot;
    public Transform rightFoot;

    public LayerMask floorMask;

    public bool isBlocking;
    public bool isHitStunned;
    public bool overrideJumpAnim;

    [HideInInspector]
    public Move currentMove;
    [HideInInspector]
    public MoveSet moveSet;

    private PlayerInput playerInput;
    private Rigidbody2D rb;
    private Animator animator;

    private CharacterControls characterControls; // This is the generated json file
    private AnimationController animationController;

    //Input Actions
    private InputAction jumpAction;
    private InputAction moveAction;
    private InputAction lightAttackAction;
    private InputAction heavyAttackAction;
    private InputAction specialAttackAction;

    private void Awake()
    {
        moveSet = characterStats.moveSet;

        currentMove = null;
        isBlocking = true;
        overrideJumpAnim = false;

        characterControls = new CharacterControls();
        
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        animationController = GetComponentInChildren<AnimationController>();

        rb.gravityScale = characterStats.gravityScale;

        //Set the Input Actions
        moveAction = characterControls.InGame.Move;
        jumpAction = characterControls.InGame.Jump;
        lightAttackAction = characterControls.InGame.LightAttack;
        heavyAttackAction = characterControls.InGame.HeavyAttack;
        specialAttackAction = characterControls.InGame.SpecialAttack;

        //Subscribe to the actions
        jumpAction.performed += Jump;
        lightAttackAction.started += LightAttack;
        heavyAttackAction.started += HeavyAttack;
        specialAttackAction.started += SpecialAttack;
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (isGrounded() && acceptingCommands)
            rb.AddForce(Vector2.up * characterStats.jumpForce, ForceMode2D.Impulse);
    }

    private void LightAttack(InputAction.CallbackContext context)
    {
        buffer.Add(CommandInputs.light);
    }

    private void HeavyAttack(InputAction.CallbackContext context)
    {
        buffer.Add(CommandInputs.heavy);
    }

    private void SpecialAttack(InputAction.CallbackContext context)
    {
        buffer.Add(CommandInputs.special);
    }



    public List<CommandInputs> buffer;

    public float bufferLength = 0.25f;
    public float multiTimer = 0f;

    public bool acceptingCommands = true;
    public bool isPressingButtons = false;

    private void Update()
    {
        if (!overrideJumpAnim)
            animator.SetBool("Grounded", isGrounded());
        else
            animator.SetBool("Grounded", true);
        animator.SetFloat("HorSpeed", rb.velocity.x);
        animator.SetBool("IsStationary", rb.velocity.x == 0 ? true : false);
        lateralMovement(); // For some reason, if this is done in FixedUpdate, input is ignored 99% of the time

        //Check if the move can even be entered before checking the buffer
        if (buffer.Count > 0)
        {
            isPressingButtons = true;
        }
        else
        {
            isPressingButtons = false;
        }

        if (!acceptingCommands)
        {
            buffer.Clear();
            return;
        }

        //Check if two or more buttons are pressed
        multiTimer += Time.deltaTime;
        if (multiTimer > bufferLength)
        {
            multiTimer = 0;

            //Get the last 1-3 inputs depending on how we check last movement for moves

            byte bufferResult = 0b_0000;

            foreach (CommandInputs ci in buffer)
            {
                switch(ci)
                {
                    case CommandInputs.light:
                        bufferResult |= 0b_0001;
                        break;
                    case CommandInputs.heavy:
                        bufferResult |= 0b_0010;
                        break;
                    case CommandInputs.special:
                        bufferResult |= 0b_0100;
                        break;
                }
                
            }

            switch (bufferResult)
            {
                case 0b_0001:
                    acceptingCommands = false;
                    if (!isHitStunned)
                        animator.SetTrigger("LightAttack");
                    break;
                case 0b_0010:
                    acceptingCommands = false;
                    if (!isHitStunned)
                        animator.SetTrigger("HeavyAttack");
                    break;
                case 0b_0100:
                    acceptingCommands = false;
                    if (!isHitStunned)
                        animator.SetTrigger("SpecialAttack");
                    break;
            }


            buffer.Clear();
        }


        //Check the state and what move should come next given the input. Otherwise, don't do anything

        //acceptingCommands = false;
    }

    private bool isGrounded()
    {
        if (Physics2D.Raycast(leftFoot.position, Vector2.down, 0.1f, floorMask)
            || Physics2D.Raycast(rightFoot.position, Vector2.down, 0.1f, floorMask))
        {
            return true;
        }
        return false;
    }

    private void lateralMovement()
    {
        if (isGrounded()) //Will only check for lateral movement while grounded. Experimental
        {
            Vector2 inputVector = moveAction.ReadValue<Vector2>();

            if (Mathf.Abs(rb.velocity.x) < characterStats.maxSpeed)
            {
                rb.AddForce(new Vector2(inputVector.x, 0) * characterStats.speed, ForceMode2D.Force);
            }
            if (inputVector.Equals(Vector2.zero))
            {
                // Hits the character with a force exactly equal opposite to its X velocity
                // This stops the 
                rb.AddForce(new Vector2(-rb.velocity.x, 0));
            }
        }
    }

    private void OnEnable()
    {
        characterControls.InGame.Enable();
    }

    private void OnDisable()
    {
        characterControls.InGame.Disable();
    }
}
