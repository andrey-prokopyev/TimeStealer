using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Configuration;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Spawn
{
    public class SpawnManager : MonoBehaviour
    {
        private const string SpawnPointTag = "Respawn";

        private readonly Queue<GameObject> spawnQueue = new Queue<GameObject>();

        private Transform[] spawnPoints;

        private IEnemyGenerator enemyGenerator;

        private Settings gameSettings;

        private int currentWave = -1;
        
        [Inject]
        public void Construct(IEnemyGenerator enemyGenerator, Settings gameSettings)
        {
            this.spawnPoints = GameObject.FindGameObjectsWithTag(SpawnPointTag).Select(g => g.transform).ToArray();
            this.enemyGenerator = enemyGenerator;
            this.gameSettings = gameSettings;

            this.StartNextWave();

            this.enemyGenerator.SetQueue(this.spawnQueue);
            this.StartCoroutine(this.WaitForSpawn());
        }

        private void StartNextWave()
        {
            this.currentWave++;
            this.enemyGenerator.SetCurrentWave(this.gameSettings.WaveConfigurations[this.currentWave]);
        }

        private IEnumerator WaitForSpawn()
        {
            while (true)
            {
                if (spawnQueue.Count == 0)
                {
                    yield return new WaitForSeconds(1f);
                    continue;
                }

                var spawned = spawnQueue.Dequeue();
                var spawnPoint = spawnPoints[Random.Range(0, this.spawnPoints.Length)];

                var navMeshAgent = spawned.GetComponent<NavMeshAgent>();
                navMeshAgent.Warp(spawnPoint.position);
                navMeshAgent.gameObject.SetActive(true);
            }
        }
    }
}

