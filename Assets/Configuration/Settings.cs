using System;
using Control;
using Spawn;
using UnityEngine;

namespace Configuration
{
    [Serializable]
    public class Settings
    {
        public WaveConfiguration[] WaveConfigurations;

        public PlayerController.Settings PlayerControlSettings;

        public PlayerState.Settings PlayerStateSettings;
    }
}
