using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHealthListener : MonoBehaviour
{
    public BasicMoveTester playerToCheck;
    [SerializeField] public int totalHealth = 100;
    [SerializeField] TextMeshProUGUI healthText;

    private void Awake()
    {
        healthText = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        if(playerToCheck != null)
            playerToCheck.onPlayerHealthLoss += UpdateHealth;
    }

    public void UpdateHealth(int difference)
    {
        totalHealth -= difference;
        healthText?.SetText(totalHealth.ToString());
    }
}
