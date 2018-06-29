using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Configuration
{
    [Serializable]
    public class WaveConfiguration
    {
        public float MinTimeBetweenSpawns;

        public float MaxTimeBetweenSpawns;

        public int MaxEnemies;

        public GameObject EnemyPrefab;
    }
}