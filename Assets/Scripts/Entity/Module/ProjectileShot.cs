using UnityEngine;

namespace Entity.Module
{
    public class ProjectileShot : MonoBehaviour
    {
        //Implementation of a non-auto-hit projectile.
        
        public Entity Target { get; set; }

        public bool Tracking { get; set; }
        
        //Calculate and perform arc.
    }
}