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
    CharacterPanel p2Selection;
    int p2Element = 1;

    public GameObject p1PointerGraphic;
    public GameObject p2PointerGrapic;

    public void Process(int player, Vector2Int direction)
    {
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


}
