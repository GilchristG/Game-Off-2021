using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterHealth : MonoBehaviour
{
    public uint characterNumber;
    public float currentHealth;

    private float maxHealth;

    private HealthManager healthManager;
    private MyCharacterController moveController;

    public UnityEvent onDamageTaken;

    // Start is called before the first frame update
    void Start()
    {
        healthManager = FindObjectOfType<HealthManager>();
        moveController = GetComponentInParent<MyCharacterController>();

        maxHealth = Constants.MAX_HEALTH;
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void takeDamage(float dmg, Move moveThatHitMe)
    {
        currentHealth -= dmg;

        if (currentHealth <= 0f)
        {
            Destroy(gameObject);
        }
        
        healthManager.updateHealthBars(characterNumber, currentHealth);

        StartCoroutine(hitStun(moveThatHitMe));
    }

    //FOR TEST ONLY
    public void takeDamageButton(float dmg)
    {
        currentHealth -= dmg;
        healthManager.updateHealthBars(characterNumber, currentHealth);
    }

    private IEnumerator hitStun(Move moveThatHitMe)
    {
        if (moveThatHitMe.hitStunTime <= 0f && moveThatHitMe.blockStunTime <= 0f)
        {
            yield return 0;
        }

        moveController.isHitStunned = true;

        if (moveController.isBlocking)
            yield return new WaitForSeconds(moveThatHitMe.blockStunTime);
        else
            yield return new WaitForSeconds(moveThatHitMe.hitStunTime);

        moveController.isHitStunned = false;

        yield return 0;
    }
}
