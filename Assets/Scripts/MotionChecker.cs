using System.Collections.Generic;

public class MotionChecker
{
    //TODO: ADD DPs

    int[] fc_R = new int[] {6,3,2,1,4,7,8,9,6};
    int[] hc_R = new int[] {6,3,2,1,4,6};
    int[] qcf_R = new int[] { 2, 3, 6 };
    int[] qcb_R = new int[] { 2, 1, 4 };

    int[] towards_R = new int[] { 6 };
    int[] away_R = new int[] { 4 };

    int[] fc_L = new int[] { 4,1,2,3,6,9,8,7,4 };
    int[] hc_L = new int[] { 4,1,2,3,6,4};
    int[] qcf_L = new int[] { 2,1,4};
    int[] qcb_L = new int[] { 2,3,6 };

    int[] towards_L = new int[] { 4 };
    int[] away_L = new int[] { 6 };

    int[] neutral = new int[] { 5 };
    int[] down = new int[] {2};

    public MotionType CheckForSingleMostComplicated(CharacterInput inputSequence, bool facingRight)
    {
        if (facingRight)
        {
            if (inputSequence.CheckSequence(fc_R, 24))
            {
                return MotionType.FC;
            }
            else if(inputSequence.CheckSequence(hc_R, 20))
            {
                return MotionType.HCF;
            }
            else if(inputSequence.CheckSequence(qcf_R, 16))
            {
                return MotionType.QCF;
            }
            else if(inputSequence.CheckSequence(qcb_R,16))
            {
                return MotionType.QCB;
            }
            else if(inputSequence.CheckSequence(towards_R,8))
            {
                return MotionType.Towards;
            }
            else if (inputSequence.CheckSequence(away_R, 8))
            {
                return MotionType.Away;
            }
            else if (inputSequence.CheckSequence(down, 8))
            {
                return MotionType.Down;
            }
        }
        else
        {
            if (inputSequence.CheckSequence(fc_L, 24))
            {
                return MotionType.FC;
            }
            else if (inputSequence.CheckSequence(hc_L, 20))
            {
                return MotionType.HCF;
            }
            else if (inputSequence.CheckSequence(qcf_L, 16))
            {
                return MotionType.QCF;
            }
            else if (inputSequence.CheckSequence(qcb_L, 16))
            {
                return MotionType.QCB;
            }
            else if (inputSequence.CheckSequence(towards_L, 8))
            {
                return MotionType.Towards;
            }
            else if (inputSequence.CheckSequence(away_L, 8))
            {
                return MotionType.Away;
            }
            else if (inputSequence.CheckSequence(down, 8))
            {
                return MotionType.Down;
            }
        }

        return MotionType.Neutral;
    }

    public List<MotionType> CheckForAllApplicable(CharacterInput inputSequence, bool facingRight)
    {
        List<MotionType> moves = new List<MotionType>();

        if (facingRight)
        {
            if (inputSequence.CheckSequence(fc_R, 24))
            {
                moves.Add(MotionType.FC);
            }
            if (inputSequence.CheckSequence(hc_R, 20))
            {
                moves.Add(MotionType.HCF);
            }
            if (inputSequence.CheckSequence(qcf_R, 16))
            {
                moves.Add(MotionType.QCF);
            }
            if (inputSequence.CheckSequence(qcb_R, 16))
            {
                moves.Add(MotionType.QCB);
            }
            if (inputSequence.CheckSequence(towards_R, 8))
            {
                moves.Add(MotionType.Towards);
            }
            if (inputSequence.CheckSequence(away_R, 8))
            {
                moves.Add(MotionType.Away);
            }
            if (inputSequence.CheckSequence(down, 8))
            {
                moves.Add(MotionType.Down);
            }
        }
        else
        {
            if (inputSequence.CheckSequence(fc_L, 24))
            {
                moves.Add(MotionType.FC);
            }
            if (inputSequence.CheckSequence(hc_L, 20))
            {
                moves.Add(MotionType.HCF);
            }
            if (inputSequence.CheckSequence(qcf_L, 16))
            {
                moves.Add(MotionType.QCF);
            }
            if (inputSequence.CheckSequence(qcb_L, 16))
            {
                moves.Add(MotionType.QCB);
            }
            if (inputSequence.CheckSequence(towards_L, 8))
            {
                moves.Add(MotionType.Towards);
            }
            if (inputSequence.CheckSequence(away_L, 8))
            {
                moves.Add(MotionType.Away);
            }
            if (inputSequence.CheckSequence(down, 8))
            {
                moves.Add(MotionType.Down);
            }
        }

        if (inputSequence.CheckSequence(neutral, 8))
        {
            moves.Add(MotionType.Neutral);
        }

        return moves;
    }
}
