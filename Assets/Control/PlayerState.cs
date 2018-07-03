using System;
using UnityEngine;
using Zenject;

namespace Control
{
    public class PlayerState
    {
        private readonly SignalBus signalBus;

        private float health;

        public PlayerState(Settings settings, SignalBus signalBus)
        {
            this.signalBus = signalBus;
            this.health = settings.Health;
        }

        public Vector3 LookDirection { get; set; }

        public void TakeDamage(float damage)
        {
            var healthBefore = this.health;
            this.health -= damage;

            this.signalBus.Fire(new HealthChanged(healthBefore, this.health));
        }

        [Serializable]
        public class Settings
        {
            public float Health;
        }

        public class HealthChanged
        {
            public HealthChanged(float healthBefore, float healthAfter)
            {
                HealthBefore = healthBefore;
                HealthAfter = healthAfter;
            }

            public float HealthBefore { get; private set; }

            public float HealthAfter { get; private set; }
        }
    }
}