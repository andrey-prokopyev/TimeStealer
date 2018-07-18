using System;
using Control;
using UnityEngine;
using Zenject;

namespace Weapon
{
    public class Weapon : ITickable
    {
        private readonly WeaponCharger weaponCharger;

        private readonly PlayerState playerState;

        private readonly Bullet.Factory bulletFactory;

        private readonly Settings weaponSettings;

        private readonly Transform weaponTransform;

        private float bulletSpeed;

        public Weapon(WeaponCharger weaponCharger, PlayerState playerState, Bullet.Factory bulletFactory, Settings weaponSettings, Transform weaponTransform, SignalBus bus)
        {
            this.weaponCharger = weaponCharger;
            this.playerState = playerState;
            this.bulletFactory = bulletFactory;
            this.weaponSettings = weaponSettings;
            this.weaponTransform = weaponTransform;
            this.bulletSpeed = this.weaponSettings.BulletSpeed;

            bus.Subscribe<PlayerState.SpeedChanged>(this.OnPlayerSpeedChanged);
        }

        public void Tick()
        {
            var startCharging = Input.GetAxis("Fire1") > 0;

            if (startCharging)
            {
                this.weaponCharger.StartCharging();
            }
            else
            {
                var charge = this.weaponCharger.StopCharging();

                if (charge.Current > 0f)
                {
                    this.Fire(charge);
                }
            }
        }

        private void Fire(WeaponCharger.Charge charge)
        {
            this.bulletFactory.Create(this.bulletSpeed, charge, this.playerState.LookDirection, this.weaponTransform.position);
        }

        private void OnPlayerSpeedChanged(PlayerState.SpeedChanged speedChanged)
        {
            this.bulletSpeed = this.weaponSettings.BulletSpeed * speedChanged.SpeedAfter / speedChanged.InitialSpeed;
        }

        [Serializable]
        public class Settings
        {
            public float BulletSpeed;
            public GameObject BulletPrefab;
        }
    }
}