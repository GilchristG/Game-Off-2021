using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveData : ScriptableObject
{
    float windupTime;
    float activeTime;
    float recoveryTimeHit;
    float recoveryTimeMiss;
    float recoveryTimeBlocked;
    float hitStunTime;
    float blockStunTime;
    Vector2 knockbackDir;
    float knockbackForce;
    float damage;
}
