using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    public Transform player1Spawn;
    public Transform player2Spawn;

    public GameObject bombardier;
    public GameObject hercules;

    private CharacterSelection characterSelection; 

    // Start is called before the first frame update
    void Start()
    {
        characterSelection = FindObjectOfType<CharacterSelection>();
        characterSelection.gameHasStarted = true;
        characterSelection.disableControls();

        spawnPlayers();
    }

    private void spawnPlayers()
    {
        GameObject player1 = null;
        GameObject player2 = null;

        switch(characterSelection.currentCharacter1)
        {
            case Character.BOMBARDIER:
                player1 = Instantiate(bombardier, player1Spawn.position, player1Spawn.rotation);
                break;
            case Character.HERCULES:
                player1 = Instantiate(hercules, player1Spawn.position, player1Spawn.rotation);
                break;
        }
        switch (characterSelection.currentCharacter2)
        {
            case Character.BOMBARDIER:
                player2 = Instantiate(bombardier, player2Spawn.position, player2Spawn.rotation);
                break;
            case Character.HERCULES:
                player2 = Instantiate(hercules, player2Spawn.position, player2Spawn.rotation);
                break;
        }

        if (player1 != null)
            player1.GetComponent<MyCharacterController>().setMirrored(false);
        if (player2 != null)
            player2.GetComponent<MyCharacterController>().setMirrored(true);
    }
}
