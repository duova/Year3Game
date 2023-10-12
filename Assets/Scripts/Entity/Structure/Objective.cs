using UnityEngine;

namespace Entity.Structure
{
    public class Objective : Entity
    {
        public override void BeginSimulation()
        {
            
        }

        public override void EndSimulation()
        {
            
        }

        public override void Destroy()
        {
            Actor.Objectives.Remove(this);
            Actor.CheckIfLost();
            base.Destroy();
        }
    }
}