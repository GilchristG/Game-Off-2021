using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HerculesSpecificManager : SpecificMoveManager
{
    public float chargeSpeed;
    public Animator herculesAnimator;
    public float chargeLength = 2f;

    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponentInParent<Rigidbody2D>();
        moveController = GetComponentInParent<MyCharacterController>();
    }

    public override void DoCoolStuff(int i)
    {
        switch (i)
        {
            case 1:
                StartCoroutine(Charge());
                break;
        }
    }

    IEnumerator Charge()
    {
        herculesAnimator.SetBool("Charging", true);

        float timer = 0f;

        while(timer < chargeLength)
        {
            timer += Time.deltaTime;

            if (moveController.mirrored)
                rb.AddForce(new Vector2(-1,0)*chargeSpeed/*Time.deltaTime*/, ForceMode2D.Force);
            else
                rb.AddForce(new Vector2(1, 0) * chargeSpeed/*Time.deltaTime*/, ForceMode2D.Force);

            yield return new WaitForEndOfFrame();
        }


        herculesAnimator.SetBool("Charging", false);
    }
}
