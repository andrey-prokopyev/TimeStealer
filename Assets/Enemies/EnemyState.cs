using System;
using Weapon;
using Zenject;

namespace Enemies
{
    public class EnemyState : IDamageTaker
    {
        private readonly SignalBus bus;

        private readonly Settings settings;

        private float health;

        public EnemyState(SignalBus bus, Settings settings)
        {
            this.bus = bus;
            this.settings = settings;
        }

        public void TakeDamage(float damage, string damagerReceiverName)
        {
            var healthBefore = this.health;

            if (damage >= this.health)
            {
                this.health = 0f;
            }
            else
            {
                this.health -= damage;
            }

            this.bus.Fire(new EnemyHealthChanged(damagerReceiverName, healthBefore, this.health));
        }

        public void Reinitialize()
        {
            this.health = this.settings.Health;
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

            public bool Killed
            {
                get { return this.HealthAfter <= 0f; }
            }
        }
    }
}