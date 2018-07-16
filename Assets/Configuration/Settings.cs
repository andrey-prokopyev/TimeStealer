using System;
using Control;
using Enemies;

namespace Configuration
{
    [Serializable]
    public class Settings
    {
        public WaveConfiguration[] WaveConfigurations;

        public PlayerState.Settings PlayerStateSettings;

        public Weapon.Weapon.Settings WeaponSettings;

        public Weapon.WeaponCharger.Settings ChargeSettings;

        public EnemyState.Settings EnemyStateSettings;
    }
}
