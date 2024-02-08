using System;
using UnityEngine;

namespace Entity.Module
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public class ProjectileShot : MonoBehaviour, IShot
    {
        //Implementation of a non-auto-hit projectile.
        
        public Entity Target { get; set; }
        
        public Entity Origin { get; set; }

        private Rigidbody _rb;

        private Collider _col;

        [SerializeField]
        private float radius;

        [SerializeField]
        private float damage;

        [SerializeField]
        private GameObject areaIndicatorPrefab;

        [SerializeField]
        private float indicatorDuration;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            
            //Get velocity component ratio.
            var horizontalVector = Target.transform.position - transform.position;
            horizontalVector.y = 0;
            var angle = Quaternion.Angle(Quaternion.LookRotation(horizontalVector), Quaternion.LookRotation(transform.forward));
            if (Math.Abs(angle - 90f) < 0.1f) throw new Exception("Projectile cannot hit targets if shot directly upwards.");
            var vRatio = Mathf.Tan(angle * Mathf.PI / 180);

            //Calculate velocity.
            /*
             * t = 2 * vv / g
             * d = t * vh
             * d = 2 * vv * vh / g
             * r = vv/vh
             * vv = r * vh
             * vv = r * d * g / (2 * vv)
             * vv * vv = r * d * g / 2
             * vv = sqrt(r * d * g / 2)
             * v = vv / sin(angle)
             */
            var horizontalDistance = horizontalVector.magnitude;
            var verticalVelocity = Mathf.Sqrt(vRatio * horizontalDistance * -Physics.gravity.y / 2f);
            var sinAngle = Mathf.Sin(angle * Mathf.PI / 180);
            if (sinAngle == 0f) throw new Exception("Projectile cannot hit targets if shot directly sideways.");
            var velocity = verticalVelocity / sinAngle;
            
            //Apply.
            _rb.AddForce(transform.forward * velocity, ForceMode.VelocityChange);
            
            //Destroy if not hit anything.
            Destroy(gameObject, 15f);
        }

        private void OnTriggerEnter(Collider other)
        {
            //Return if hit itself.
            if (Origin != null)
            {
                var originGo = Origin.gameObject;
                if (other.gameObject == originGo) return;
                var toCheck = other.gameObject;
                while (toCheck.transform.parent != null)
                {
                    toCheck = toCheck.transform.parent.gameObject;
                    if (toCheck == originGo) return;
                }
            }

            //Deal AoE damage.
            foreach (var hitObject in Physics.OverlapSphere(transform.position, radius))
            {
                if (!hitObject.TryGetComponent(out Entity entity)) continue;
                if (entity.Actor == Origin.Actor) continue;
                entity.AddHealth(-damage);
                print("hit enemy");
            }
            
            //Play effect.
            var effectLocation = transform.position;
            effectLocation.y = 0;
            Destroy(Instantiate(areaIndicatorPrefab, effectLocation, Quaternion.identity), indicatorDuration);
            
            
            Destroy(gameObject);
        }
    }
}