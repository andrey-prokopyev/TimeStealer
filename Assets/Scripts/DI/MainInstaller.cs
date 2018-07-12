using System.Collections.Generic;
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
            this.Container.Bind<ITargetPicker>().To<PlayerPicker>().AsSingle();

            var poolSize = this.gameSettings.WaveConfigurations.Max(wc => wc.MaxEnemies);
            var enemyPrefab = this.gameSettings.WaveConfigurations.First(wc => wc.EnemyPrefab != null).EnemyPrefab;

            //TODO : change concrete type to config prefab
            this.Container.BindFactory<Pursuer, Pursuer.Factory>().FromPoolableMemoryPool<Pursuer, Pursuer.Pool>(
                binder => binder.WithInitialSize(poolSize).FromComponentInNewPrefab(enemyPrefab)
                    .UnderTransformGroup("Enemies"));
            
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
            this.Container.DeclareSignal<EnemyState.EnemyHealthChanged>();
            this.Container.DeclareSignal<PlayerController.Movement>();
            this.Container.DeclareSignal<WeaponCharger.ChargingWeapon>();
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
    }
}