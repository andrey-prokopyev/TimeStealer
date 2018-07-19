using Configuration;
using UnityEngine;
using Zenject;

namespace DI
{
    [CreateAssetMenu(menuName = "TimeStealer/Game Settings")]
    public class SettingsInstaller : ScriptableObjectInstaller<SettingsInstaller>
    {
        public Settings GameSettings;

        public override void InstallBindings()
        {
            this.Container.BindInstance(this.GameSettings);

            this.Container.BindInstance(this.GameSettings.PlayerStateSettings);

            this.Container.BindInstance(this.GameSettings.EnemyStateSettings);

            this.Container.BindInstance(this.GameSettings.ChargeSettings);

            this.Container.BindInstance(this.GameSettings.WaveConfigurations);

            this.Container.BindInstance(this.GameSettings.WeaponSettings);
        }
    }
}