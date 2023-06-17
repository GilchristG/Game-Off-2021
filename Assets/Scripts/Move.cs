using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Move
{
    //public MoveData moveData;
    public float totalDuration;
    public string moveTrigger;

    public Move()
    {
        totalDuration = 0;
        moveTrigger = "";
    }

    public Move(float td, string trigger)
    {
        totalDuration = td;
        moveTrigger = trigger;
    }
}
