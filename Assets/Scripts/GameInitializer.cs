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
        switch(characterSelection.currentCharacter1)
        {
            case Character.BOMBARDIER:
                Instantiate(bombardier, player1Spawn.position, player1Spawn.rotation);
                break;
            case Character.HERCULES:
                Instantiate(hercules, player1Spawn.position, player1Spawn.rotation);
                break;
        }
        switch (characterSelection.currentCharacter2)
        {
            case Character.BOMBARDIER:
                Instantiate(bombardier, player2Spawn.position, player2Spawn.rotation);
                break;
            case Character.HERCULES:
                Instantiate(hercules, player2Spawn.position, player2Spawn.rotation);
                break;
        }
    }
}
