using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProgramManager : MonoBehaviour
{
    public GameObject loadScreenCanvas;
    public GameObject loadScreen_pf;

    public EnumCharacter p1Character;
    public EnumCharacter p2Character;

    void Awake()
    {
        if(SceneManager.sceneCount == 1)
        {
            SceneManager.LoadScene("MainMenu");
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
        StartCoroutine(LoadLocalVersus());
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
    }
}
