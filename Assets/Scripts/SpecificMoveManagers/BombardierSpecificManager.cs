using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombardierSpecificManager : SpecificMoveManager
{
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
                boostSmash();
                break;
        }
    }

    IEnumerator boostSmash()
    {
        float xcomponent = Mathf.Cos(boostAngle * Mathf.PI / 180) * boostForce;
        float ycomponent = Mathf.Sin(boostAngle * Mathf.PI / 180) * boostForce;

        rb.AddForce(new Vector2(xcomponent, ycomponent), ForceMode2D.Impulse);

        yield return new WaitForSeconds(slamDelay);

        xcomponent = Mathf.Cos(-boostAngle * Mathf.PI / 180) * slamForce;
        ycomponent = Mathf.Sin(-boostAngle * Mathf.PI / 180) * slamForce;

        rb.AddForce(new Vector2(xcomponent, ycomponent), ForceMode2D.Impulse);

        yield return 0;
    }
}
