using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCharacterController : MonoBehaviour
{
    public FighterState currentState;

    private void Awake()
    {
        currentState = FighterState.Neutral;
    }

    private void Start()
    {
        
    }

    //Recieve inputs here from consistent input handler. Should it be an input stream or what?
    public void Move()
    {

    }

    //If an enemy attack touches you
    public void OnContact()
    {
        if(currentState == FighterState.Neutral || currentState == FighterState.Hit || currentState == FighterState.Attacking)
        {
            //Process normal hit. Can add punishment for being in an attack later

            currentState = FighterState.Hit;

            //Send player into hit stun
        }
        else if(currentState == FighterState.Blocking)
        {
            //Send player into block stun
        }
        else
        {
            //Player is invincible. They are uneffected
        }

    }




}
