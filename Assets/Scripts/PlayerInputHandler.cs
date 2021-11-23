using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInput playerInput;
    private MyCharacterController characterController;

    // Start is called before the first frame update
    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        var controllers = FindObjectsOfType<MyCharacterController>();
        int index = playerInput.playerIndex;
        characterController = controllers.FirstOrDefault(c => c.playerIndex == index);
    }
}
