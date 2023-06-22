[System.Serializable]
public class Move
{
    public MoveState moveState;
    public string animationName;
    public int damage;
    public int windupTime;
    public int activeTime;
    public int basicRecovery;
    public int bufferWindow;
    public int hitStunTime;
    public int blockStunTime;
    public float knockbackForce;
    public float launchForce;

    public Move(string anim, int dmg, int windup, int active, int recovery, int buffer, int hitStun, int blockStun, float kf, float lf)
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
        knockbackForce = kf;
        launchForce = lf;
    }
}
