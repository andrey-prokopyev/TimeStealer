using System;
using Configuration;
using UnityEngine;
using Weapon;
using Zenject;

namespace DI
{
    public class WeaponInstaller : MonoInstaller
    {
        [SerializeField]
        private Components components;

        [Inject]
        private Weapon.Weapon.Settings gameSettings;

        public override void InstallBindings()
        {
            this.Container.BindInterfacesAndSelfTo<Weapon.Weapon>().AsSingle()
                .WithArguments(this.gameSettings, this.components.WeaponTransform);

            this.Container.BindInterfacesAndSelfTo<WeaponCharger>().AsSingle();
            
            this.Container.BindFactory<float, WeaponCharger.Charge, Vector3, Vector3, Bullet, Bullet.Factory>()
                .FromPoolableMemoryPool<Bullet, Bullet.Pool>(binder =>
                    binder.WithInitialSize(10).FromComponentInNewPrefab(this.gameSettings.BulletPrefab)
                        .UnderTransform(this.components.BulletsGroup));
        }

        [Serializable]
        public class Components
        {
            public Transform WeaponTransform;
            public Transform BulletsGroup;
        }
    }
}