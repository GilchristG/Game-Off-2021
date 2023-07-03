using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGridInfo : MonoBehaviour
{
    public int rowLength = 4;
    CharacterPanel[] allChildren;
    //The number of children in regards to array counting
    int numberOfChildren = 0;

    void Awake()
    {
        allChildren = GetComponentsInChildren<CharacterPanel>();
        numberOfChildren = allChildren.Length - 1;
    }

    public CharacterPanel GetPanelAt(Vector2Int direction, ref int element)
    {
        if (direction.y > 0)
        {
            element -= rowLength;
            if(element < 0)
            {
                element += numberOfChildren;
            }
        }
        else if(direction.y < 0)
        {
            element += rowLength;
            if (element > numberOfChildren)
            {
                element -= numberOfChildren;
            }
        }

        if (direction.x > 0)
        {
            element++;
            if (element > numberOfChildren)
            {
                element = 0;
            }
        }
        else if (direction.x < 0)
        {
            element--;
            if (element < 0)
            {
                element = numberOfChildren;
            }
        }

        return allChildren[element];
    }


}
