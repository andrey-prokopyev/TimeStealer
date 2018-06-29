using System.Linq;
using Configuration;
using Control;
using Enemies;
using Spawn;
using Targeting;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace DI
{
    public class MainInstaller : MonoInstaller
    {
        [Inject]
        private Settings gameSettings;

        public override void InstallBindings()
        {
            this.Container.Bind<ITargetPicker>().To<PlayerPicker>().AsSingle();

            var poolSize = this.gameSettings.WaveConfigurations.Max(wc => wc.MaxEnemies);
            var enemyPrefab = this.gameSettings.WaveConfigurations.First(wc => wc.EnemyPrefab != null).EnemyPrefab;

            //TODO : change concrete type to config prefab
            this.Container.BindMemoryPool<Pursuer, Pursuer.Pool>().WithInitialSize(poolSize).FromComponentInNewPrefab(enemyPrefab).UnderTransformGroup("Enemies");

            this.Container.BindInterfacesAndSelfTo<DefaultEnemyGenerator>().AsSingle();

            this.InstallSignals();
        }

        private void InstallSignals()
        {
            SignalBusInstaller.Install(this.Container);

            this.Container.DeclareSignal<PlayerState.HealthChanged>();
        }
    }
}