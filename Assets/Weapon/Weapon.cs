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

        public Weapon(WeaponCharger weaponCharger, PlayerState playerState, Bullet.Factory bulletFactory, Settings weaponSettings, Transform weaponTransform)
        {
            this.weaponCharger = weaponCharger;
            this.playerState = playerState;
            this.bulletFactory = bulletFactory;
            this.weaponSettings = weaponSettings;
            this.weaponTransform = weaponTransform;
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
                if (this.weaponCharger.StopCharging())
                {
                    this.Fire();
                }
            }
        }

        private void Fire()
        {
            var bullet = this.bulletFactory.Create(this.weaponSettings.BulletSpeed, this.weaponCharger.CurrentCharge,
                this.playerState.LookDirection, this.weaponTransform.position);

            Debug.LogFormat("Fired {0} to direction {1}", bullet, this.playerState.LookDirection);
        }

        [Serializable]
        public class Settings
        {
            public float BulletSpeed;
            public GameObject BulletPrefab;
        }
    }
}