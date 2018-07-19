using System;
using Control;
using Enemies;
using Weapon;
using Zenject;

namespace DI
{
    public class DamageTakerFactory : IFactory<string, IDamageTaker>
    {
        private readonly DiContainer container;

        public DamageTakerFactory(DiContainer container)
        {
            this.container = container;
        }

        public IDamageTaker Create(string tag)
        {
            if (tag == "Bullet")
            {
                return this.container.Instantiate<EnemyState>();
            }

            if (tag == "Enemy")
            {
                return this.container.Resolve<PlayerState>();
            }

            throw new Exception(string.Format("Unknown damage taker tag '{0}'", tag));
        }
    }
}