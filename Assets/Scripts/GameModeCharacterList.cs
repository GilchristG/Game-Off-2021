using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGameModeCharacterList", menuName = "Character Assets/Chararacter List", order = 0)]
public class GameModeCharacterList : ScriptableObject
{
    public List<CharacterPref> charactersForMode;
}

public struct CharacterPref
{
    public EnumCharacter characterType;
    public GameObject character_pf;
}
