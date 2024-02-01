using UnityEngine;

namespace Entity.Module
{
    public class ProjectileShot : MonoBehaviour, IShot
    {
        //Implementation of a non-auto-hit projectile.
        
        public Entity Target { get; set; }
        
        public Entity Origin { get; set; }
        
        //Calculate and perform arc.
    }
}