using System;
using Control;
using UnityEngine;
using Zenject;

namespace Weapon
{
    public class WeaponCharger : IFixedTickable, IInitializable
    {
        private const float ThrottleChargeLeftChangedValue = 1f;

        private readonly IWeaponHolder weaponHolder;

        private readonly SignalBus bus;

        private readonly Settings settings;

        private Charge currentCharge = Charge.Zero;

        private float chargeSpeed;

        private float chargeLeft;

        private float throttledChargeLeft;

        private bool chargingWeapon;

        public WeaponCharger(Settings settings, IWeaponHolder weaponHolder, SignalBus bus)
        {
            this.weaponHolder = weaponHolder;
            this.bus = bus;
            this.settings = settings;
            this.chargeLeft = settings.InitialCharge;
            this.chargeSpeed = settings.ChargeSpeed;

            this.bus.Subscribe<PlayerState.SpeedChanged>(this.OnPlayerSpeedChanged);
        }

        private float ChargeLeft
        {
            get { return this.chargeLeft; }
            set
            {
                this.chargeLeft = value;
                this.OnChargeLeftChanged();
            }
        }

        private Charge CurrentCharge
        {
            get { return this.currentCharge; }
            set
            {
                this.currentCharge = value;
                this.OnCurrentChargeChanged();
            }
        }

        public void Initialize()
        {
            this.OnChargeLeftChanged();
        }

        public void StartCharging()
        {
            if (!this.chargingWeapon)
            {
                this.chargingWeapon = true;
                this.weaponHolder.OnHold = true;
            }
        }

        public Charge StopCharging()
        {
            if (this.chargingWeapon)
            {
                this.chargingWeapon = false;
                this.ChargeLeft -= this.CurrentCharge.Current;
                var charge = this.CurrentCharge;
                this.CurrentCharge = Charge.Zero;
                this.weaponHolder.OnHold = false;

                return charge;
            }

            return Charge.Zero;
        }

        public void FixedTick()
        {
            var deltaTime = Time.fixedDeltaTime;

            this.ChargeWeapon(deltaTime);
            this.Recharge(deltaTime);
        }

        private void ChargeWeapon(float deltaTime)
        {
            if (this.chargingWeapon)
            {
                var newCharge = this.CurrentCharge.Current + this.chargeSpeed * deltaTime;

                if (newCharge < Mathf.Min(this.chargeLeft, this.settings.MaxCharge))
                {
                    this.CurrentCharge = new Charge(newCharge, this.settings.MaxCharge);
                }
            }
        }

        private void Recharge(float deltaTime)
        {
            if (this.chargingWeapon)
            {
                return;
            }

            var recharged = this.ChargeLeft + this.settings.RechargeSpeed * deltaTime;

            if (recharged <= this.settings.InitialCharge)
            {
                this.ChargeLeft = recharged;
            }
        }

        private void OnPlayerSpeedChanged(PlayerState.SpeedChanged speedChanged)
        {
            this.chargeSpeed = this.settings.ChargeSpeed * speedChanged.SpeedAfter / speedChanged.InitialSpeed;
        }

        private void OnCurrentChargeChanged()
        {
            this.bus.Fire(new WeaponChargeChanged(this.CurrentCharge));
        }

        private void OnChargeLeftChanged()
        {
            if (Mathf.Abs(this.throttledChargeLeft - this.chargeLeft) > ThrottleChargeLeftChangedValue)
            {
                this.throttledChargeLeft = this.chargeLeft;

                this.bus.Fire(new ChargeLeftChanged(this.throttledChargeLeft, this.settings.InitialCharge));
            }
        }

        public class WeaponChargeChanged
        {
            public WeaponChargeChanged(Charge currentCharge)
            {
                this.Charge = currentCharge;
            }

            public Charge Charge { get; private set; }
        }

        public class ChargeLeftChanged
        {
            public ChargeLeftChanged(float chargeLeft, float initialCharge)
            {
                this.ChargeLeft = chargeLeft;
                this.InitialCharge = initialCharge;
            }

            public float ChargeLeft { get; private set; }

            public float InitialCharge { get; private set; }
        }

        [Serializable]
        public class Settings
        {
            public float InitialCharge;
            public float ChargeSpeed;
            public float MaxCharge;
            public float RechargeSpeed;
        }

        public class Charge
        {
            public static readonly Charge Zero = new Charge(0f, 0f);

            public Charge(float current, float max)
            {
                this.Current = current;
                this.Max = max;
            }

            public float Current { get; private set; }
            public float Max { get; private set; }

            public override string ToString()
            {
                return string.Format("Current: {0}, Max: {1}", this.Current, this.Max);
            }
        }
    }
}