using UnityEngine;

namespace Entity.Structure
{
    public class Drill : Entity
    {
        public override void BeginSimulation()
        {
            //Resource gain animation.
            Actor.currency += SpawnLocation.Node.CurrencyPerTurn;
        }

        public override void EndSimulation()
        {
            
        }
    }
}