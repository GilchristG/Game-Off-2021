[System.Serializable]
public class Move
{
    public MoveState moveState;
    public string animationName;
    public float damage;
    public int windupTime;
    public int activeTime;
    public int basicRecovery;
    public int bufferWindow;
    public int hitStunTime;
    public int blockStunTime;

    public Move(string anim, float dmg, int windup, int active, int recovery, int buffer, int hitStun, int blockStun)
    {
        moveState = MoveState.Waiting;
        animationName = anim;
        damage = dmg;
        windupTime = windup;
        activeTime = active;
        basicRecovery = recovery;
        bufferWindow = buffer;
        hitStunTime = hitStun;
        blockStunTime = blockStun;
    }
}
