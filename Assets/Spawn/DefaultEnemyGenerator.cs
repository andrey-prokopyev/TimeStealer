using System;
using System.Collections.Generic;
using Configuration;
using Enemies;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Spawn
{
    public class DefaultEnemyGenerator : IEnemyGenerator, ITickable
    {
        private Queue<GameObject> spawnQueue;

        private float timeBetweenGenerations = 0f;

        private float nextGenerationTime = float.MaxValue;

        private int enemiesLeft;

        private WaveConfiguration currentWave;

        private readonly Pursuer.Pool pool;

        public DefaultEnemyGenerator(Pursuer.Pool pool)
        {
            this.pool = pool;
        }

        public void SetCurrentWave(WaveConfiguration wave)
        {
            this.currentWave = wave;
            this.enemiesLeft = wave.MaxEnemies;

            this.CalculateNextGenerationTime();
        }

        public void SetQueue(Queue<GameObject> spawnQueue)
        {
            this.spawnQueue = spawnQueue;
        }

        public void Tick()
        {
            if (this.spawnQueue == null)
            {
                return;
            }

            if (this.enemiesLeft <= 0)
            {
                return;
            }

            if (this.timeBetweenGenerations <= this.nextGenerationTime)
            {
                this.timeBetweenGenerations += Time.deltaTime;
                return;
            }

            var enemy = this.pool.Spawn();

            enemy.gameObject.name = "Enemy" + this.enemiesLeft;

            this.spawnQueue.Enqueue(enemy.gameObject);

            this.enemiesLeft--;

            this.CalculateNextGenerationTime();

            Debug.LogFormat("{3}. Spawned '{0}'. {1} enemies left. Next generation in {2}", enemy.gameObject.name, this.enemiesLeft, this.nextGenerationTime, DateTime.Now.ToString("O"));
        }

        private void CalculateNextGenerationTime()
        {
            this.nextGenerationTime = Random.Range(this.currentWave.MinTimeBetweenSpawns, this.currentWave.MaxTimeBetweenSpawns);
            this.timeBetweenGenerations = 0f;
        }
    }
}