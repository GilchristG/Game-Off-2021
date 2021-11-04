using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    MyCharacterController moveController;


    // Start is called before the first frame update
    void Start()
    {
        moveController = GetComponentInParent<MyCharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SignalReadyForInput(int trueOrFalse)
    {
        moveController.acceptingCommands = (trueOrFalse == 0)? false:true;
    }
}
