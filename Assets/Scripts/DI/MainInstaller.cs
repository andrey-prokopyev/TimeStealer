using System.Linq;
using Configuration;
using Control;
using Enemies;
using Spawn;
using Targeting;
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
            this.InstallTargetPickers();
            this.InstallEnemyPrefabs();
            this.InstallSignals();
            this.InstallSpawnManagement();

            this.Container.BindInterfacesAndSelfTo<PlayerState>().AsSingle();
            this.Container.BindInterfacesAndSelfTo<EnemyState>().AsTransient();
            this.Container.Bind<IFactory<string, IDamageTaker>>().To<DamageTakerFactory>().AsSingle();
            this.Container.BindInterfacesAndSelfTo<WaveManager>().AsSingle();
            this.Container.Bind<CoroutinesWrapper>().FromNewComponentOnNewGameObject().AsSingle();
        }

        private void InstallSpawnManagement()
        {
            this.Container.BindInterfacesAndSelfTo<SpawnManager>().AsSingle();
            this.Container.BindInterfacesAndSelfTo<DefaultEnemyGenerator>().AsSingle();
        }

        private void InstallTargetPickers()
        {
            this.Container.Bind<ITargetPicker>().To<PlayerPicker>().AsSingle().WhenInjectedExactlyInto<Pursuer>();
            this.Container.Bind<ITargetPicker>().To<PlayerAdvanceTargetPicker>().AsSingle().WhenInjectedExactlyInto<PlayerAdvancedPursuer>();
        }

        private void InstallEnemyPrefabs()
        {
            var poolSize = this.gameSettings.WaveConfigurations.Max(wc => wc.MaxEnemies);
            this.InstallEnemyPrefab<Pursuer, Pursuer.Factory, Pursuer.Pool>(poolSize);
            this.InstallEnemyPrefab<PlayerAdvancedPursuer, PlayerAdvancedPursuer.Factory, PlayerAdvancedPursuer.Pool>(poolSize);
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
            this.Container.DeclareSignal<Pursuer.PursuerSpawned>();
        }

        private void InstallEnemyPrefab<TEnemy, TFactory, TPool>(int poolSize) where TFactory : PlaceholderFactory<TEnemy> where TPool : MemoryPool<IMemoryPool, TEnemy> where TEnemy : IPoolable<IMemoryPool>
        {
            this.Container.BindFactory<TEnemy, TFactory>().FromPoolableMemoryPool<TEnemy, TPool>(
                binder => binder.WithInitialSize(poolSize).FromComponentInNewPrefab(this.gameSettings.EnemyPrefabConfiguration.PrefabDictionary[typeof(TEnemy).Name])
                    .UnderTransformGroup("Enemies"));
        }
    }
}