using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterSelectorManager : MonoBehaviour
{
    public CharacterGridInfo gridInfo;

    public GameObject p1CharacterAvatar;
    public GameObject p2CharacterAvatar;

    [SerializeField] CharacterPanel p1Selection;
    [SerializeField] int p1Element = 0;
    bool p1Selected = false;
    [SerializeField] CharacterPanel p2Selection;
    int p2Element = 1;
    bool p2Selected = false;

    public GameObject p1PointerGraphic;
    public GameObject p2PointerGraphic;

    ProgramManager programManager;
    ModeLauncher modeLauncher;
    MainMenuManager mainMenu;

    private void OnEnable()
    {
        programManager = FindObjectOfType<ProgramManager>();
        mainMenu = FindObjectOfType<MainMenuManager>();

        modeLauncher = GetComponent<ModeLauncher>();
    }

    public void Process(int player, Vector2Int direction)
    {
        if (player == 1 && p1Selected)
            return;
        if (player == 2 && p2Selected)
            return;

        if (direction.magnitude == 0)
            return;

        switch (player)
        {
            case 1:
                p1Selection = gridInfo.GetPanelAt(direction, ref p1Element);
                p1PointerGraphic.transform.parent = p1Selection.transform;
                p1PointerGraphic.transform.SetPositionAndRotation(p1Selection.transform.position, p1Selection.transform.rotation);
                break;
            case 2:
                p2Selection = gridInfo.GetPanelAt(direction, ref p2Element);
                p2PointerGraphic.transform.parent = p2Selection.transform;
                p2PointerGraphic.transform.SetPositionAndRotation(p2Selection.transform.position, p2Selection.transform.rotation);
                break;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        int player = 1;

        Vector2 newInput = context.ReadValue<Vector2>();

        Vector2Int newDirection = new Vector2Int((int)newInput.x, (int)newInput.y); //new Vector2Int(Mathf.RoundToInt(newInput.x), Mathf.RoundToInt(newInput.y));
        //Get player input and send to process

        Process(player, newDirection);
    }

    public void OnConfirm(InputAction.CallbackContext context)
    {
        int player = 1;

        //Testing that auto confirms p2 character due to holding down a button sends multiple commands
        //if (p1Selected)
            //player = 2;


        //Check which player did it
        switch (player)
        {
            case 1:
                if (!p1Selected)
                {
                    programManager.SelectP1Character(p1Selection.panelCharacter);
                    p1Selected = true;
                }
                break;
            case 2:
                if (!p2Selected)
                {
                    programManager.SelectP2Character(p2Selection.panelCharacter);
                    p2Selected = true;
                }
                break;
        }

        if(p1Selected && p2Selected)
        {
            //Attempt to start match
            modeLauncher.LaunchMode();
        }
    }

    public void OnBack(InputAction.CallbackContext context)
    {
        int player = 1;

        switch (player)
        {
            case 1:
                if (p1Selected)
                {
                    p1Selected = false;
                }
                else
                {
                    //Go back a menu or selection
                    mainMenu.ToMainMenu((int)MainMenuManager.MenuScreens.LocalPlay);
                }
                break;
            case 2:
                if (p2Selected)
                {
                    p2Selected = false;
                }
                else
                {
                    //Go back a menu or selection OR just prevent p2 from doing anything
                }
                break;
        }


    }
}
