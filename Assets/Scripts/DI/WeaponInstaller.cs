using System;
using Configuration;
using Control;
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
        private Settings gameSettings;

        public override void InstallBindings()
        {
            this.Container.BindInterfacesAndSelfTo<Weapon.Weapon>().AsSingle()
                .WithArguments(this.gameSettings.WeaponSettings, this.components.WeaponTransform);

            this.Container.BindInterfacesAndSelfTo<WeaponCharger>().FromMethod(s => new WeaponCharger(this.gameSettings.ChargeSettings)).AsSingle();

            this.Container.BindMemoryPool<Bullet, Bullet.Pool>().WithInitialSize(10).FromComponentInNewPrefab(gameSettings.WeaponSettings.BulletPrefab).UnderTransformGroup("Bullets");

            this.Container.BindFactory<float, float, Vector3, Vector3, Bullet, Bullet.Factory>()
                .FromPoolableMemoryPool<Bullet, Bullet.Pool>(binder =>
                    binder.WithInitialSize(10).FromComponentInNewPrefab(gameSettings.WeaponSettings.BulletPrefab)
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