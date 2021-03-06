﻿using Control;
using Enemies;
using UnityEngine;

namespace Targeting
{
    public class PlayerPicker : ITargetPicker
    {
        private readonly PlayerState playerState;

        public PlayerPicker(PlayerState playerState)
        {
            this.playerState = playerState;
        }

        public Vector3 PickFor(GameObject pursuer)
        {
            return this.playerState.Position;
        }
    }
}