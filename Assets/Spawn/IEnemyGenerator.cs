using System.Collections.Generic;
using Configuration;
using UnityEngine;

namespace Spawn
{
    public interface IEnemyGenerator
    {
        void SetQueue(Queue<GameObject> spawnQueue);
        void SetCurrentWave(WaveConfiguration wave, int currentWaveNumber);
    }
}