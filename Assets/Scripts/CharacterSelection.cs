using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{
    public Image player1Image;
    public Image player2Image;

    public Image highlight1;
    public Image highlight2;

    public Sprite bombardier;
    public Sprite hercules;

    private Character currentCharacter1;
    private Character currentCharacter2;

    private void Start()
    {
        currentCharacter1 = Character.BOMBARDIER;
        currentCharacter2 = Character.HERCULES;

        player1Image.sprite = bombardier;
        player2Image.sprite = hercules;

        highlight1.enabled = true;
        highlight2.enabled = false;
    }
}
