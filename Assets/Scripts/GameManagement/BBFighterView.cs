using System.Collections;
using System.Collections.Generic;
using UnityGGPO;
using UnityEngine;
using UnityEngine.UI;
using SharedGame;

public class BBFighterView : MonoBehaviour
{
    public Text txtStatus;
    public Image imgProgress;
    public Animator anim;
    public int health;
    public string move;
    public int moveFrame;

    [SerializeField] FighterState state;
    [SerializeField] FighterStance stance;

    //public Transform model;
    //Add reference to fighter here

    Fighter fighter;

    public void Populate(Fighter fighterG, PlayerConnectionInfo info)
    {
        fighter = fighterG;
        transform.position = fighterG.position;
        health = fighterG.health;
        state = fighterG.currentState;
        stance = fighterG.currentStance;
        move = fighterG.currentMove?.animationName;
        moveFrame = fighterG.elapsedMoveTime;

        if (fighterG.facingRight)
        {
            transform.localScale = new Vector3(5f, 5f, 1f);
        }
        else
        {
            transform.localScale = new Vector3(-5f, 5f, 1f);
        }

        //Put fighter reference animation stuff here it seems
        //Might have to put the fighter logic within the game code and just have the player update animation on a PER fighter basis
        //model.rotation = Quaternion.Euler(0, 0, fighterG.heading);

        string status = "";
        int progress = -1;
        switch (info.state)
        {
            case PlayerConnectState.Connecting:
                status = (info.type == GGPOPlayerType.GGPO_PLAYERTYPE_LOCAL) ? "Local Player" : "Connecting...";
                break;

            case PlayerConnectState.Synchronizing:
                progress = info.connect_progress;
                status = (info.type == GGPOPlayerType.GGPO_PLAYERTYPE_LOCAL) ? "Local Player" : "Synchronizing...";
                break;

            case PlayerConnectState.Disconnected:
                status = "Disconnected";
                break;

            case PlayerConnectState.Disconnecting:
                status = "Waiting for player...";
                progress = (Utils.TimeGetTime() - info.disconnect_start) * 100 / info.disconnect_timeout;
                break;
        }

        if (progress > 0)
        {
            imgProgress.gameObject.SetActive(true);
            imgProgress.fillAmount = progress / 100f;
        }
        else
        {
            imgProgress.gameObject.SetActive(false);
        }

        if (status.Length > 0)
        {
            txtStatus.gameObject.SetActive(true);
            txtStatus.text = status;
        }
        else
        {
            txtStatus.gameObject.SetActive(false);
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            fighter.currentStance = FighterStance.Standing;
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {

    }
}
