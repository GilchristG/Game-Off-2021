using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Move
{
    //public MoveData moveData;
    public int totalDuration;
    public string moveTrigger;

    public Move()
    {
        totalDuration = 0;
        moveTrigger = "";
    }

    public Move(int td, string trigger)
    {
        totalDuration = td;
        moveTrigger = trigger;
    }
}
