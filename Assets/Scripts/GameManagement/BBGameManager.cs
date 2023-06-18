using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SharedGame;
using UnityGGPO;

public class BBGameManager : GameManager
{

    public override void StartLocalGame()
    {
        StartGame(new LocalRunner(new BBGame(2)));
    }

    public override void StartGGPOGame(IPerfUpdate perfPanel, IList<Connections> connections, int playerIndex)
    {
        var game = new GGPORunner("bugbattle", new BBGame(connections.Count), perfPanel);
        game.Init(connections,playerIndex);
        StartGame(game);
    }

}
