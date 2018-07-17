using UnityEngine;
using Zenject;

namespace Enemies
{
    public class PlayerAdvancedPursuer : Pursuer
    {
        public new class Factory : PlaceholderFactory<PlayerAdvancedPursuer>, IFactory<MonoBehaviour>
        {
            public new MonoBehaviour Create()
            {
                return base.Create();
            }
        }

        public new class Pool : MonoPoolableMemoryPool<IMemoryPool, PlayerAdvancedPursuer>
        {
            protected override void OnSpawned(PlayerAdvancedPursuer item)
            {
                base.OnSpawned(item);
                item.gameObject.SetActive(false);
            }
        }
    }
}