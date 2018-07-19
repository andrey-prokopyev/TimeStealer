using System.Collections;
using Targeting;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Enemies
{
    public class Pursuer : MonoBehaviour, IPoolable<IMemoryPool>
    {
        private const float NavThrottlingInterval = 1f;

        private ITargetPicker targetPicker;

        private NavMeshAgent navMeshAgent;

        private IMemoryPool pool;

        private SignalBus bus;

        [Inject]
        public void Construct(ITargetPicker targetPicker, SignalBus bus)
        {
            this.targetPicker = targetPicker;
            this.bus = bus;
            this.navMeshAgent = this.GetComponent<NavMeshAgent>();

            bus.Subscribe<EnemyState.EnemyHealthChanged>(this.HealthChanged);
        }

        private void HealthChanged(EnemyState.EnemyHealthChanged enemyHealthChanged)
        {
            if (this.gameObject.name != enemyHealthChanged.Name)
            {
                return;
            }

            Debug.LogFormat("{0}'s health changed from {1} to {2}", enemyHealthChanged.Name, enemyHealthChanged.HealthBefore, enemyHealthChanged.HealthAfter);

            if (enemyHealthChanged.Killed)
            {
                this.pool.Despawn(this);
            }
        }

        private void OnEnable()
        {
            this.StartCoroutine(this.Pursue());
        }

        private IEnumerator Pursue()
        {
            while (true)
            {
                var target = this.targetPicker.PickFor(this.gameObject);

                this.navMeshAgent.SetDestination(target);

                yield return new WaitForSeconds(NavThrottlingInterval);
            }
        }

        public void OnDespawned()
        {
        }

        public void OnSpawned(IMemoryPool pool)
        {
            Debug.LogFormat("Spawned {0}, pull != null = {1}", this.gameObject.name, pool != null);

            this.pool = pool;

            this.bus.Fire(new PursuerSpawned(this.gameObject.name));
        }

        public class Factory : PlaceholderFactory<Pursuer>
        {
        }

        public class Pool : MonoPoolableMemoryPool<IMemoryPool, Pursuer>
        {
            protected override void OnSpawned(Pursuer item)
            {
                base.OnSpawned(item);
                item.gameObject.SetActive(false);
            }
        }

        public class PursuerSpawned
        {
            public PursuerSpawned(string name)
            {
                this.Name = name;
            }

            public string Name { get; private set; }
        }
    }
}