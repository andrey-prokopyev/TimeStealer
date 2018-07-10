using UnityEngine;
using Zenject;

namespace Weapon
{
    public class Bullet : MonoBehaviour, IPoolable<float, float, Vector3, Vector3, IMemoryPool>
    {
        //TODO : fix fireball speed
        private float speed;

        private IMemoryPool pool;

        public void OnSpawned(float speed, float charge, Vector3 direction, Vector3 position, IMemoryPool pool)
        {
            this.speed = speed;
            this.GetComponent<Damager>().Damage = charge;
            this.pool = pool;

            this.transform.position = position;
            this.transform.rotation = Quaternion.LookRotation(direction);
        }

        public void OnDespawned()
        {
            this.pool = null;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            Debug.LogFormat("Bullet {0} collided with {1}", this.gameObject.name, other.gameObject.name);

            if (other.gameObject.CompareTag("Enemy"))
            {
                this.pool.Despawn(this);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            this.CheckLeftBoundaries(other);
        }

        private void CheckLeftBoundaries(Component other)
        {
            if (other.CompareTag("Boundary"))
            {
                Debug.LogFormat("{0} has left level boundary", this.gameObject.name);
                this.pool.Despawn(this);
            }
        }

        public override string ToString()
        {
            return string.Format("Name: {0}, Speed: {1}", this.name, speed);
        }

        public class Factory : PlaceholderFactory<float, float, Vector3, Vector3, Bullet>
        {
        }

        public class Pool : MonoPoolableMemoryPool<float, float, Vector3, Vector3, IMemoryPool, Bullet>
        {
        }
    }
}