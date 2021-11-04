using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHealth : MonoBehaviour
{
    public uint characterNumber;
    public float currentHealth;

    private float maxHealth;

    private HealthManager healthManager;

    // Start is called before the first frame update
    void Start()
    {
        healthManager = FindObjectOfType<HealthManager>();

        maxHealth = Constants.MAX_HEALTH;
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void takeDamage(float dmg)
    {
        currentHealth -= dmg;
        healthManager.updateHealthBars(characterNumber, currentHealth);
    }
}
