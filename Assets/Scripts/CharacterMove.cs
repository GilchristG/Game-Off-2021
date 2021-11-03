using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Move",menuName ="Character Move",order = 0)]
public class CharacterMove : ScriptableObject
{
    public string moveAnimation;
    public int damage;

    //For simplicity sake, we can just have this set to the animation and not the other way around. 
    //Use this number inside the player controller to determine if the attack has ended.
    public int moveLength;

    //public Input for move


}
