using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class ProgramManager : MonoBehaviour
{
    public GameObject loadScreenCanvas;
    public GameObject loadScreen_pf;

    public EnumCharacter p1Character;
    public EnumCharacter p2Character;

    bool isLoading = false;

    public enum GameState
    {
        Menu,
        Loading,
        InGame
    }

    private GameState _GameState;

    void Awake()
    {


        if(SceneManager.sceneCount == 1)
        {
            SceneManager.LoadScene("MainMenu");
            _GameState = GameState.Menu;
        }
    }

    public void SelectP1Character(EnumCharacter character)
    {
        p1Character = character;
    }

    public void SelectP2Character(EnumCharacter character)
    {
        p2Character = character;
    }

    
    public void LoadLocalMatch()
    {
        if (!isLoading)
        {
            isLoading = true;
            StartCoroutine(LoadLocalVersus());
        }
    }

    IEnumerator LoadLocalVersus()
    {
        //Spawn in loadscreen
        var loadScreenInstance = Instantiate(loadScreen_pf, loadScreenCanvas.transform);
        loadScreenInstance.GetComponent<Animator>().SetTrigger("Start");

        yield return new WaitForSeconds(1f);

        //Deload menus
        AsyncOperation deloadMenu = SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("MainMenu"));
        yield return new WaitUntil(() => deloadMenu.isDone == true );

        //Load new scene
        AsyncOperation loadStage = SceneManager.LoadSceneAsync("OfflineVersus",LoadSceneMode.Additive);

        yield return new WaitUntil(() => loadStage.isDone == true);

        yield return new WaitForSeconds(1f);

        //InitializeMatch
        FindObjectOfType<OfflineBBGame>().InitializeMatch(p1Character, p2Character);

        //Remove loadscreen
        loadScreenInstance.GetComponent<Animator>().SetTrigger("Finish");

        //Start music, intro animations, etc
        //Start match
        //Wait for match end or quit to menu

        isLoading = false;
    }


    //Input handling

    public PlayerInput p1Input;
    public PlayerInput p2Input;

    Action<PlayerInput> onPlayer1Joined;
    Action onPlayer1Left;
    Action<PlayerInput> onPlayer2Joined;
    Action onPlayer2Left;

    public void OnPlayerJoined(PlayerInput newPlayer)
    {
        if(p1Input == null)
        {
            p1Input = newPlayer;
            onPlayer1Joined?.Invoke(p1Input);
        }
        else if(p2Input == null)
        {
            p2Input = newPlayer;
            onPlayer2Joined?.Invoke(p2Input);
        }
    }

    public void OnPlayerLeft(PlayerInput leftPlayer)
    {
        if (p1Input == leftPlayer)
        {
            onPlayer1Left?.Invoke();
            p1Input = null;
        }
        else if (p2Input == leftPlayer)
        {
            onPlayer2Left?.Invoke();
            p2Input = null;
        }
    }
}
