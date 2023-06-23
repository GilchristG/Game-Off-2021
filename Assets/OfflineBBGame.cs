using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class OfflineBBGame : MonoBehaviour
{
    public BasicMoveTester fighter1;
    public BasicMoveTester fighter2;
    public CinemachineTargetGroup cameraTargetGroup;

    public double frameDuration = 0.016f;
    public double nextFrameTime = 0;

    Vector3 moveDirection_P1 = new Vector3(0, 0, 0);
    int[] attackButtons_P1 = new int[4];

    Vector3 moveDirection_P2 = new Vector3(0, 0, 0);
    int[] attackButtons_P2 = new int[4];

    private void Awake()
    {
        cameraTargetGroup = FindObjectOfType<CinemachineTargetGroup>();
        if (cameraTargetGroup != null)
        {
            cameraTargetGroup.AddMember(fighter1.transform, 1f, 0);
            cameraTargetGroup.AddMember(fighter2.transform, 1f, 0);
        }

        if (fighter1 != null && fighter2 != null)
        {
            fighter1.opponentTransform = fighter2.transform;
            fighter2.opponentTransform = fighter1.transform;
        }
    }

    private void Update()
    {
        InputFrame inputFrame = new InputFrame();

        moveDirection_P1 = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.A))
        {
            moveDirection_P1 = new Vector3(-1, 0, 0);
        }

        if (Input.GetKey(KeyCode.D))
        {
            moveDirection_P1 = new Vector3(1, 0, 0);
        }

        if (Input.GetKey(KeyCode.W))
        {
            moveDirection_P1 += new Vector3(0, 1, 0);
        }

        if (Input.GetKey(KeyCode.S))
        {
            moveDirection_P1 += new Vector3(0, -1, 0);
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            attackButtons_P1[0] = 1;
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            attackButtons_P1[1] = 1;
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            attackButtons_P1[2] = 1;
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            attackButtons_P1[3] = 1;
        }


        moveDirection_P2 = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveDirection_P2 = new Vector3(-1, 0, 0);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            moveDirection_P2 = new Vector3(1, 0, 0);
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            moveDirection_P2 += new Vector3(0, 1, 0);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            moveDirection_P2 += new Vector3(0, -1, 0);
        }

        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            attackButtons_P2[0] = 1;
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            attackButtons_P2[1] = 1;
        }

        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            attackButtons_P2[2] = 1;
        }

        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            attackButtons_P2[3] = 1;
        }


        if (nextFrameTime < Time.time)
        {
            fighter1?.FrameUpdate(moveDirection_P1, attackButtons_P1);
            fighter2?.FrameUpdate(moveDirection_P2,attackButtons_P2);

            nextFrameTime = Time.time + frameDuration;

            moveDirection_P1 = new Vector3(0, 0, 0);
            attackButtons_P1 = new int[4];
            moveDirection_P2 = new Vector3(0, 0, 0);
            attackButtons_P2 = new int[4];
        }
    }

}
