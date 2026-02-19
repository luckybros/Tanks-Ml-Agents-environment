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
        // adding a variable for the power ups (should i add also the position of the power ups?)

        // we could add information about the enemy and where the projectile is
        public TankState(Transform transform, Rigidbody rb, float currentHP, bool canShoot, float cooldown, PowerUpML.PowerUpType type, float gridSize)
        {
            this.x = Mathf.FloorToInt(transform.position.x / gridSize);
            this.z = Mathf.FloorToInt(transform.position.z / gridSize);

            this.vel = Mathf.FloorToInt(transform.InverseTransformDirection(rb.linearVelocity).z);
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
                powerUp == other.powerUp; 
        }

        public override string ToString()
        {
            return $"{x}_{z}_{vel}_{angle}_{health}_{canShoot}_{powerUp}";
        }
    }

    public struct GlobalGameState: IEquatable<GlobalGameState>
    {
        public const int MaxPowerUps = 4;

        public TankState t1;
        public TankState t2;

        public PowerUpState p1;
        public PowerUpState p2;
        public PowerUpState p3;
        public PowerUpState p4;

        public GlobalGameState(
            TankState t1,
            TankState t2,
            PowerUpState p1,
            PowerUpState p2,
            PowerUpState p3,
            PowerUpState p4)
        {
            this.t1 = t1;
            this.t2 = t2;
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
            this.p4 = p4;
        }

        public bool Equals(GlobalGameState other)
        {
            return t1.Equals(other.t1) &&
                t2.Equals(other.t2) &&
                p1.Equals(other.p1) &&
                p2.Equals(other.p2) &&
                p3.Equals(other.p3) &&
                p4.Equals(other.p4);
        }

        public override string ToString()
        {
            return $"T1[{t1}]_T2[{t2}]_P1[{p1}]_P2[{p2}]_P3[{p3}]_P4[{p4}]";
        }

        public string ToCompactString()
        {
            return $"{t1}|{t2}|{p1}|{p2}|{p3}|{p4}";
        }

    }
}
