using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramManager : MonoBehaviour
{
    public GameObject loadScreenCanvas;
    public GameObject loadScreen_pf;

    public EnumCharacter p1Character;
    public EnumCharacter p2Character;


    public void SelectP1Character(EnumCharacter character)
    {
        p1Character = character;
    }

    public void SelectP2Character(EnumCharacter character)
    {
        p2Character = character;
    }

    /*
    public void LoadLocalMatch()
    {
        //Spawn in loadscreen
        //Deload menus
        //Load new scene
        //InitializeMatch
        FindObjectOfType<OfflineBBGame>().InitializeMatch(p1Character, p2Character);
        //Remove loadscreen
        //Start music, intro animations, etc
        //Start match
        //Wait for match end or quit to menu
    }

    */
}
