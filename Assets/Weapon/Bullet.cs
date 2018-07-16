using DigitalRuby.PyroParticles;
using UnityEngine;
using Zenject;

namespace Weapon
{
    public class Bullet : MonoBehaviour, IPoolable<float, WeaponCharger.Charge, Vector3, Vector3, IMemoryPool>
    {
        private IMemoryPool pool;

        public void OnSpawned(float speed, WeaponCharger.Charge charge, Vector3 direction, Vector3 position, IMemoryPool pool)
        {
            this.GetComponent<Damager>().Damage = charge.Current;
            this.pool = pool;

            this.transform.position = position;
            this.transform.rotation = Quaternion.LookRotation(direction);

            // todo: refactor code below pls
            var fireball = this.transform.Find("Fireball");
            var projectilesParticles = fireball.Find("FireballProjectiles").GetComponent<ParticleSystem>().main;
            var bulletSpeed = speed * (1 - charge.Current / (2 * charge.Max));
            projectilesParticles.startSpeed = new ParticleSystem.MinMaxCurve(bulletSpeed);

            var tailParticles = fireball.Find("FireballTail").GetComponent<ParticleSystem>().main;
            tailParticles.startSize = new ParticleSystem.MinMaxCurve(tailParticles.startSize.constant * charge.Current / charge.Max);
            tailParticles.startSpeed = projectilesParticles.startSpeed;

            fireball.GetComponent<FireProjectileScript>().ProjectileColliderSpeed = bulletSpeed;
        }

        public void OnDespawned()
        {
            this.pool = null;
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

        public class Factory : PlaceholderFactory<float, WeaponCharger.Charge, Vector3, Vector3, Bullet>
        {
        }

        public class Pool : MonoPoolableMemoryPool<float, WeaponCharger.Charge, Vector3, Vector3, IMemoryPool, Bullet>
        {
        }
    }
}