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
    [SerializeField] int p2Element = 0;
    bool p2Selected = false;

    public GameObject p1PointerGraphic;
    public GameObject p2PointerGraphic;

    ProgramManager programManager;
    ModeLauncher modeLauncher;
    MainMenuManager mainMenu;

    InputAction p1Move;
    InputAction p1Confirm;
    InputAction p1Back;

    InputAction p2Move;
    InputAction p2Confirm;
    InputAction p2Back;

    private void OnEnable()
    {
        programManager = FindObjectOfType<ProgramManager>();

        if (programManager.p1Input != null)
        {
            p1Move = programManager.p1Input.actions["Move"];
            p1Confirm = programManager.p1Input.actions["Confirm"];
            p1Back = programManager.p1Input.actions["Back"];

            p1Move.performed += OnMove1;
            p1Confirm.performed += OnConfirm1;
            p1Back.performed += OnBack1;
        }

        if (programManager.p2Input != null)
        {
            p2Move = programManager.p2Input.actions["Move"];
            p2Confirm = programManager.p2Input.actions["Confirm"];
            p2Back = programManager.p2Input.actions["Back"];

            p2Move.performed += OnMove2;
            p2Confirm.performed += OnConfirm2;
            p2Back.performed += OnBack2;
        }

        mainMenu = FindObjectOfType<MainMenuManager>();

        modeLauncher = GetComponent<ModeLauncher>();
    }

    private void OnDisable()
    {
        p1Move.performed -= OnMove1;
        p1Confirm.performed -= OnConfirm1;
        p1Back.performed -= OnBack1;

        p2Move.performed -= OnMove2;
        p2Confirm.performed -= OnConfirm2;
        p2Back.performed -= OnBack2;
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

    public void OnMove1(InputAction.CallbackContext context)
    {
        OnMove(context, 1);
    }

    public void OnMove2(InputAction.CallbackContext context)
    {
        OnMove(context, 2);
    }

    public void OnMove(InputAction.CallbackContext context, int player)
    {
        Vector2 newInput = context.ReadValue<Vector2>();

        Vector2Int newDirection = new Vector2Int((int)newInput.x, (int)newInput.y); //new Vector2Int(Mathf.RoundToInt(newInput.x), Mathf.RoundToInt(newInput.y));
        //Get player input and send to process

        Process(player, newDirection);
    }

    public void OnConfirm1(InputAction.CallbackContext context)
    {
        OnConfirm(context, 1);
    }

    public void OnConfirm2(InputAction.CallbackContext context)
    {
        OnConfirm(context, 2);
    }

    public void OnConfirm(InputAction.CallbackContext context, int player)
    {
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

    public void OnBack1(InputAction.CallbackContext context)
    {
        OnBack(context, 1);
    }

    public void OnBack2(InputAction.CallbackContext context)
    {
        OnBack(context, 2);
    }

    public void OnBack(InputAction.CallbackContext context, int player)
    {
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
