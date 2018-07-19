using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Utilities;
using Zenject;

namespace Spawn
{
    public class SpawnManager : IInitializable
    {
        private const string SpawnPointTag = "Respawn";

        private readonly Queue<GameObject> spawnQueue;

        private readonly IEnemyGenerator enemyGenerator;

        private readonly Transform[] spawnPoints;

        public SpawnManager(IEnemyGenerator enemyGenerator, CoroutinesWrapper coroutinesWrapper)
        {
            this.spawnPoints = GameObject.FindGameObjectsWithTag(SpawnPointTag).Select(g => g.transform).ToArray();
            this.spawnQueue = new Queue<GameObject>();
            coroutinesWrapper.StartCoroutine(this.WaitForSpawn());

            this.enemyGenerator = enemyGenerator;
        }

        public void Initialize()
        {
            enemyGenerator.SetQueue(this.spawnQueue);
        }

        private IEnumerator WaitForSpawn()
        {
            while (true)
            {
                if (this.spawnQueue.Count == 0)
                {
                    yield return new WaitForSeconds(1f);
                    continue;
                }

                var spawned = this.spawnQueue.Dequeue();
                var spawnPoint = this.spawnPoints[Random.Range(0, this.spawnPoints.Length)];

                var navMeshAgent = spawned.GetComponent<NavMeshAgent>();
                navMeshAgent.Warp(spawnPoint.position);
                navMeshAgent.gameObject.SetActive(true);
            }
        }
    }
}

