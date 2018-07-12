﻿using System;
using Control;
using UnityEngine;
using Zenject;

namespace Weapon
{
    public class WeaponCharger : IFixedTickable
    {
        private readonly IWeaponHolder weaponHolder;

        private readonly SignalBus bus;

        private readonly float chargeSpeed;

        private readonly float maxCharge;

        private float chargeLeft;

        private bool charging;

        public WeaponCharger(Settings settings, IWeaponHolder weaponHolder, SignalBus bus)
        {
            this.weaponHolder = weaponHolder;
            this.bus = bus;
            this.chargeLeft = settings.InitialCharge;
            this.chargeSpeed = settings.ChargeSpeed;
            this.maxCharge = settings.MaxCharge;
        }

        public Charge CurrentCharge { get; private set; }

        public bool IsCharged
        {
            get { return this.CurrentCharge.Current > 0f; }
        }

        public void StartCharging()
        {
            if (!this.charging)
            {
                this.CurrentCharge = Charge.Zero;
                this.charging = true;

                this.weaponHolder.OnHold = true;
            }
        }

        public bool StopCharging()
        {
            if (this.charging)
            {
                this.charging = false;
                this.chargeLeft -= this.CurrentCharge.Current;
                Debug.LogFormat("After charging stop chargeLeft = {0}", this.chargeLeft);

                this.weaponHolder.OnHold = false;

                return true;
            }

            return false;
        }

        public void FixedTick()
        {
            if (this.charging)
            {
                var newCharge = this.CurrentCharge.Current + this.chargeSpeed * Time.fixedDeltaTime;

                if (newCharge < Mathf.Min(chargeLeft, this.maxCharge))
                {
                    this.CurrentCharge = new Charge(newCharge, this.maxCharge);

                    this.bus.Fire(new ChargingWeapon(this.CurrentCharge));
                }
            }
        }

        public class ChargingWeapon
        {
            public ChargingWeapon(Charge currentCharge)
            {
                this.Charge = currentCharge;
            }

            public Charge Charge { get; private set; }
        }

        [Serializable]
        public class Settings
        {
            public float InitialCharge;
            public float ChargeSpeed;
            public float MaxCharge;
        }

        public class Charge
        {
            public static readonly Charge Zero = new Charge(0f, 0f);

            public Charge(float current, float max)
            {
                Current = current;
                Max = max;
            }

            public float Current { get; private set; }
            public float Max { get; private set; }

            public override string ToString()
            {
                return string.Format("Current: {0}, Max: {1}", Current, Max);
            }
        }
    }
}