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

    public float hitStunTime;
    public float blockStunTime;
}
