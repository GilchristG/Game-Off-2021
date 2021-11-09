using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This began as an animation controller but it also kinda controls attacks oh well
public class AnimationController : MonoBehaviour
{
    MyCharacterController moveController;
    SpecificMoveManager specificMoveManager;

    // Start is called before the first frame update
    void Start()
    {
        specificMoveManager = GetComponent<SpecificMoveManager>();
        moveController = GetComponentInParent<MyCharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SignalReadyForInput(int trueOrFalse)
    {
        moveController.acceptingCommands = (trueOrFalse == 0)? false:true;
    }

    public void activateColliders()
    {
        GetComponent<BoxCollider2D>().enabled = true;
    }

    public void deactivateColliders()
    {
        GetComponent<BoxCollider2D>().enabled = false;
    }

    public void clearCurrentMove()
    {
        moveController.currentMove = null;
    }

    public void setLightAttack()
    {
        moveController.currentMove = moveController.moveSet.lightAttack;
    }

    public void setHeavyAttack()
    {
        moveController.currentMove = moveController.moveSet.heavyAttack;
    }

    public void setSpecialAttack()
    {
        moveController.currentMove = moveController.moveSet.neutralSpecial;
    }

    public void TriggerSpecialMoveExtras(int number)
    {
        specificMoveManager.DoCoolStuff(number);
    }

    public void setOverrideJump(int number)
    {
        if (number == 1)
            moveController.overrideJumpAnim = true;
        if (number == 0)
            moveController.overrideJumpAnim = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<CharacterHealth>() != null)
        {
            if (collision.gameObject.GetComponent<MyCharacterController>() != null && 
                collision.gameObject.GetComponent<MyCharacterController>().isBlocking)
            {
                collision.gameObject.GetComponent<CharacterHealth>().takeDamage(moveController.currentMove.chipDamage, moveController.currentMove);
            }
            else
            {
                collision.gameObject.GetComponent<CharacterHealth>().takeDamage(moveController.currentMove.damage, moveController.currentMove);
            }
        }
    }
}
