using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Move",menuName = "Character Assets/MoveData", order = 0)]
[System.Serializable]
public class MoveData : ScriptableObject
{
    [Tooltip("Motion sequence needed from P1 position")]
    public MotionType motionSequence;
    [Tooltip("Buttons needed at the same time")]
    public Button[] attackbuttons;

    [SerializeField] string animationName;

    [SerializeField] int damage;
    [SerializeField] int windupTime;
    [SerializeField] int activeTime;
    [SerializeField] int basicRecovery;
    [Tooltip("The last number of frames during recovery that will let you queue up a new move")]
    [SerializeField] int bufferWindow;

    [SerializeField] int hitStunTime;
    [SerializeField] int blockStunTime;
    [SerializeField] float knockbackForce;
    [SerializeField] float launchForce;

    //NOTE: Don't forget to put these values in frames (about 16ms each)
    //Unused at the moment
    float recoveryTimeHit;
    float recoveryTimeMiss;
    float recoveryTimeBlocked;

    public Move CreateMove()
    {
        Move newMove = new Move(animationName, damage, windupTime, activeTime, basicRecovery, bufferWindow, hitStunTime, blockStunTime, knockbackForce, launchForce);
        return newMove;
    }
}
