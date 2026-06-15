using UnityEngine;
using System;

namespace Tanks.Complete
{
    public struct PowerUpState : IEquatable<PowerUpState>
    {
        public int x;
        public int z;
        public int type;
        public int isActive;

        public PowerUpState(Transform transform, PowerUpML.PowerUpType type, float gridSize)
        {
            this.x = Mathf.FloorToInt(transform.position.x / gridSize);
            this.z = Mathf.FloorToInt(transform.position.z / gridSize);
            this.type = (int) type;
            this.isActive = 1;
        }

        public PowerUpState(Transform transform, float gridSize)
        {
            this.x = Mathf.FloorToInt(transform.position.x / gridSize);
            this.z = Mathf.FloorToInt(transform.position.z / gridSize);
            this.type = 0;
            this.isActive = 0;
        }

        public bool Equals(PowerUpState other)
        {
            return x == other.x &&
                z == other.z &&
                type == other.type &&
                isActive == other.isActive;
        }

        public override string ToString()
        {
            return $"{x}_{z}_{type}_{isActive}";
        }
    }

    public struct TankState : IEquatable<TankState>
    {
        public int x;
        public int z;
        public int vel;
        public int angle;
        public int health;
        public int canShoot;
        // public int cooldownTimer;
        public int powerUp;
        public int hasProjectileInAir;
        
        // adding a variable for the power ups (should i add also the position of the power ups?)

        // we could add information about the enemy and where the projectile is
        public TankState(Transform transform, Rigidbody rb, float currentHP, bool canShoot, float cooldown, PowerUpML.PowerUpType type, float gridSize, bool hasProjectileInAir)
        {
            this.x = Mathf.FloorToInt(transform.localPosition.x / gridSize);
            this.z = Mathf.FloorToInt(transform.localPosition.z / gridSize);

            this.vel = Mathf.FloorToInt(transform.InverseTransformDirection(rb.linearVelocity).z);
            if (Mathf.Abs(vel) < 0.5f) this.vel = 0;     
            else if (vel > 0.5f) this.vel = 1;           
            else this.vel = -1;

            float rot = transform.eulerAngles.y;
            this.angle = Mathf.RoundToInt(rot / 45.0f) % 8;

            // starting 100
            // facciamo 4 bucket high, medium, low, dead (che è stato limite)
            if (currentHP <= 0) this.health = 0;
            else if (currentHP > 0 && currentHP <= 30) this.health = 1;
            else if (currentHP > 30 && currentHP <= 65) this.health = 2;
            else this.health = 3;
            
            // float speed = rb.linearVelocity.magnitude;
            this.canShoot = canShoot ? 1 : 0;

            this.powerUp = (int) type;

            this.hasProjectileInAir = hasProjectileInAir ? 1 : 0;
        }

        public TankState(int x, int z, int vel, int angle, int health, int canShoot, int powerUp, int hasProjectileInAir)
        {
            this.x = x;
            this.z = z;
            this.vel = vel;
            this.angle = angle;
            this.health = health;
            this.canShoot = canShoot;
            this.powerUp = powerUp;
            this.hasProjectileInAir = hasProjectileInAir;
        }

        public static TankState Parse(string s)
        {
            string[] p = s.Split('_');
            return new TankState(
                int.Parse(p[0]), int.Parse(p[1]), int.Parse(p[2]), int.Parse(p[3]),
                int.Parse(p[4]), int.Parse(p[5]), int.Parse(p[6]), int.Parse(p[7])
            );
        }

        public bool Equals(TankState other)
        {
            return x == other.x &&
                z == other.z &&
                vel == other.vel &&
                angle == other.angle &&
                health == other.health &&
                canShoot == other.canShoot &&
                //cooldownTimer == other.cooldownTimer &&
                powerUp == other.powerUp &&
                hasProjectileInAir == other.hasProjectileInAir; 
        }

        public override string ToString()
        {
            return $"{x}_{z}_{vel}_{angle}_{health}_{canShoot}_{powerUp}_{hasProjectileInAir}";
        }

        public override int GetHashCode()
        {
            unchecked 
            {
                int hash = 17;
                // Esempio per TankState, ripeti logica simile per le altre
                hash = hash * 31 + x.GetHashCode();
                hash = hash * 31 + z.GetHashCode();
                hash = hash * 31 + vel.GetHashCode();
                hash = hash * 31 + angle.GetHashCode();
                hash = hash * 31 + health.GetHashCode();
                hash = hash * 31 + canShoot.GetHashCode();
                hash = hash * 31 + powerUp.GetHashCode();
                hash = hash * 31 + hasProjectileInAir.GetHashCode();
                return hash;
            }
        }

        public override bool Equals(object obj) => obj is TankState other && Equals(other);

    }

    public struct GlobalGameState: IEquatable<GlobalGameState>
    {
        public const int MaxPowerUps = 4;

        public TankState t1;
        public TankState t2;

        //public PowerUpState p1;
        //public PowerUpState p2;
        //public PowerUpState p3;
        //public PowerUpState p4;

        public GlobalGameState(
            TankState t1,
            TankState t2
        //    PowerUpState p1,
        //    PowerUpState p2,
        //    PowerUpState p3,
        //    PowerUpState p4
        )
        {
            this.t1 = t1;
            this.t2 = t2;
            //this.p1 = p1;
            //this.p2 = p2;
            //this.p3 = p3;
            //this.p4 = p4;
        }

        public static GlobalGameState Parse(string s)
        {
            string[] p = s.Split('|');
            return new GlobalGameState(TankState.Parse(p[0]), TankState.Parse(p[1]));
        }

        public bool Equals(GlobalGameState other)
        {
            return t1.Equals(other.t1) &&
                t2.Equals(other.t2); 
            //    p1.Equals(other.p1) &&
            //    p2.Equals(other.p2) &&
            //    p3.Equals(other.p3) &&
            //    p4.Equals(other.p4);
        }

        public override int GetHashCode()
        {
            unchecked {
                return (t1.GetHashCode() * 397) ^ t2.GetHashCode();
            }
        }

        public override string ToString()
        {
            return $"T1[{t1}]_T2[{t2}]";
            //return $"T1[{t1}]_T2[{t2}]_P1[{p1}]_P2[{p2}]_P3[{p3}]_P4[{p4}]";
        }

        public string ToCompactString()
        {
            return $"{t1}|{t2}";
            //return $"{t1}|{t2}|{p1}|{p2}|{p3}|{p4}";
        }

    }
}
