using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSorter : IComparer<MoveData>
{
        public int Compare(MoveData x, MoveData y)
        {
            int compareMotion = x.motionSequence.CompareTo(y.motionSequence);
            if (compareMotion == 0)
            {
                return x.attackbuttons.Length.CompareTo(y.attackbuttons.Length);
            }
            return compareMotion;
        }
}
