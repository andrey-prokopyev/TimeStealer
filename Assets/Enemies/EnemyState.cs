using System;
using Weapon;
using Zenject;

namespace Enemies
{
    public class EnemyState : IDamageTaker
    {
        private readonly SignalBus signalBus;

        private float health;

        public EnemyState(SignalBus signalBus, Settings settings)
        {
            this.signalBus = signalBus;
            this.health = settings.Health;
        }

        public float TakeDamage(float damage, string damagerReceiverName)
        {
            var healthBefore = this.health;
            var damageLeft = 0f;

            if (damage > this.health)
            {
                damageLeft = damage - this.health;
                this.health = 0f;
            }
            else
            {
                this.health -= damage;
            }

            this.signalBus.Fire(new EnemyHealthChanged(damagerReceiverName, healthBefore, this.health));

            return damageLeft;
        }

        [Serializable]
        public class Settings
        {
            public float Health;
        }

        public class EnemyHealthChanged
        {
            public EnemyHealthChanged(string name, float healthBefore, float healthAfter)
            {
                this.Name = name;
                this.HealthBefore = healthBefore;
                this.HealthAfter = healthAfter;
            }

            public string Name { get; private set; }

            public float HealthBefore { get; private set; }

            public float HealthAfter { get; private set; }
        }
    }
}