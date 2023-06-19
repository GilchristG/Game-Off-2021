using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMoveSet",menuName = "Character Assets/Move Set", order = 1)]
public class MoveSet : ScriptableObject
{
    public MoveData[] moves;
}
