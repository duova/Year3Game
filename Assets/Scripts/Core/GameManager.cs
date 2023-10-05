using Terrain;
using UnityEngine;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        public Board Board { get; private set; }
        
        public Actor HomeActor { get; private set; }

        public Actor AwayActor { get; private set; }
    }
}