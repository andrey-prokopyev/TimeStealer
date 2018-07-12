using System;
using UnityEngine;
using Weapon;
using Zenject;

namespace Control
{
    public class PlayerState : IDamageTaker, IWeaponHolder
    {
        private readonly SignalBus signalBus;

        private float health;

        public PlayerState(Settings settings, SignalBus signalBus)
        {
            this.signalBus = signalBus;
            this.health = settings.Health;
        }

        public Vector3 LookDirection { get; set; }

        public bool OnHold { get; set; }

        public float TakeDamage(float damage, string damagerReceiverName)
        {
            var healthBefore = this.health;
            this.health -= damage;

            this.signalBus.Fire(new PlayerHealthChanged(healthBefore, this.health));

            return 0f;
        }

        [Serializable]
        public class Settings
        {
            public float Health;
        }

        public class PlayerHealthChanged
        {
            public PlayerHealthChanged(float healthBefore, float healthAfter)
            {
                HealthBefore = healthBefore;
                HealthAfter = healthAfter;
            }

            public float HealthBefore { get; private set; }

            public float HealthAfter { get; private set; }
        }
    }
}