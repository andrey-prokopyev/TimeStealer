using UnityEngine;
using Zenject;

namespace Weapon
{
    public class Bullet : MonoBehaviour, IPoolable<float, float, Vector3, Vector3, IMemoryPool>
    {
        private float speed;

        private float charge;

        private IMemoryPool pool;

        private Vector3 direction;

        public void OnSpawned(float speed, float charge, Vector3 direction, Vector3 position, IMemoryPool pool)
        {
            this.speed = speed;
            this.charge = charge;
            this.pool = pool;

            this.direction = direction;

            this.transform.position = position;
        }

        public void OnDespawned()
        {
            this.pool = null;
        }
        
        private void OnCollisionEnter(Collision other)
        {
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

        private void FixedUpdate()
        {
            var deltaPosition = this.speed * this.direction.normalized;

            this.transform.position += deltaPosition;
        }

        public override string ToString()
        {
            return string.Format("Name: {0}, Speed: {1}, Charge: {2}", this.name, speed, charge);
        }

        public class Factory : PlaceholderFactory<float, float, Vector3, Vector3, Bullet>
        {
        }

        public class Pool : MonoPoolableMemoryPool<float, float, Vector3, Vector3, IMemoryPool, Bullet>
        {
        }
    }
}