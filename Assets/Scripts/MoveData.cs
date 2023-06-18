using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Move",menuName = "Chracter Assets/MoveData", order = 0)]
public class MoveData : ScriptableObject
{
    [SerializeField] string animationName;

    [SerializeField] float damage;
    [SerializeField] int windupTime;
    [SerializeField] int activeTime;
    [SerializeField] int basicRecovery;
    [Tooltip("The last number of frames during recovery that will let you queue up a new move")]
    [SerializeField] int bufferWindow;

    //NOTE: Don't forget to put these values in frames (about 16ms each)
    //Unused at the moment
    float recoveryTimeHit;
    float recoveryTimeMiss;
    float recoveryTimeBlocked;
    float hitStunTime;
    float blockStunTime;
    Vector2 knockbackDir;
    float knockbackForce;

    public Move CreateMove()
    {
        Move newMove = new Move(animationName, damage, windupTime, activeTime, basicRecovery, bufferWindow);
        return newMove;
    }
}
