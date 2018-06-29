using System;
using System.Collections;
using Targeting;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Enemies
{
    public class Pursuer : MonoBehaviour
    {
        private const float NavThrottlingIntervalSeconds = 1f;

        private ITargetPicker targetPicker;

        private NavMeshAgent navMeshAgent;

        [Inject]
        public void Construct(ITargetPicker targetPicker)
        {
            this.targetPicker = targetPicker;
            this.navMeshAgent = this.GetComponent<NavMeshAgent>();
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

        public class Pool : MonoMemoryPool<Pursuer>
        {
            protected override void OnSpawned(Pursuer item)
            {
                base.OnSpawned(item);
                item.gameObject.SetActive(false);
            }
        }
    }
}