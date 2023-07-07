using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput_Instance : MonoBehaviour
{
    [SerializeField] int player = 0;

    void Awake()
    {
        var otherPlayer = FindObjectOfType<PlayerInput_Instance>();

        if (otherPlayer.player == 1)
        {

        }
    }



}
