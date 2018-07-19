using System.Collections;
using Configuration;
using Enemies;
using UnityEngine;
using Utilities;
using Zenject;

namespace Spawn
{
    public class WaveManager : IInitializable
    {
        private readonly IEnemyGenerator enemyGenerator;

        private int currentWaveNumber = -1;

        private readonly WaveConfiguration[] waveConfigurations;

        private readonly CoroutinesWrapper coroutinesWrapper;

        private int enemiesAlive;

        public WaveManager(IEnemyGenerator enemyGenerator, WaveConfiguration[] waveConfigurations,
            CoroutinesWrapper coroutinesWrapper, SignalBus bus)
        {
            this.enemyGenerator = enemyGenerator;
            this.waveConfigurations = waveConfigurations;
            this.coroutinesWrapper = coroutinesWrapper;

            bus.Subscribe<EnemyState.EnemyHealthChanged>(this.OnEnemyHealthChanged);
        }

        public void Initialize()
        {
            this.coroutinesWrapper.StartCoroutine(this.StartNextWave());
        }

        private void OnEnemyHealthChanged(EnemyState.EnemyHealthChanged healthChanged)
        {
            if (healthChanged.Killed)
            {
                this.enemiesAlive--;

                this.CheckWaveFinished();
            }
        }

        private void CheckWaveFinished()
        {
            if (this.enemiesAlive == 0)
            {
                Debug.LogFormat("Wave {0} cleared", this.currentWaveNumber);

                this.coroutinesWrapper.StartCoroutine(this.StartNextWave());
            }
        }

        private IEnumerator StartNextWave()
        {
            this.currentWaveNumber++;

            if (this.waveConfigurations.Length <= this.currentWaveNumber)
            {
                Debug.LogFormat("Wave {0} does not exist. Stopping enemy generation", this.currentWaveNumber);
                yield break;
            }

            var currentWave = this.waveConfigurations[this.currentWaveNumber];
            yield return new WaitForSeconds(currentWave.TimeBeforeWave);

            this.enemiesAlive = currentWave.MaxEnemies;

            Debug.LogFormat("Wave {0} started!", this.currentWaveNumber);

            this.enemyGenerator.SetCurrentWave(currentWave, this.currentWaveNumber);
        }
    }
}