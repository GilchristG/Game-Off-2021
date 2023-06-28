using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtboxManager : MonoBehaviour
{
    Move currentMove;
    bool hurtBoxActive = false;

    //This doesn't take into account multi hit moves
    bool alreadyHit = false;
    HitboxManager ourHitbox;

    public void SetSelfHitbox(HitboxManager hbm)
    {
        ourHitbox = hbm;
    }

    public void ActivateHurtbox(Move activeMove)
    {
        currentMove = activeMove;
        hurtBoxActive = true;
        alreadyHit = false;
    }

    public void DeactivateHurtbox()
    {
        hurtBoxActive = false;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hurtBoxActive || alreadyHit)
            return;

        if(collision.TryGetComponent<HitboxManager>(out HitboxManager hitbox) && hitbox != ourHitbox)
        {
            alreadyHit = true;
            hitbox.ProcessHit(currentMove,collision.ClosestPoint(transform.position));
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (!hurtBoxActive || alreadyHit)
            return;

        if (collision.TryGetComponent<HitboxManager>(out HitboxManager hitbox) && hitbox != ourHitbox)
        {
            alreadyHit = true;
            hitbox.ProcessHit(currentMove, collision.ClosestPoint(transform.position));
        }
    }
}
