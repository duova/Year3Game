using UnityEngine;

namespace Entity.Structure
{
    public class Drill : Entity
    {
        public override void BeginSimulation()
        {
            //Resource gain animation.
            Actor.Currency += SpawnLocation.Node.CurrencyPerTurn;
        }

        public override void EndSimulation()
        {
            
        }
    }
}