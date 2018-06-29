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

        public PlayerController(CharacterController controller, Transform viewPort, Settings settings)
        {
            this.controller = controller;
            this.settings = settings;
            this.viewPort = viewPort;
        }

        public void Tick()
        {
            var input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * this.settings.MoveSpeed;

            Debug.LogFormat("Input: {0}", input);

            this.controller.SimpleMove(this.TransformInputToViewPort(input));
        }

        private Vector3 TransformInputToViewPort(Vector3 input)
        {
            var viewPortForward = Vector3.Scale(this.viewPort.forward, new Vector3(1, 0, 1)).normalized;
            return input.z * viewPortForward + input.x * this.viewPort.right;
        }

        [Serializable]
        public class Settings
        {
            public float MoveSpeed;
        }
    }
}
