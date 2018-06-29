using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace Control
{
    public class DamageTaker : MonoBehaviour
    {
        private const string EnemyTag = "Enemy";

        private PlayerState playerState;

        [Inject]
        public void Construct(PlayerState playerState)
        {
            this.playerState = playerState;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(EnemyTag))
            {
                var damager = other.GetComponent<Damager>();
                Assert.IsNotNull(damager, "Enemy should contain Damager component");

                this.playerState.TakeDamage(damager.Damage);
            }
        }
    }
}
