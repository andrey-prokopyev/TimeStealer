using Control;
using UnityEngine;
using UnityEngine.UI;
using Weapon;
using Zenject;

namespace Hud
{
    public class PlayerStateDisplay : MonoBehaviour
    {
        public Text Health;

        public Text ChargeLeft;

        public Text WeaponCharge;

        [Inject]
        public void Construct(SignalBus bus)
        {
            bus.Subscribe<PlayerState.PlayerHealthChanged>(this.OnHealthChanged);
            bus.Subscribe<WeaponCharger.ChargeLeftChanged>(this.OnChargeLeftChanged);
            bus.Subscribe<WeaponCharger.WeaponChargeChanged>(this.OnWeaponChargeChanged);
        }

        private void OnHealthChanged(PlayerState.PlayerHealthChanged playerHealthChanged)
        {
            this.Health.text = string.Format("{0:F1} / {1:F1}", playerHealthChanged.HealthAfter, playerHealthChanged.InitialHealth);
        }

        private void OnChargeLeftChanged(WeaponCharger.ChargeLeftChanged chargeLeftChanged)
        {
            this.ChargeLeft.text = string.Format("{0:F1} / {1:F1}", chargeLeftChanged.ChargeLeft, chargeLeftChanged.InitialCharge);
        }

        private void OnWeaponChargeChanged(WeaponCharger.WeaponChargeChanged weaponChargeChanged)
        {
            this.WeaponCharge.text = string.Format("{0:F1} / {1:F1}", weaponChargeChanged.Charge.Current, weaponChargeChanged.Charge.Max);
        }
    }
}
