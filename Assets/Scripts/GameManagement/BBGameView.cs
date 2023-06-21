using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SharedGame;
using System;

public class BBGameView : MonoBehaviour, IGameView
{
    public BBFighterView fighterPrefab;

    private BBFighterView[] fighterViews = Array.Empty<BBFighterView>();
    //private Transform[][] bulletLists;
    private GameManager gameManager => GameManager.Instance;

    private void ResetView(BBGame gs)
    {
        var shipGss = gs._fighters;
        fighterViews = new BBFighterView[shipGss.Length];
        //bulletLists = new Transform[shipGss.Length][];

        for (int i = 0; i < shipGss.Length; ++i)
        {
            fighterViews[i] = Instantiate(fighterPrefab, transform);
            //bulletLists[i] = new Transform[shipGss[i].bullets.Length];
            /*for (int j = 0; j < bulletLists[i].Length; ++j)
            {
                bulletLists[i][j] = Instantiate(bulletPrefab, transform);
            }*/
        }
    }

    public void UpdateGameView(IGameRunner runner)
    {
        var gs = (BBGame)runner.Game;
        var ngs = runner.GameInfo;

        var fighterGss = gs._fighters;
        if (fighterViews.Length != fighterGss.Length)
        {
            ResetView(gs);
        }

        for (int i = 0; i < fighterGss.Length; ++i)
        {
            fighterViews[i].Populate(fighterGss[i], ngs.players[i]);
            //UpdateBullets(fighterGss[i].bullets, bulletLists[i]);
        }

    }

    /*private void UpdateBullets(Bullet[] bullets, Transform[] bulletList)
    {
        for (int j = 0; j < bulletList.Length; ++j)
        {
            bulletList[j].position = bullets[j].position;
            if (bullets[j].velocity.sqrMagnitude > Mathf.Epsilon)
            {
                bulletList[j].rotation = Quaternion.LookRotation(bullets[j].velocity, Vector3.up);
            }
            bulletList[j].gameObject.SetActive(bullets[j].active);
        }
    }*/


    private void Update()
    {
        if (gameManager.IsRunning)
        {
            UpdateGameView(gameManager.Runner);
        }
    }
}
