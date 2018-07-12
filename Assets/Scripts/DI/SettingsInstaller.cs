using System.Collections.Generic;
using Configuration;
using Control;
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
        }
    }
}