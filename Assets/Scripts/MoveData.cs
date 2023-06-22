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

    [Tooltip("Animation trigger name for use in animator")]
    [SerializeField] string animationName;
    [Tooltip("Animation clip for direct use")]
    [SerializeField] AnimationClip animationClip;

    [SerializeField] int damage;
    [SerializeField] int windupTime;
    [SerializeField] int activeTime;

    [Tooltip("The amount of time the attacker must recover for after attacking")]
    [SerializeField] int basicRecovery;

    
    [Tooltip("The last number of frames during recovery that will let you queue up a new move")]
    [SerializeField] int bufferWindow;

    [Tooltip("The amount of frames the opponent must be in hit stun for before gaining control")]
    [SerializeField] int hitStunTime;
    [Tooltip("The amount of frames the opponent must block for before gaining control")]
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

    public void CheckAnimationFrames()
    {
        int allEvents = animationClip.events.Length;

        int totalFrames = Mathf.RoundToInt(animationClip.length * animationClip.frameRate);

        int windup = 0;
        int active = 0;
        int recovery = 0;

        int activeFrameStart = 0;
        int recoveryStart = 0;

        for (int i = 0; i < allEvents; i++)
        {
            if(animationClip.events[i].functionName == "Active")
            {
                activeFrameStart = Mathf.RoundToInt(animationClip.events[i].time * animationClip.frameRate);
                windup = activeFrameStart - 1;
            }
            else if (animationClip.events[i].functionName == "Recovery")
            {
                recoveryStart = Mathf.RoundToInt(animationClip.events[i].time * animationClip.frameRate);
                active = recoveryStart - activeFrameStart;
                recovery = totalFrames - recoveryStart;
            }
        }

        if(windup > 0)
        {
            windupTime = windup;
        }

        if(active > 0)
        {
            activeTime = active;
        }

        if(recovery > 0)
        {
            basicRecovery = recovery;
        }


    }
}
