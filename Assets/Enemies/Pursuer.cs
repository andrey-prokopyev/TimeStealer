using System;
using System.Collections;
using Targeting;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Enemies
{
    public class Pursuer : MonoBehaviour, IPoolable<IMemoryPool>
    {
        private const float NavThrottlingIntervalSeconds = 1f;

        private ITargetPicker targetPicker;

        private NavMeshAgent navMeshAgent;

        private IMemoryPool pool;

        [Inject]
        public void Construct(ITargetPicker targetPicker, SignalBus signalBus)
        {
            this.targetPicker = targetPicker;
            this.navMeshAgent = this.GetComponent<NavMeshAgent>();

            signalBus.Subscribe<EnemyState.EnemyHealthChanged>(HealthChanged);
        }

        private void HealthChanged(EnemyState.EnemyHealthChanged enemyHealthChanged)
        {
            if (this.gameObject.name != enemyHealthChanged.Name)
            {
                return;
            }

            Debug.LogFormat("{0}'s health changed from {1} to {2}", enemyHealthChanged.Name, enemyHealthChanged.HealthBefore, enemyHealthChanged.HealthAfter);

            if (enemyHealthChanged.HealthAfter <= 0f)
            {
                this.pool.Despawn(this);
            }
        }

        private bool pursuing;
        private void FixedUpdate()
        {
            if (this.pursuing)
            {
                return;
            }

            this.pursuing = true;

            var target = this.targetPicker.PickFor(this.gameObject);

            this.navMeshAgent.SetDestination(target.transform.position);

            Debug.LogFormat("{0} is pursuing {1}", this.name, target.name);
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

                this.navMeshAgent.SetDestination(target.transform.position);

                yield return new WaitForSeconds(NavThrottlingIntervalSeconds);
            }
        }

        public void OnDespawned()
        {
        }

        public void OnSpawned(IMemoryPool pool)
        {
            Debug.LogFormat("Spawned {0}, pull != null = {1}", this.gameObject.name, pool != null);

            this.pool = pool;
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
    }
}