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
        private readonly PlaceholderFactory<Pursuer> pursuerFactory;

        private readonly PlaceholderFactory<PlayerAdvancedPursuer> advancedPursuerFactory;

        private Queue<GameObject> spawnQueue;

        private float timeBetweenGenerations;

        private float nextGenerationTime = float.MaxValue;

        private int enemiesLeft;

        private WaveConfiguration currentWave;

        private int currentWaveNumber;

        public DefaultEnemyGenerator(Pursuer.Factory pursuerFactory, PlayerAdvancedPursuer.Factory advancedPursuerFactory)
        {
            this.pursuerFactory = pursuerFactory;
            this.advancedPursuerFactory = advancedPursuerFactory;
        }

        public void SetCurrentWave(WaveConfiguration wave, int currentWaveNumber)
        {
            this.currentWave = wave;
            this.enemiesLeft = wave.MaxEnemies;
            this.currentWaveNumber = currentWaveNumber;

            this.CalculateNextGenerationTime();
        }

        public void SetQueue(Queue<GameObject> spawnQueue)
        {
            this.spawnQueue = spawnQueue;
        }

        public void Tick()
        {
            if (!this.IsReadyToGenerate())
            {
                return;
            }

            var enemy = this.Create(this.currentWave.EnemyType);

            enemy.gameObject.name = string.Format("Enemy-{0}-{1}-{2}", this.currentWave.EnemyType, this.currentWaveNumber, this.enemiesLeft);

            this.spawnQueue.Enqueue(enemy.gameObject);

            this.enemiesLeft--;

            this.CalculateNextGenerationTime();

            Debug.LogFormat("Spawned '{0}'. {1} enemies left. Next generation in {2}", enemy.gameObject.name, this.enemiesLeft, this.nextGenerationTime);
        }

        private MonoBehaviour Create(string enemyType)
        {
            // TODO: remove ifs
            if (enemyType == typeof(Pursuer).Name)
            {
                return this.pursuerFactory.Create();
            }

            if (enemyType == typeof(PlayerAdvancedPursuer).Name)
            {
                return this.advancedPursuerFactory.Create();
            }

            throw new ArgumentException(string.Format("Unknown enemy type '{0}'", enemyType ?? "null"), "enemyType");
        }

        private bool IsReadyToGenerate()
        {
            if (this.spawnQueue == null)
            {
                return false;
            }

            if (this.enemiesLeft <= 0)
            {
                return false;
            }

            if (this.timeBetweenGenerations <= this.nextGenerationTime)
            {
                this.timeBetweenGenerations += Time.deltaTime;
                return false;
            }

            return true;
        }

        private void CalculateNextGenerationTime()
        {
            this.nextGenerationTime = Random.Range(this.currentWave.MinTimeBetweenSpawns, this.currentWave.MaxTimeBetweenSpawns);
            this.timeBetweenGenerations = 0f;
        }
    }
}