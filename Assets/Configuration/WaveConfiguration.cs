using System;

namespace Configuration
{
    [Serializable]
    public class WaveConfiguration
    {
        public float MinTimeBetweenSpawns;

        public float MaxTimeBetweenSpawns;

        public int MaxEnemies;

        public float TimeBeforeWave;

        public string EnemyType;
    }
}