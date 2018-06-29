using UnityEngine;

namespace Targeting
{
    public class PlayerPicker : ITargetPicker
    {
        private GameObject player;

        private GameObject Player
        {
            get
            {
                if (this.player == null)
                {
                    this.player = GameObject.FindWithTag("Player");
                }

                return this.player;
            }
        }

        public GameObject PickFor(GameObject pursuer)
        {
            return this.Player;
        }
    }
}