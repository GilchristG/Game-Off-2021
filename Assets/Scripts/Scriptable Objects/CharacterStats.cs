using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character Stats", menuName = "Character Stats", order = 3)]
public class CharacterStats : ScriptableObject
{
    public MoveSet moveSet;
    public float jumpForce;
    public float speed;
    public float maxSpeed;
    public float gravityScale;
}
