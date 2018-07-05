using System;
using UnityEngine;
using Zenject;

namespace Weapon
{
    public class WeaponCharger : IFixedTickable
    {
        private float chargeLeft;

        private float chargeSpeed;

        private bool charging;

        public WeaponCharger(Settings settings)
        {
            this.chargeLeft = settings.InitialCharge;
            this.chargeSpeed = settings.ChargeSpeed;
        }

        public float CurrentCharge { get; private set; }

        public void StartCharging()
        {
            if (!this.charging)
            {
                this.CurrentCharge = 0f;
                this.charging = true;
            }
        }

        public bool StopCharging()
        {
            if (this.charging)
            {
                this.charging = false;
                this.chargeLeft -= this.CurrentCharge;
                Debug.LogFormat("After charging stop chargeLeft = {0}", this.chargeLeft);

                return true;
            }

            return false;
        }

        public void FixedTick()
        {
            if (this.charging)
            {
                var newCharge = this.CurrentCharge + this.chargeSpeed * Time.fixedDeltaTime;

                if (newCharge < chargeLeft)
                {
                    this.CurrentCharge = newCharge;
                }
            }
        }

        [Serializable]
        public class Settings
        {
            public float InitialCharge;
            public float ChargeSpeed;
        }
    }
}