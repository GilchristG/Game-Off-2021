using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHealthListener : MonoBehaviour
{
    public BasicMoveTester playerToCheck;
    public int player = 1;
    [SerializeField] public int totalHealth = 100;
    [SerializeField] TextMeshProUGUI healthText;

    private void Awake()
    {
        healthText = GetComponent<TextMeshProUGUI>();
    }

    public void AssignPlayer(BasicMoveTester player)
    {
        playerToCheck = player;

        if (playerToCheck != null)
            playerToCheck.onPlayerHealthLoss += UpdateHealth;
    }

    public void ResetHealth()
    {
        totalHealth = 100;
        healthText?.SetText(totalHealth.ToString());
    }

    public void UpdateHealth(int difference)
    {
        totalHealth -= difference;
        totalHealth = Mathf.Clamp(totalHealth, 0, 100);
        healthText?.SetText(totalHealth.ToString());
    }
}
