using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInitializer : MonoBehaviour
{
    public Transform player1Spawn;
    public Transform player2Spawn;

    public GameObject bombardier;
    public GameObject hercules;

    private CharacterSelection characterSelection;
    private PlayerInputManager playerInputManager;

    // Start is called before the first frame update
    void Start()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
        characterSelection = FindObjectOfType<CharacterSelection>();
        characterSelection.gameHasStarted = true;
        characterSelection.disableControls();

        spawnPlayers();
    }

    private void spawnPlayers()
    {
        PlayerInput player1 = null;
        PlayerInput player2 = null;

        switch(characterSelection.currentCharacter1)
        {
            case Character.BOMBARDIER:
                playerInputManager.playerPrefab = bombardier;
                player1 = PlayerInput.Instantiate(bombardier, controlScheme: characterSelection.player1Controls);
                break;
            case Character.HERCULES:
                playerInputManager.playerPrefab = hercules;
                player1 = PlayerInput.Instantiate(hercules, controlScheme: characterSelection.player1Controls);
                break;
        }
        switch (characterSelection.currentCharacter2)
        {
            case Character.BOMBARDIER:
                playerInputManager.playerPrefab = bombardier;
                player2 = PlayerInput.Instantiate(bombardier, controlScheme: characterSelection.player2Controls);
                break;
            case Character.HERCULES:
                playerInputManager.playerPrefab = hercules;
                player2 = PlayerInput.Instantiate(hercules, controlScheme: characterSelection.player2Controls);
                break;
        }

        if (player1 != null)
            player1.gameObject.GetComponent<MyCharacterController>().setMirrored(false);
        if (player2 != null)
            player2.gameObject.GetComponent<MyCharacterController>().setMirrored(true);

        player1.transform.position = player1Spawn.position;
        player2.transform.position = player2Spawn.position;
    }
}
