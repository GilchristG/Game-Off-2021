using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CharacterSelection : MonoBehaviour
{
    private enum Player { P1, P2 };

    public Image player1Image;
    public Image player2Image;

    public Image highlight1;
    public Image highlight2;

    public Sprite bombardier;
    public Sprite hercules;

    private Character currentCharacter1;
    private Character currentCharacter2;

    private Player currentPlayer;

    private MainMenu menuControls;

    private InputAction moveRightAction;
    private InputAction moveLeftAction;
    private InputAction moveUpAction;
    private InputAction moveDownAction;
    private InputAction startAction;
    private InputAction selectAction;

    private void Awake()
    {
        menuControls = new MainMenu();

        currentCharacter1 = Character.BOMBARDIER;
        currentCharacter2 = Character.HERCULES;

        player1Image.sprite = bombardier;
        player2Image.sprite = hercules;

        highlight1.enabled = true;
        highlight2.enabled = false;

        currentPlayer = Player.P1;

        moveRightAction = menuControls.Menus.MoveRight;
        moveLeftAction = menuControls.Menus.MoveLeft;
        moveUpAction = menuControls.Menus.MoveUp;
        moveDownAction = menuControls.Menus.MoveDown;
        startAction = menuControls.Menus.Start;
        selectAction = menuControls.Menus.Select;

        moveRightAction.started += MoveRight;
        moveLeftAction.started += MoveLeft;
        moveUpAction.started += MoveUp;
        moveDownAction.started += MoveDown;
        startAction.performed += MyStart;
        selectAction.performed += Select;
    }

    private void MoveRight(InputAction.CallbackContext context)
    {
        Debug.Log("Right");
        swapHighlight();
    }

    private void MoveLeft(InputAction.CallbackContext context)
    {
        Debug.Log("Left");
        swapHighlight();
    }

    private void MoveUp(InputAction.CallbackContext context)
    {
        Debug.Log("Up");
        cycleCharacterSelection();
    }

    private void MoveDown(InputAction.CallbackContext context)
    {
        Debug.Log("Down");
        cycleCharacterSelection();
    }

    private void MyStart(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene("MainScene", LoadSceneMode.Additive);
    }

    private void Select(InputAction.CallbackContext context)
    {
        //This is here just in case, technically we don't need to use this for anything
    }

    public void swapHighlight()
    {
        if (highlight1.enabled)
        {
            highlight1.enabled = false;
            highlight2.enabled = true;
            currentPlayer = Player.P2;
        }
        else if (highlight2.enabled)
        {
            highlight2.enabled = false;
            highlight1.enabled = true;
            currentPlayer = Player.P1;
        }
    }

    public void cycleCharacterSelection()
    {
        if (currentPlayer == Player.P1)
        {
            if (currentCharacter1 == Character.BOMBARDIER)
            {
                currentCharacter1 = Character.HERCULES;
                player1Image.sprite = hercules;
            }
            else if (currentCharacter1 == Character.HERCULES)
            {
                currentCharacter1 = Character.BOMBARDIER;
                player1Image.sprite = bombardier;
            }
        }
        else if (currentPlayer == Player.P2)
        {

            if (currentCharacter2 == Character.BOMBARDIER)
            {
                currentCharacter2 = Character.HERCULES;
                player2Image.sprite = hercules;
            }
            else if (currentCharacter2 == Character.HERCULES)
            {
                currentCharacter2 = Character.BOMBARDIER;
                player2Image.sprite = bombardier;
            }
        }
    }

    public void StartButton()
    {
        MyStart(new InputAction.CallbackContext());
    }
}
