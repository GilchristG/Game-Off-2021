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
    public const int INPUT_THRUST = (1 << 0);
    public const int INPUT_BREAK = (1 << 1);
    public const int INPUT_ROTATE_LEFT = (1 << 2);
    public const int INPUT_ROTATE_RIGHT = (1 << 3);
    public const int INPUT_FIRE = (1 << 4);
    public const int INPUT_BOMB = (1 << 5);
    public const int MAX_BULLETS = 30;

    public const float PI = 3.1415926f;
    public const int STARTING_HEALTH = 100;
    public const float ROTATE_INCREMENT = 3f;
    public const float SHIP_RADIUS = 15f;
    public const float SHIP_THRUST = 0.06f;
    public const float SHIP_MAX_THRUST = 4.0f;
    public const float SHIP_BREAK_SPEED = 0.6f;
    public const float BULLET_SPEED = 5f;
    public const int BULLET_COOLDOWN = 8;
    public const int BULLET_DAMAGE = 10;
}


[Serializable]
public class Fighter
{
    public Vector2 position;
    public Vector2 velocity;
    public int health;
    //public Bullet[] bullets = new Bullet[MAX_BULLETS];

    public void Serialize(BinaryWriter bw)
    {
        bw.Write(position.x);
        bw.Write(position.y);
        bw.Write(velocity.x);
        bw.Write(velocity.y);
        bw.Write(health);
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
        return hashCode;
    }

}


public struct BBGame : IGame
{
    public int Framenumber { get; private set; }

    public int Checksum => GetHashCode();

    public Fighter[] _fighters;

    public static Rect _bounds = new Rect(0, 0, 640, 480);

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

    public BBGame(int num_players)
    {
        var w = _bounds.xMax - _bounds.xMin;
        var h = _bounds.yMax - _bounds.yMin;
        var r = h / 4;
        Framenumber = 0;
        _fighters = new Fighter[num_players];
        for (int i = 0; i < _fighters.Length; i++)
        {
            _fighters[i] = new Fighter();
            int heading = i * 360 / num_players;
            float cost, sint, theta;

            theta = (float)heading * PI / 180;
            cost = Mathf.Cos(theta);
            sint = Mathf.Sin(theta);

            _fighters[i].position.x = (w / 2) + r * cost;
            _fighters[i].position.y = (h / 2) + r * sint;
            _fighters[i].health = STARTING_HEALTH;
        }
    }

    public void GetFigherAI(int i, out float heading, out float thrust, out int fire)
    {
        //Get an AI to take over for a player if they disconnect
        heading = 0;// (_fighters[i].heading + 5) % 360;
        thrust = 0;
        fire = 0;
    }

    public void ParseShipInputs(long inputs, int i, out float heading, out float thrust, out int fire)
    {
        var fighter = _fighters[i];

        GGPORunner.LogGame($"parsing ship {i} inputs: {inputs}.");

        //TODO: Parse the barebones inputs into proper inputs for local fighers here

        
        /*if ((inputs & INPUT_ROTATE_RIGHT) != 0)
        {
            heading = (fighter.heading - ROTATE_INCREMENT) % 360;
        }
        else if ((inputs & INPUT_ROTATE_LEFT) != 0)
        {
            heading = (fighter.heading + ROTATE_INCREMENT + 360) % 360;
        }
        else
        {
            heading = fighter.heading;
        }*/

        heading = 0;

        if ((inputs & INPUT_THRUST) != 0)
        {
            thrust = SHIP_THRUST;
        }
        else if ((inputs & INPUT_BREAK) != 0)
        {
            thrust = -SHIP_THRUST;
        }
        else
        {
            thrust = 0;
        }
        fire = (int)(inputs & INPUT_FIRE);
        
    }

    public void ProcessFighter(int index, float heading, float thrust, int fire)
    {
        var fighter = _fighters[index];

        //GGPORunner.LogGame($"calculation of new fighter coordinates: (thrust:{thrust} heading:{heading}).");

        //TODO: Process local fighters here. Just send info to the prefab fighters


        /*if (fighter.cooldown == 0)
        {
            if (fire != 0)
            {
                GGPORunner.LogGame("firing bullet.");
                for (int i = 0; i < ship.bullets.Length; i++)
                {
                    float dx = Mathf.Cos(DegToRad(ship.heading));
                    float dy = Mathf.Sin(DegToRad(ship.heading));
                    if (!ship.bullets[i].active)
                    {
                        ship.bullets[i].active = true;
                        ship.bullets[i].position.x = ship.position.x + (ship.radius * dx);
                        ship.bullets[i].position.y = ship.position.y + (ship.radius * dy);
                        ship.bullets[i].velocity.x = ship.velocity.x + (BULLET_SPEED * dx);
                        ship.bullets[i].velocity.y = ship.velocity.y + (BULLET_SPEED * dy);
                        ship.cooldown = BULLET_COOLDOWN;
                        break;
                    }
                }
            }
        }*/

        if (thrust != 0)
        {
            float dx = thrust * Mathf.Cos(DegToRad(heading));
            float dy = thrust * Mathf.Sin(DegToRad(heading));

            fighter.velocity.x += dx;
            fighter.velocity.y += dy;
            float mag = Mathf.Sqrt(fighter.velocity.x * fighter.velocity.x +
                             fighter.velocity.y * fighter.velocity.y);
            if (mag > SHIP_MAX_THRUST)
            {
                fighter.velocity.x = (fighter.velocity.x * SHIP_MAX_THRUST) / mag;
                fighter.velocity.y = (fighter.velocity.y * SHIP_MAX_THRUST) / mag;
            }
        }
        GGPORunner.LogGame($"new ship velocity: (dx:{fighter.velocity.x} dy:{fighter.velocity.y}).");

        fighter.position.x += fighter.velocity.x;
        fighter.position.y += fighter.velocity.y;
        GGPORunner.LogGame($"new ship position: (dx:{fighter.position.x} dy:{fighter.position.y}).");

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
        fp += string.Format("  num_ships: {0}.\n", _fighters.Length);
        for (int i = 0; i < _fighters.Length; i++)
        {
            var fighter = _fighters[i];
            fp += string.Format("  ship {0} position:  %.4f, %.4f\n", i, fighter.position.x, fighter.position.y);
            fp += string.Format("  ship {0} velocity:  %.4f, %.4f\n", i, fighter.velocity.x, fighter.velocity.y);
            fp += string.Format("  ship {0} health:    %d.\n", i, fighter.health);
        }
        File.WriteAllText(filename, fp);
    }

    public void Update(long[] inputs, int disconnect_flags)
    {
        Framenumber++;
        for (int i = 0; i < _fighters.Length; i++)
        {
            float thrust, heading;
            int fire;

            if ((disconnect_flags & (1 << i)) != 0)
            {
                GetFigherAI(i, out heading, out thrust, out fire);
            }
            else
            {
                ParseShipInputs(inputs[i], i, out heading, out thrust, out fire);
            }
            ProcessFighter(i, heading, thrust, fire);

            /* This was the between bullet cooldown
            if (_fighters[i].cooldown != 0)
            {
                _fighters[i].cooldown--;
            }*/
        }
    }

    //TODO: Fix this up for our game's needs
    public long ReadInputs(int id)
    {
        long input = 0;

        if (id == 0)
        {
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.UpArrow))
            {
                input |= INPUT_THRUST;
            }
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.DownArrow))
            {
                input |= INPUT_BREAK;
            }
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.LeftArrow))
            {
                input |= INPUT_ROTATE_LEFT;
            }
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.RightArrow))
            {
                input |= INPUT_ROTATE_RIGHT;
            }
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.RightControl))
            {
                input |= INPUT_FIRE;
            }
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.RightShift))
            {
                input |= INPUT_BOMB;
            }
        }
        else if (id == 1)
        {
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.W))
            {
                input |= INPUT_THRUST;
            }
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.S))
            {
                input |= INPUT_BREAK;
            }
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.A))
            {
                input |= INPUT_ROTATE_LEFT;
            }
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.D))
            {
                input |= INPUT_ROTATE_RIGHT;
            }
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.F))
            {
                input |= INPUT_FIRE;
            }
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.G))
            {
                input |= INPUT_BOMB;
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
