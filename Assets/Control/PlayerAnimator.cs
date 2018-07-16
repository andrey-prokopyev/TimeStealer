using Control;
using UnityEngine;
using Weapon;
using Zenject;

public class PlayerAnimator : IInitializable
{
    private readonly Animator animator;

    private readonly SignalBus bus;

    private string currentState = PlayerAnimationState.Idle;

    public PlayerAnimator(Animator animator, SignalBus bus)
    {
        this.animator = animator;
        this.bus = bus;
    }

    public void Initialize()
    {
        this.bus.Subscribe<PlayerController.Movement>(this.OnPlayerMoved);
        this.bus.Subscribe<PlayerState.SpeedChanged>(this.OnPlayerSpeedChanged);
        this.bus.Subscribe<WeaponCharger.ChargingWeapon>(this.OnPlayerCharging);
    }

    private void OnPlayerMoved(PlayerController.Movement movement)
    {
        var newState = movement.Direction.sqrMagnitude > 0 ? PlayerAnimationState.Moving : PlayerAnimationState.Idle;
        this.UpdateStateIfChanged(newState);
    }

    private void OnPlayerSpeedChanged(PlayerState.SpeedChanged speedChanged)
    {
        this.animator.speed = speedChanged.SpeedAfter / speedChanged.InitialSpeed;
    }

    private void OnPlayerCharging(WeaponCharger.ChargingWeapon weaponCharge)
    {
        this.UpdateStateIfChanged(PlayerAnimationState.Charging);
    }

    private void UpdateStateIfChanged(string newState)
    {
        if (this.currentState != newState)
        {
            this.currentState = newState;
            this.animator.SetTrigger(this.currentState);
        }
    }

    private class PlayerAnimationState
    {
        public const string Idle = "idle";
        public const string Moving = "run";
        public const string Charging = "cast magic";
    }
}
