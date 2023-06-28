using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflineVersusLauncher : ModeLauncher
{
    public override void LaunchMode()
    {
        base.LaunchMode();
        manager.LoadLocalMatch();
    }
}
