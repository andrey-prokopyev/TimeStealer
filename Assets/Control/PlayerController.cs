using System;
using UnityEngine;
using Zenject;

namespace Control
{
    public class PlayerController : ITickable
    {
        private readonly CharacterController controller;

        private readonly Transform viewPort;

        private readonly Settings settings;

        private readonly PlayerState playerState;

        private readonly SignalBus bus;

        public PlayerController(CharacterController controller, Transform viewPort, Settings settings, PlayerState playerState, SignalBus bus)
        {
            this.controller = controller;
            this.settings = settings;
            this.playerState = playerState;
            this.viewPort = viewPort;
            this.bus = bus;
        }

        public void Tick()
        {
            if (!this.playerState.OnHold)
            {
                var input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * this.settings.MoveSpeed;

                var moveSpeed = this.TransformInputToViewPort(input);

                this.controller.SimpleMove(moveSpeed);
                this.controller.transform.rotation = Quaternion.LookRotation(this.playerState.LookDirection);

                this.bus.Fire(new Movement(moveSpeed));
            }
        }

        private Vector3 TransformInputToViewPort(Vector3 input)
        {
            var viewPortForward = Vector3.Scale(this.viewPort.forward, new Vector3(1, 0, 1)).normalized;

            this.playerState.LookDirection = viewPortForward;

            return input.z * viewPortForward + input.x * this.viewPort.right;
        }

        [Serializable]
        public class Settings
        {
            public float MoveSpeed;
        }

        public class Movement
        {
            public Movement(Vector3 speed)
            {
                Speed = speed;
            }

            public Vector3 Speed { get; private set; }
        }
    }
}
