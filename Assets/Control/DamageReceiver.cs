using Enemies;
using UnityEngine;
using UnityEngine.Assertions;
using Weapon;
using Zenject;

namespace Control
{
    public class DamageReceiver : MonoBehaviour
    {
        public string DamageTakerTag;

        private IDamageTaker damageTaker;

        [Inject]
        public void Construct(IFactory<string, IDamageTaker> damageTakerFactory, SignalBus bus)
        {
            this.damageTaker = damageTakerFactory.Create(this.DamageTakerTag);
            bus.Subscribe<Pursuer.PursuerSpawned>(this.OnEnemySpawned);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(this.DamageTakerTag))
            {
                Debug.LogFormat("{0} received damage from {1}", this.gameObject.name, other.name);

                var damager = other.GetComponent<Damager>();
                Assert.IsNotNull(damager, string.Format("Damager taker {0} should contain Damager component", other.gameObject.name));

                this.damageTaker.TakeDamage(damager.Damage, this.gameObject.name);
            }
        }

        private void OnEnemySpawned(Pursuer.PursuerSpawned enemySpawned)
        {
            if (enemySpawned.Name == this.gameObject.name)
            {
                this.damageTaker.Reinitialize();
            }
        }
    }
}
