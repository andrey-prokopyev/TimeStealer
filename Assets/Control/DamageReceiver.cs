using System.Collections.Generic;
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
        public void Construct(IDictionary<string, IDamageTaker> damageTakers)
        {
            this.damageTaker = damageTakers[DamageTakerTag];
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(DamageTakerTag))
            {
                var damager = other.GetComponent<Damager>();
                Assert.IsNotNull(damager, string.Format("Damager taker {0} should contain Damager component", other.gameObject.name));

                this.damageTaker.TakeDamage(damager.Damage, this.gameObject.name);
            }
        }
    }
}
