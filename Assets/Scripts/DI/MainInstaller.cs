using System.Collections.Generic;
using System.Linq;
using Configuration;
using Control;
using Enemies;
using Spawn;
using Targeting;
using UnityEngine;
using Utilities;
using Weapon;
using Zenject;

namespace DI
{
    public class MainInstaller : MonoInstaller
    {
        [Inject]
        private Settings gameSettings;

        public override void InstallBindings()
        {
            this.Container.Bind<ITargetPicker>().To<PlayerPicker>().AsSingle().WhenInjectedExactlyInto<Pursuer>();
            this.Container.Bind<ITargetPicker>().To<PlayerAdvanceTargetPicker>().AsSingle().WhenInjectedExactlyInto<PlayerAdvancedPursuer>();

            var poolSize = this.gameSettings.WaveConfigurations.Max(wc => wc.MaxEnemies);

            this.InstallEnemyPrefab<Pursuer, Pursuer.Factory, Pursuer.Pool>(poolSize);
            this.InstallEnemyPrefab<PlayerAdvancedPursuer, PlayerAdvancedPursuer.Factory, PlayerAdvancedPursuer.Pool>(poolSize);

            this.Container.BindInterfacesAndSelfTo<PlayerState>().AsSingle();
            this.Container.BindInterfacesAndSelfTo<EnemyState>().AsTransient();

            this.Container.BindInterfacesAndSelfTo<DefaultEnemyGenerator>().AsSingle();

            this.Container.BindInterfacesAndSelfTo<WaveManager>().AsSingle().WithArguments(this.gameSettings.WaveConfigurations);

            this.Container.Bind<CoroutinesWrapper>().FromNewComponentOnNewGameObject().AsSingle();

            this.Container.BindInterfacesAndSelfTo<SpawnManager>().AsSingle();

            this.InstallSignals();
            this.InstallDamageTakers();
        }

        private void InstallSignals()
        {
            SignalBusInstaller.Install(this.Container);
            this.Container.DeclareSignal<PlayerState.PlayerHealthChanged>();
            this.Container.DeclareSignal<PlayerState.SpeedChanged>();
            this.Container.DeclareSignal<EnemyState.EnemyHealthChanged>();
            this.Container.DeclareSignal<PlayerController.Movement>();
            this.Container.DeclareSignal<WeaponCharger.WeaponChargeChanged>();
            this.Container.DeclareSignal<WeaponCharger.ChargeLeftChanged>();
        }

        private void InstallDamageTakers()
        {
            this.Container.Bind<IDictionary<string, IDamageTaker>>().FromMethod(c =>
                new Dictionary<string, IDamageTaker>
                {
                    {"Bullet", c.Container.Resolve<EnemyState>()},
                    {"Enemy", c.Container.Resolve<PlayerState>()}
                }).AsSingle();
        }

        private void InstallEnemyPrefab<TEnemy, TFactory, TPool>(int poolSize) where TFactory : PlaceholderFactory<TEnemy> where TPool : MemoryPool<IMemoryPool, TEnemy> where TEnemy : IPoolable<IMemoryPool>
        {
            this.Container.BindFactory<TEnemy, TFactory>().FromPoolableMemoryPool<TEnemy, TPool>(
                binder => binder.WithInitialSize(poolSize).FromComponentInNewPrefab(this.gameSettings.EnemyPrefabConfiguration.PrefabDictionary[typeof(TEnemy).Name])
                    .UnderTransformGroup("Enemies"));
        }
    }
}