using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterInput
{
    int bufferSize = 60;
    [SerializeField] InputFrame[] buffer = new InputFrame[60];

    [SerializeField] int CurrentTick = 0;

    public bool training = false;

    BufferVisualizer bv;

    public CharacterInput()
    {
        buffer = new InputFrame[60];
    }

    public void SetupBV(BufferVisualizer bufferV)
    {
        bv = bufferV;
    }

    //maxDuration must not be bigger than the bufferSize;
    public bool CheckSequence(int[] sequence, int maxDuration)
    {
        int w = sequence.Length - 1;

        for (int i = 0; i < maxDuration; i++)
        {
            int direction = buffer[(CurrentTick - i + bufferSize) % bufferSize].inputs[0];

            if (direction == sequence[w])
                --w;
            if (w == -1)
                return true;
        }

        return false;
    }

    //Push input keys into buffer at key 0
    public void PushIntoBuffer(InputFrame inputs)
    {
        CurrentTick++;
        if (CurrentTick > bufferSize-1)
        {
            CurrentTick = 0;
        }

        buffer[CurrentTick] = inputs;

        if(training)
        {
            bv?.DisplayBuffer(inputs);
        }
    }

    public InputFrame GetCurrentFrame()
    {
        return buffer[CurrentTick];
    }
}
