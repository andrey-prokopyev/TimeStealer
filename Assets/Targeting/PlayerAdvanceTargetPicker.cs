using Control;
using UnityEngine;

namespace Targeting
{
    public class PlayerAdvanceTargetPicker : ITargetPicker
    {
        private readonly PlayerState playerState;

        public PlayerAdvanceTargetPicker(PlayerState playerState)
        {
            this.playerState = playerState;
        }

        public Vector3 PickFor(GameObject pursuer)
        {
            return this.playerState.Position + this.playerState.MoveDirection.normalized * this.playerState.Speed;
        }
    }
}