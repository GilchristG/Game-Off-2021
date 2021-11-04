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
        animator.SetTrigger("LightAttack");
    }

    private void HeavyAttack(InputAction.CallbackContext context)
    {
        animator.SetTrigger("HeavyAttack");
    }

    private void SpecialAttack(InputAction.CallbackContext context)
    {
        animator.SetTrigger("SpecialAttack");
    }

    private void Update()
    {
        animator.SetBool("Grounded", isGrounded());
        animator.SetFloat("HorSpeed", rb.velocity.x);
        animator.SetBool("IsStationary", rb.velocity.x == 0 ? true : false);
        lateralMovement(); // For some reason, if this is done in FixedUpdate, input is ignored 99% of the time
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
