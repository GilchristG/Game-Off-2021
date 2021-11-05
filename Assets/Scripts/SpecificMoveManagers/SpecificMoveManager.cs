using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpecificMoveManager : MonoBehaviour
{
    public MyCharacterController moveController;
    public abstract void DoCoolStuff(int i);
}
