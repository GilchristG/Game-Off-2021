using System.IO;
using UnityEngine;
using Unity.Collections;
using SharedGame;
using System;

using static BBConstrants;

public static class BBConstrants
{
    public const int MAX_FIGHTERS = 2;
    public const int MAX_PLAYERS = 64;


    //THESE WERE LEFT HERE FROM THE EXAMPLE CODE. I NEED TO CHANGE THESE TO FIT OUR GAME
    //Use the bitwise opperations to help with making input packets smaller
    public const int INPUT_DIRECTION_UP = (1 << 0); //0000 0000 0000 0001
    public const int INPUT_DIRECTION_DOWN = (1 << 1); //0000 0000 0000 0010
    public const int INPUT_DIRECTION_LEFT = (1 << 2); //Etc
    public const int INPUT_DIRECTION_RIGHT = (1 << 3);
    public const int INPUT_LIGHT = (1 << 4);
    public const int INPUT_MEDIUM = (1 << 5);
    public const int INPUT_HEAVY = (1 << 6);
    public const int INPUT_SPECIAL = (1 << 7);
    //There is still the last byte of info we can use




    public const float PI = 3.1415926f;
    public const int STARTING_HEALTH = 100;
    public const int STARTING_STATE = 0;
    public const int STARTING_STANCE = 0;

    public const float DEFAULT_FIGHTER_SPEED = 5f;
    public const float DEFAULT_FIGHTER_JUMP_MULTI = 3f;

    //I DON'T THINK WE'LL NEED THIS
    public const float ROTATE_INCREMENT = 3f;
    public const float BULLET_SPEED = 5f;
    public const int BULLET_COOLDOWN = 8;
    public const int BULLET_DAMAGE = 10;

    public const int MAX_BULLETS = 30;
}


[Serializable]
public class Fighter
{
    public Vector2 position;
    public Vector2 velocity;
    public int health;
    public int state;
    public int stance;
    public int move;
    public int moveFrame;

    public CharacterInput fighterInput = new CharacterInput();

    //Leaving this here as a reminder we might want projectiles
    //public Bullet[] bullets = new Bullet[MAX_BULLETS];

    public void Serialize(BinaryWriter bw)
    {
        bw.Write(position.x);
        bw.Write(position.y);
        bw.Write(velocity.x);
        bw.Write(velocity.y);
        bw.Write(health);
        bw.Write(state);
        bw.Write(stance);
        bw.Write(move);
        /*for (int i = 0; i < MAX_BULLETS; ++i)
        {
            bullets[i].Serialize(bw);
        }*/
    }

    public void Deserialize(BinaryReader br)
    {
        position.x = br.ReadSingle();
        position.y = br.ReadSingle();
        velocity.x = br.ReadSingle();
        velocity.y = br.ReadSingle();
        health = br.ReadInt32();
        state = br.ReadInt32();
        stance = br.ReadInt32();
        move = br.ReadInt32();
        /*for (int i = 0; i < MAX_BULLETS; ++i)
        {
            bullets[i].Deserialize(br);
        }*/
    }

    // @LOOK Not hashing bullets.
    public override int GetHashCode()
    {
        int hashCode = 1858597544;
        hashCode = hashCode * -1521134295 + position.GetHashCode();
        hashCode = hashCode * -1521134295 + velocity.GetHashCode();
        hashCode = hashCode * -1521134295 + health.GetHashCode();
        hashCode = hashCode * -1521134295 + state.GetHashCode();
        hashCode = hashCode * -1521134295 + stance.GetHashCode();
        hashCode = hashCode * -1521134295 + move.GetHashCode();
        return hashCode;
    }

}


public struct BBGame : IGame
{
    public int Framenumber { get; private set; }

    public int Checksum => GetHashCode();

    public Fighter[] _fighters;

    public static Rect _bounds = new Rect(-14, -2, 28, 30);

    public void Serialize(BinaryWriter bw)
    {
        bw.Write(Framenumber);
        bw.Write(_fighters.Length);
        for (int i = 0; i < _fighters.Length; ++i)
        {
            _fighters[i].Serialize(bw);
        }
    }

    public void Deserialize(BinaryReader br)
    {
        Framenumber = br.ReadInt32();
        int length = br.ReadInt32();
        if (length != _fighters.Length)
        {
            _fighters = new Fighter[length];
        }
        for (int i = 0; i < _fighters.Length; ++i)
        {
            _fighters[i].Deserialize(br);
        }
    }

    public NativeArray<byte> ToBytes()
    {
        using (var memoryStream = new MemoryStream())
        {
            using (var writer = new BinaryWriter(memoryStream))
            {
                Serialize(writer);
            }
            return new NativeArray<byte>(memoryStream.ToArray(), Allocator.Persistent);
        }
    }

    public void FromBytes(NativeArray<byte> bytes)
    {
        using (var memoryStream = new MemoryStream(bytes.ToArray()))
        {
            using (var reader = new BinaryReader(memoryStream))
            {
                Deserialize(reader);
            }
        }
    }

    private static float DegToRad(float deg)
    {
        return PI * deg / 180;
    }

    private static float Distance(Vector2 lhs, Vector2 rhs)
    {
        float x = rhs.x - lhs.x;
        float y = rhs.y - lhs.y;
        return Mathf.Sqrt(x * x + y * y);
    }

    /*
     * InitGameState --
     *
     * Initialize our game state.
     */

    //TODO: Update the player initialization
    public BBGame(int num_players)
    {
        var w = _bounds.xMax - _bounds.xMin;
        var h = _bounds.yMax - _bounds.yMin;
        var r = w / 2;
        Framenumber = 0;
        _fighters = new Fighter[num_players];
        for (int i = 0; i < _fighters.Length; i++)
        {
            _fighters[i] = new Fighter();


            _fighters[i].position.x = _bounds.x + (w / 4) + (r * i);
            _fighters[i].position.y = _bounds.y + 1.45f;
            _fighters[i].health = STARTING_HEALTH;
            _fighters[i].velocity = new Vector2(0, 0);
            _fighters[i].state = STARTING_STATE;
            _fighters[i].stance = STARTING_STANCE;
        }
    }

    //TODO: Make a prediction here
    public void GetFigherAI(int i, out int moveDirection, out bool light, out bool medium, out bool heavy, out bool special)
    {
        //Get an AI to take over for a player for frames you don't have/disconnect. PREDICT THEIR ACTIONS HERE
        moveDirection = 0;// (_fighters[i].heading + 5) % 360;
        //thrust = 0;
        light = false;
        medium = false;
        heavy = false;
        special = false;
    }

    public void ParseShipInputs(long inputs, int i, out int numDirection, out bool light, out bool medium, out bool heavy, out bool special)
    {
        var fighter = _fighters[i];

        GGPORunner.LogGame($"parsing fighter {i} inputs: {inputs}.");

        Vector2 moveDirection = new Vector2(0, 0);

        if ((inputs & INPUT_DIRECTION_UP) != 0)
        {
            //heading = (fighter.heading - ROTATE_INCREMENT) % 360;
            moveDirection += new Vector2(0, 1);
        }
        else if ((inputs & INPUT_DIRECTION_DOWN) != 0)
        {
            moveDirection += new Vector2(0, -1);
        }
        else if ((inputs & INPUT_DIRECTION_LEFT) != 0)
        {
            moveDirection = new Vector2(-1, 0);
        }
        else if ((inputs & INPUT_DIRECTION_RIGHT) != 0)
        {
            moveDirection = new Vector2(1, 0);
        }

        //No direction input default
        numDirection = 5;

        if (moveDirection.y == 0)
        {
            if (moveDirection.x == 1)
            {
                numDirection = 6;
            }
            else if (moveDirection.x == -1)
            {
                numDirection = 4;
            }
        }
        else if (moveDirection.y == -1)
        {
            if (moveDirection.x == 1)
            {
                numDirection = 3;
            }
            else if (moveDirection.x == -1)
            {
                numDirection = 1;
            }
            else
            {
                numDirection = 2;
            }
        }
        else if (moveDirection.y == 1)
        {
            if (moveDirection.x == 1)
            {
                numDirection = 9;
            }
            else if (moveDirection.x == -1)
            {
                numDirection = 7;
            }
            else
            {
                numDirection = 8;
            }
        }


        light = false;
        medium = false;
        heavy = false;
        special = false;


        if ((inputs & INPUT_LIGHT) != 0)
        {
            light = true;
        }
        else if ((inputs & INPUT_MEDIUM) != 0)
        {
            medium = true;
        }
        else if((inputs & INPUT_HEAVY) != 0)
        {
            heavy = true;
        }
        else if ((inputs & INPUT_SPECIAL) != 0)
        {
            special = true;
        }
        
        //L = 1
        //M = 5
        //H = 10
        //S = 20

        //L M = 4
        //L H = 11
        //L S = 21
        
        //M H = 15
        //M S = 25
 
        //H S = 30

        //L M H = 16
        //L H S = 31
        //M H S = 35
        //L M S = 26
        
        //L M H S = 36

    }

    //TODO: Make fighters update as needed
    public void ProcessFighter(int index, int numDirection, bool light, bool medium, bool heavy, bool special)
    {
        var fighter = _fighters[index];

        //GGPORunner.LogGame($"calculation of new fighter coordinates: (thrust:{thrust} heading:{heading}).");

        //TODO: Process local fighters here. Just send info to the prefab fighters

        //TODO: Adjust frame inputs for special button
        //fighter.fighterInput.PushIntoBuffer(new InputFrame(numDirection, light ? 1 : 0, medium ? 1 : 0, heavy ? 1 : 0));//, special ? 1 : 0));


        if(numDirection == 6)
        {
            fighter.velocity = new Vector2(DEFAULT_FIGHTER_SPEED, 0);
        }
        else if(numDirection == 4)
        {
            fighter.velocity = new Vector2(-DEFAULT_FIGHTER_SPEED, 0);
        }
        else
        {
            fighter.velocity = new Vector2(0, 0);
        }

        //USE THIS TO LOG EVENTS
        //GGPORunner.LogGame("firing bullet.");

        GGPORunner.LogGame($"new fighter velocity: (dx:{fighter.velocity.x} dy:{fighter.velocity.y}).");

        fighter.position.x += fighter.velocity.x;
        fighter.position.y += fighter.velocity.y;
        GGPORunner.LogGame($"new fighter position: (dx:{fighter.position.x} dy:{fighter.position.y}).");

        //NOTE: Notice how the bullet collisions are done here
        /* OLD BULLET STUFF FROM SAMPLE GAME
        for (int i = 0; i < fighter.bullets.Length; i++)
        {
            if (fighter.bullets[i].active)
            {
                fighter.bullets[i].position.x += fighter.bullets[i].velocity.x;
                fighter.bullets[i].position.y += fighter.bullets[i].velocity.y;
                if (fighter.bullets[i].position.x < _bounds.xMin ||
                    fighter.bullets[i].position.y < _bounds.yMin ||
                    fighter.bullets[i].position.x > _bounds.xMax ||
                    fighter.bullets[i].position.y > _bounds.yMax)
                {
                    ship.bullets[i].active = false;
                }
                else
                {
                    for (int j = 0; j < _ships.Length; j++)
                    {
                        var other = _ships[j];
                        if (Distance(ship.bullets[i].position, other.position) < other.radius)
                        {
                            ship.score++;
                            other.health -= BULLET_DAMAGE;
                            ship.bullets[i].active = false;
                            break;
                        }
                    }
                }
            }
        }
        */
    }

    public void LogInfo(string filename)
    {
        string fp = "";
        fp += "GameState object.\n";
        fp += string.Format("  bounds: {0},{1} x {2},{3}.\n", _bounds.xMin, _bounds.yMin, _bounds.xMax, _bounds.yMax);
        fp += string.Format("  num_fighters: {0}.\n", _fighters.Length);
        for (int i = 0; i < _fighters.Length; i++)
        {
            var fighter = _fighters[i];
            fp += string.Format("  fighter {0} position:  %.4f, %.4f\n", i, fighter.position.x, fighter.position.y);
            fp += string.Format("  fighter {0} velocity:  %.4f, %.4f\n", i, fighter.velocity.x, fighter.velocity.y);
            fp += string.Format("  fighter {0} health:    %d.\n", i, fighter.health);
            fp += string.Format("  fighter {0} state:    %d.\n", i, (FighterState)fighter.state);
            fp += string.Format("  fighter {0} stance:    %d.\n", i, (FighterStance)fighter.stance);
            fp += string.Format("  fighter {0} move:    %d.\n", i, fighter.move);
            fp += string.Format("  fighter {0} moveFrame:    %d.\n", i, fighter.moveFrame);
        }
        File.WriteAllText(filename, fp);
    }

    public void Update(long[] inputs, int disconnect_flags)
    {
        Framenumber++;
        for (int i = 0; i < _fighters.Length; i++)
        {
            int moveDirection;
            bool l,m,h,s;

            if ((disconnect_flags & (1 << i)) != 0)
            {
                GetFigherAI(i, out moveDirection, out l, out m, out h, out s);
            }
            else
            {
                ParseShipInputs(inputs[i], i, out moveDirection, out l,out m,out h,out s);
            }
            ProcessFighter(i, moveDirection, l,m,h,s);

            /* This was the between bullet cooldown
            if (_fighters[i].cooldown != 0)
            {
                _fighters[i].cooldown--;
            }*/
        }
    }

    //TODO: Update this for the new unity input system too
    public long ReadInputs(int id)
    {
        long input = 0;

        if (id == 0)
        {
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.UpArrow))
            {
                input |= INPUT_DIRECTION_UP;
            }
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.DownArrow))
            {
                input |= INPUT_DIRECTION_DOWN;
            }
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.LeftArrow))
            {
                input |= INPUT_DIRECTION_LEFT;
            }
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.RightArrow))
            {
                input |= INPUT_DIRECTION_RIGHT;
            }
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.Keypad5))
            {
                input |= INPUT_LIGHT;
            }
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.Keypad6))
            {
                input |= INPUT_SPECIAL;
            }
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.Keypad2))
            {
                input |= INPUT_MEDIUM;
            }
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.Keypad3))
            {
                input |= INPUT_HEAVY;
            }
        }
        else if (id == 1)
        {
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.W))
            {
                input |= INPUT_DIRECTION_UP;
            }
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.S))
            {
                input |= INPUT_DIRECTION_DOWN;
            }
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.A))
            {
                input |= INPUT_DIRECTION_LEFT;
            }
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.D))
            {
                input |= INPUT_DIRECTION_RIGHT;
            }
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.Y))
            {
                input |= INPUT_LIGHT;
            }
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.U))
            {
                input |= INPUT_SPECIAL;
            }
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.H))
            {
                input |= INPUT_MEDIUM;
            }
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.J))
            {
                input |= INPUT_HEAVY;
            }
        }

        return input;
    }

    public void FreeBytes(NativeArray<byte> data)
    {
        if (data.IsCreated)
        {
            data.Dispose();
        }
    }

    public override int GetHashCode()
    {
        int hashCode = -1214587014;
        hashCode = hashCode * -1521134295 + Framenumber.GetHashCode();
        foreach (var fighter in _fighters)
        {
            hashCode = hashCode * -1521134295 + fighter.GetHashCode();
        }
        return hashCode;
    }
}
