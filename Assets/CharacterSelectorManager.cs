using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectorManager : MonoBehaviour
{
    public CharacterGridInfo gridInfo;

    public GameObject p1CharacterAvatar;
    public GameObject p2CharacterAvatar;

    CharacterPanel p1Selection;
    int p1Element = 0;
    bool p1Selected = false;
    CharacterPanel p2Selection;
    int p2Element = 1;
    bool p2Selected = false;

    public GameObject p1PointerGraphic;
    public GameObject p2PointerGrapic;

    ProgramManager programManager;

    private void OnEnable()
    {
        programManager = FindObjectOfType<ProgramManager>();
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
                break;
            case 2:
                p2Selection = gridInfo.GetPanelAt(direction, ref p2Element);
                break;
        }
    }

    public void Move(/*Input */)
    {
        Vector2Int newDirection = new Vector2Int(0, 0);
        //Get player input and send to process

        Process(0, newDirection);
    }

    public void Select(/*Input*/)
    {
        int player = 1;

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
        }
    }

    public void Back()
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
