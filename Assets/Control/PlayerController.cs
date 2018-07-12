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

        public PlayerController(CharacterController controller, Transform viewPort, Settings settings, PlayerState playerState)
        {
            this.controller = controller;
            this.settings = settings;
            this.playerState = playerState;
            this.viewPort = viewPort;

            Debug.LogFormat("View port is {0}null", viewPort == null ? string.Empty : "not ");
        }

        public void Tick()
        {
            var input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * this.settings.MoveSpeed;

            var moveSpeed = this.TransformInputToViewPort(input);

            this.controller.SimpleMove(moveSpeed);
            this.controller.transform.rotation = Quaternion.LookRotation(this.playerState.LookDirection);
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
    }
}
