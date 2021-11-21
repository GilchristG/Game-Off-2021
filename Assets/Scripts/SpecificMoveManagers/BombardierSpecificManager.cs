using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombardierSpecificManager : SpecificMoveManager
{
    public GameObject boom;

    public float boostForce;
    public float boostAngle;
    public float slamDelay;
    public float slamForce;

    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponentInParent<Rigidbody2D>();
        moveController = GetComponentInParent<MyCharacterController>();
    }

    public override void DoCoolStuff(int i)
    {
        switch(i)
        {
            case 1:
                StartCoroutine(boostSmash());
                break;
        }
    }

    IEnumerator boostSmash()
    {
        Instantiate(boom, transform.position, transform.rotation);

        float xcomponent = Mathf.Cos(boostAngle * Mathf.PI / 180) * boostForce;
        float ycomponent = Mathf.Sin(boostAngle * Mathf.PI / 180) * boostForce;

        if (moveController.mirrored)
            rb.AddForce(new Vector2(-xcomponent, ycomponent), ForceMode2D.Impulse);
        else
            rb.AddForce(new Vector2(xcomponent, ycomponent), ForceMode2D.Impulse);
        rb.gravityScale = 1f;

        yield return new WaitForSeconds(slamDelay);

        rb.gravityScale = moveController.characterStats.gravityScale;

        xcomponent = Mathf.Cos(-boostAngle * Mathf.PI / 180) * slamForce;
        ycomponent = Mathf.Sin(-boostAngle * Mathf.PI / 180) * slamForce;

        if (moveController.mirrored)
            rb.AddForce(new Vector2(-xcomponent, ycomponent), ForceMode2D.Impulse);
        else
            rb.AddForce(new Vector2(xcomponent, ycomponent), ForceMode2D.Impulse);

        yield return 0;
    }
}
