using System;
using UnityEngine;
using Weapon;
using Zenject;

namespace Control
{
    public class PlayerState : IDamageTaker, IWeaponHolder
    {
        private readonly SignalBus bus;

        private readonly Settings settings;

        private float health;

        public PlayerState(Settings settings, SignalBus bus)
        {
            this.settings = settings;
            this.bus = bus;
            this.health = settings.Health;
            this.Speed = settings.MoveSpeed;
        }

        public Vector3 LookDirection { get; set; }

        public Vector3 MoveDirection { get; set; }

        public Vector3 Position { get; set; }

        public bool OnHold { get; set; }

        public float Speed { get; private set; }

        public float TakeDamage(float damage, string damagerReceiverName)
        {
            var healthBefore = this.health;
            this.health -= damage;

            this.bus.Fire(new PlayerHealthChanged(healthBefore, this.health, this.settings.Health));

            this.UpdateSpeed();

            return 0f;
        }

        public void Reinitialize()
        {
        }

        private void UpdateSpeed()
        {
            var speedBefore = this.Speed;
            this.Speed = this.settings.MoveSpeed * this.health / this.settings.Health;

            this.bus.Fire(new SpeedChanged(speedBefore, this.Speed, this.settings.MoveSpeed));

            Debug.LogFormat("Player speed changed to {0}", this.Speed);
        }

        [Serializable]
        public class Settings
        {
            public float Health;
            public float MoveSpeed;
        }

        public class PlayerHealthChanged
        {
            public PlayerHealthChanged(float healthBefore, float healthAfter, float initialHealth)
            {
                this.HealthBefore = healthBefore;
                this.HealthAfter = healthAfter;
                this.InitialHealth = initialHealth;
            }

            public float HealthBefore { get; private set; }

            public float HealthAfter { get; private set; }

            public float InitialHealth { get; private set; }
        }

        public class SpeedChanged
        {
            public SpeedChanged(float speedBefore, float speedAfter, float initialSpeed)
            {
                this.SpeedBefore = speedBefore;
                this.SpeedAfter = speedAfter;
                this.InitialSpeed = initialSpeed;
            }

            public float SpeedBefore { get; private set; }

            public float SpeedAfter { get; private set; }

            public float InitialSpeed { get; private set; }
        }
    }
}