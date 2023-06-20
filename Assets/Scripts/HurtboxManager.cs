using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtboxManager : MonoBehaviour
{
    Move currentMove;
    bool hurtBoxActive = false;
    HitboxManager ourHitbox;

    public void SetSelfHitbox(HitboxManager hbm)
    {
        ourHitbox = hbm;
    }

    public void ActivateHurtbox(Move activeMove)
    {
        currentMove = activeMove;
        hurtBoxActive = true;
    }

    public void DeactivateHurtbox()
    {
        hurtBoxActive = false;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hurtBoxActive)
            return;

        if(collision.TryGetComponent<HitboxManager>(out HitboxManager hitbox) && hitbox != ourHitbox)
        {
            hitbox.ProcessHit(currentMove,collision.ClosestPoint(transform.position));
        }
    }
}
