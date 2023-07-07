[System.Serializable]
public class InputFrame
{
    //TODO: Add fourth button
    public int[] inputs = new int[5];

    public InputFrame(int[] newFrame)
    {
        inputs = newFrame;
    }

    public InputFrame(int direciton, int light, int medium, int heavy, int special)
    {
        inputs[0] = direciton;
        inputs[1] = light;
        inputs[2] = medium;
        inputs[3] = heavy;
        inputs[4] = special;
    }

    public InputFrame()
    {
        inputs = new int[5];
    }
}
