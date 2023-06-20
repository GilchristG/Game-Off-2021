using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSorter : IComparer<MoveData>
{
        public int Compare(MoveData x, MoveData y)
        {
            int compareMotion = y.motionSequence.CompareTo(x.motionSequence);
            if (compareMotion == 0)
            {
                return y.attackbuttons.Length.CompareTo(x.attackbuttons.Length);
            }
            return compareMotion;
        }
}
