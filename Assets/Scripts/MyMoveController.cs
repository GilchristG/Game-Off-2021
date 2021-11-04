using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MyMoveController : MonoBehaviour
{

    private PlayerInput playerInput;
    private Rigidbody2D rb;
    private Animator animator;

    private InputAction jumpAction;
    private InputAction moveAction;
    private InputAction lightAttackAction;
    private InputAction heavyAttackAction;
    private InputAction specialAttackAction;

    public List<CommandInputs> buffer;

    public float bufferLength = 0.25f;
    public float multiTimer = 0f;

    public bool acceptingCommands = true;
    public bool isPressingButtons = false;

    private void Start()
    {
        buffer = new List<CommandInputs>();


        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();

        jumpAction = playerInput.actions["Jump"];
        moveAction = playerInput.actions["Move"];
        lightAttackAction = playerInput.actions["LightAttack"];
        heavyAttackAction = playerInput.actions["HeavyAttack"];
        specialAttackAction = playerInput.actions["SpecialAttack"];

        lightAttackAction.performed += LightAttack;
        heavyAttackAction.performed += HeavyAttack;
        specialAttackAction.performed += SpecialAttack;
    }

    private void Update()
    {
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

            foreach(CommandInputs ci in buffer)
            {
                bufferResult += ci.ToString();
            }

            Debug.Log(bufferResult);

            if(!bufferResult.Equals(""))
            {
                animator.SetTrigger("LightAttack");
            }

            buffer.Clear();
        }


        //Check the state and what move should come next given the input. Otherwise, don't do anything
        


        //acceptingCommands = false;
    }

    private void Jump(InputAction.CallbackContext context)
    {
        


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


}
