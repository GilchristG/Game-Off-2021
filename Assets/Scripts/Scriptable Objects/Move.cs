using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Move", menuName = "Move", order = 1)]
public class Move : ScriptableObject
{
    public string moveName;

    public float damage;
    public float chipDamage;

    public uint startupFrames;
    public uint activeFrames;
    public uint recoveryFramesOnHit;
    public uint recoveryFramesOnWhiff;
}
