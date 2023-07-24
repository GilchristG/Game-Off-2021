using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class OfflineBBGame : MonoBehaviour
{
    [SerializeField] MatchSettings matchSettings = new MatchSettings();

    [SerializeField] int p1Rounds = 0;
    [SerializeField] int p2Rounds = 0;

    public BasicMoveTester fighter1;
    public BasicMoveTester fighter2;
    public CinemachineTargetGroup cameraTargetGroup;
    public GameModeCharacterList characterList;

    public PlayerHealthListener p1HealthUI;
    public PlayerHealthListener p2HealthUI;

    Transform p1Spawn;
    Transform p2Spawn;

    GameObject p1Health_UI;
    GameObject p2Health_UI;

    GameObject p1FrameData_UI;
    GameObject p2FrameData_UI;

    public Transform heightTarget;

    public double frameDuration = 0.016f;
    public double nextFrameTime = 0;

    Vector3 moveDirection_P1 = new Vector3(0, 0, 0);
    int[] attackButtons_P1 = new int[4];

    Vector3 moveDirection_P2 = new Vector3(0, 0, 0);
    int[] attackButtons_P2 = new int[4];

    [SerializeField] bool matchRunning = false;

    bool p1Disconnect = true;
    bool p2Disconnect = true;

    [SerializeField] ProgramManager manager;

    InputAction p1Walk;
    InputAction p1Light;
    InputAction p1Medium;
    InputAction p1Heavy;
    InputAction p1Special;

    InputAction p2Walk;
    InputAction p2Light;
    InputAction p2Medium;
    InputAction p2Heavy;
    InputAction p2Special;

    private void OnEnable()
    {
        manager = FindObjectOfType<ProgramManager>();

        manager.onPlayerJoined += OnPlayerJoin;
        manager.onPlayerLeft += OnPlayerLeft;

        if (manager.p1Input != null)
        {
            manager.p1Input.SwitchCurrentActionMap("Fight");
            p1Walk = manager.p1Input.actions["Walk"];
            p1Light = manager.p1Input.actions["LightAttack"];
            p1Medium = manager.p1Input.actions["MidAttack"];
            p1Heavy = manager.p1Input.actions["HeavyAttack"];
            p1Special = manager.p1Input.actions["Special"];
            p1Disconnect = false;
        }

        if (manager.p2Input != null)
        {
            manager.p2Input.SwitchCurrentActionMap("Fight");
            p2Walk = manager.p2Input.actions["Walk"];
            p2Light = manager.p2Input.actions["LightAttack"];
            p2Medium = manager.p2Input.actions["MidAttack"];
            p2Heavy = manager.p2Input.actions["HeavyAttack"];
            p2Special = manager.p2Input.actions["Special"];
            p2Disconnect = false;
        }

    }

    private void OnDisable()
    {
        manager.onPlayerJoined -= OnPlayerJoin;
        manager.onPlayerLeft -= OnPlayerLeft;
    }

    public void OnPlayerLeft(PlayerInput player)
    {
        //Can potentially add a pause here to let player reconnect their controller

        if(player.playerIndex == 0)
        {
            p1Disconnect = true;
        }
        else if(player.playerIndex == 1)
        {
            p2Disconnect = true;
        }
    }

    public void OnPlayerJoin(PlayerInput player)
    {
        if(player.playerIndex == 0)
        {
            manager.p1Input.SwitchCurrentActionMap("Fight");
            p1Walk = manager.p1Input.actions["Walk"];
            p1Light = manager.p1Input.actions["LightAttack"];
            p1Medium = manager.p1Input.actions["MidAttack"];
            p1Heavy = manager.p1Input.actions["HeavyAttack"];
            p1Special = manager.p1Input.actions["Special"];

            p1Disconnect = false;
        }
        else if(player.playerIndex == 1)
        {
            manager.p2Input.SwitchCurrentActionMap("Fight");
            p2Walk = manager.p2Input.actions["Walk"];
            p2Light = manager.p2Input.actions["LightAttack"];
            p2Medium = manager.p2Input.actions["MidAttack"];
            p2Heavy = manager.p2Input.actions["HeavyAttack"];
            p2Special = manager.p2Input.actions["Special"];

            p2Disconnect = false;
        }
    }

    public void InitializeCamera()
    {
        cameraTargetGroup = FindObjectOfType<CinemachineTargetGroup>();
        Array.Clear(cameraTargetGroup.m_Targets, 0, cameraTargetGroup.m_Targets.Length);

        if (cameraTargetGroup != null)
        {
            cameraTargetGroup.AddMember(heightTarget, 1f, 0);
            cameraTargetGroup.AddMember(fighter1.transform, 1f, 0);
            cameraTargetGroup.AddMember(fighter2.transform, 1f, 0);
        }
    }

    public bool InitializeMatch(EnumCharacter p1, EnumCharacter p2)
    {
        p1Spawn = GameObject.FindGameObjectWithTag("P1Spawn").transform;
        p2Spawn = GameObject.FindGameObjectWithTag("P2Spawn").transform;

        foreach (CharacterPref cPref in characterList.charactersForMode)
        {
            if(p1 == cPref.characterType)
            {
                fighter1 = Instantiate(cPref.character_pf, p1Spawn.position, p1Spawn.rotation).GetComponent<BasicMoveTester>();
            }
            if(p2 == cPref.characterType)
            {
                fighter2 = Instantiate(cPref.character_pf, p2Spawn.position, p2Spawn.rotation).GetComponent<BasicMoveTester>();
            }

            if(fighter1 != null && fighter2 != null)
            {
                break;
            }
        }

        fighter1.GetComponent<BufferVisualizer>().SetupBV(GameObject.FindGameObjectWithTag("P1Buffer"));
        fighter2.GetComponent<BufferVisualizer>().SetupBV(GameObject.FindGameObjectWithTag("P2Buffer"));

        /*var healthBars = FindObjectsOfType<PlayerHealthListener>();

        foreach(PlayerHealthListener phl in healthBars)
        {
            if(phl.player == 1)
            {
                phl.AssignPlayer(fighter1);
            }
            else if(phl.player == 2)
            {
                phl.AssignPlayer(fighter2);
            }
        }*/

        p1HealthUI.ResetHealth();
        p2HealthUI.ResetHealth();

        fighter1.onPlayerHealthLoss += OnPlayer1HealthChange;
        fighter1.onPlayerHealthLoss += p1HealthUI.UpdateHealth;
        fighter2.onPlayerHealthLoss += OnPlayer2HealthChange;
        fighter2.onPlayerHealthLoss += p2HealthUI.UpdateHealth;

        fighter1.opponentTransform = fighter2.transform;
        fighter2.opponentTransform = fighter1.transform;


        InitializeCamera();


        //Can add a delay here
        matchRunning = true;

        return true;
    }


    private void Update()
    {
        if (!matchRunning)
            return;

        InputFrame inputFrame = new InputFrame();

        moveDirection_P1 = new Vector3(0, 0, 0);

        if (!p1Disconnect)
        {

            Vector2 p1Dir = p1Walk.ReadValue<Vector2>();

            if (p1Dir != null)
            {
                moveDirection_P1 += new Vector3(Mathf.Clamp(p1Dir.x*10,-1f,1f), Mathf.Clamp(p1Dir.y*10, -1f, 1f), 0);
            }

            if (p1Light.WasPerformedThisFrame())
            {
                attackButtons_P1[0] = 1;
            }

            if (p1Medium.WasPerformedThisFrame())
            {
                attackButtons_P1[1] = 1;
            }

            if (p1Heavy.WasPerformedThisFrame())
            {
                attackButtons_P1[2] = 1;
            }

            if (p1Special.WasPerformedThisFrame())
            {
                attackButtons_P1[3] = 1;
            }
        }

        moveDirection_P2 = new Vector3(0, 0, 0);

        if (!p2Disconnect)
        {

            Vector2 p2Dir = p2Walk.ReadValue<Vector2>();

            if (p2Dir != null)
                moveDirection_P2 += new Vector3(Mathf.Clamp(p2Dir.x * 10, -1f, 1f), Mathf.Clamp(p2Dir.y * 10, -1f, 1f), 0);

            if (p2Light.WasPerformedThisFrame())
            {
                attackButtons_P2[0] = 1;
            }

            if (p2Medium.WasPerformedThisFrame())
            {
                attackButtons_P2[1] = 1;
            }

            if (p2Heavy.WasPerformedThisFrame())
            {
                attackButtons_P2[2] = 1;
            }

            if (p2Special.WasPerformedThisFrame())
            {
                attackButtons_P2[3] = 1;
            }
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


    public void OnPlayer1HealthChange(int healthChange)
    {
        if(fighter1.playerHealth <= 0)
        {
            WinRound(2);
        }
    }

    public void OnPlayer2HealthChange(int healthChange)
    {
        if (fighter2.playerHealth <= 0)
        {
            WinRound(1);
        }
    }

    
    public void WinRound(int playerNumber)
    {
        switch(playerNumber)
        {
            case 1:
                p1Rounds++;
                if(p1Rounds >= matchSettings.roundsWinsPerSet)
                {
                    StartCoroutine(EndMatchAndGoToTitle(playerNumber));
                    return;
                }
                break;
            case 2:
                p2Rounds++;
                if (p2Rounds >= matchSettings.roundsWinsPerSet)
                {
                    StartCoroutine(EndMatchAndGoToTitle(playerNumber));
                    return;
                }
                break;
        }

        StartCoroutine(EndOfRoundAndRest(playerNumber));
    }

    IEnumerator EndOfRoundAndRest(int playerRoundWinner)
    {
        matchRunning = false;

        //Queue any victory animations and text

        yield return new WaitForSeconds(3f);


        yield return new WaitForFixedUpdate();


        //AddQuickLoadingScreen;

        manager.StartLoadScreen();

        yield return new WaitForSeconds(1f);

        fighter1.onPlayerHealthLoss -= OnPlayer1HealthChange;
        fighter1.onPlayerHealthLoss -= p1HealthUI.UpdateHealth;
        fighter2.onPlayerHealthLoss -= OnPlayer2HealthChange;
        fighter2.onPlayerHealthLoss -= p2HealthUI.UpdateHealth;

        Destroy(fighter1.gameObject);
        Destroy(fighter2.gameObject);

        yield return new WaitForSeconds(0.5f);

        yield return new WaitUntil(() => InitializeMatch(manager.p1Character, manager.p2Character));

        manager.EndLoadScreen();

        yield return new WaitForSeconds(1f);

        matchRunning = true;
    }

    IEnumerator EndMatchAndGoToTitle(int playermatchWinner)
    {
        matchRunning = false;

        //Show text and give a time out for back to main menu


        yield return new WaitForFixedUpdate();
    }
}
