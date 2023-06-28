using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ModeLauncher : MonoBehaviour
{
    protected ProgramManager manager;
    public virtual void OnEnable()
    {
        manager = FindObjectOfType<ProgramManager>();
    }

    public virtual void LaunchMode()
    {

    }


}
