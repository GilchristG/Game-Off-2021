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

    private PlayerInput playerInput;
    private Rigidbody2D rb;
    private Animator animator;

    private CharacterControls characterControls; // This is the generated json file
    private AnimationController animationController;

    private MoveSet moveSet;
    private Move currentMove;

    //Input Actions
    private InputAction jumpAction;
    private InputAction moveAction;
    private InputAction lightAttackAction;
    private InputAction heavyAttackAction;
    private InputAction specialAttackAction;

    private float jumpForce;
    private float speed;
    private float maxSpeed;



    private void Awake()
    {
        moveSet = characterStats.moveSet;
        jumpForce = characterStats.jumpForce;
        speed = characterStats.speed;
        maxSpeed = characterStats.maxSpeed;

        currentMove = null;

        isBlocking = true;

        characterControls = new CharacterControls();
        
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        animationController = GetComponentInChildren<AnimationController>();

        //Set the Input Actions
        moveAction = characterControls.InGame.Move; // This is one way to set it
        jumpAction = playerInput.actions["Jump"]; // This is a slightly different way that doesn't use the json file
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
        if (isGrounded())
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void LightAttack(InputAction.CallbackContext context)
    {
        //animator.SetTrigger("LightAttack");
        buffer.Add(CommandInputs.light);
    }

    private void HeavyAttack(InputAction.CallbackContext context)
    {
        //animator.SetTrigger("HeavyAttack");
        buffer.Add(CommandInputs.heavy);
    }

    private void SpecialAttack(InputAction.CallbackContext context)
    {
        //animator.SetTrigger("SpecialAttack");
        buffer.Add(CommandInputs.special);
    }



    public List<CommandInputs> buffer;

    public float bufferLength = 0.25f;
    public float multiTimer = 0f;

    public bool acceptingCommands = true;
    public bool isPressingButtons = false;

    private void Update()
    {
        animator.SetBool("Grounded", isGrounded());
        animator.SetFloat("HorSpeed", rb.velocity.x);
        animator.SetBool("IsStationary", rb.velocity.x == 0 ? true : false);
        lateralMovement(); // For some reason, if this is done in FixedUpdate, input is ignored 99% of the time


        //Check if the move can even be entered before checking the buffer
        if (buffer.Count > 0)
            isPressingButtons = true;
        else
            isPressingButtons = false;


        if (!acceptingCommands)
            return;

        //Check if two or more buttons are pressed
        multiTimer += Time.deltaTime;
        if (multiTimer > bufferLength)
        {
            multiTimer = 0;

            //Get the last 1-3 inputs depending on how we check last movement for moves

            string bufferResult = "";

            foreach (CommandInputs ci in buffer)
            {
                bufferResult += ci.ToString();
            }

            Debug.Log(bufferResult);

            if (!bufferResult.Equals(""))
            {
                acceptingCommands = false;
                animator.SetTrigger("LightAttack");
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

            if (Mathf.Abs(rb.velocity.x) < maxSpeed)
            {
                rb.AddForce(new Vector2(inputVector.x, 0) * speed, ForceMode2D.Force);
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
