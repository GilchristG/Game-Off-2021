using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Move Set", menuName = "Move Set", order = 2)]
public class MoveSet : ScriptableObject
{
    public string moveSetName;

    public Move lightAttack;
    public Move heavyAttack;
    public Move chargeAttack; // Like a smash attack from Super Smash Bros.

    public Move aerialAttack; // Everyone should essentially just have a nair

    public Move neutralSpecial;
    public Move forwardSpecial;
    public Move downSpecial;
    public Move upSpecial;
}
