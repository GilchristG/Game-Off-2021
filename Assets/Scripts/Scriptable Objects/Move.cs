using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Move", menuName = "Move", order = 1)]
public class Move : ScriptableObject
{
    public string moveName;
    public string moveAnimation;

    public float damage;
    public float chipDamage; //Damage dealt to a blocking character

    public uint startupFrames;
    public uint activeFrames;
    public uint recoveryFramesOnHit;
    public uint recoveryFramesOnWhiff;

    public uint hitStunFrames; // Frames of stun dealt to a non-blocking character
    public uint blockStunFrames; // Frames of stun dealt to a blocking character
}
