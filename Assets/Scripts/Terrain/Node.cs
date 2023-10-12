using UnityEngine;

namespace Terrain
{
    public class Node : MonoBehaviour
    {
        public int CurrencyPerTurn => currencyPerTurn;

        [SerializeField]
        private int currencyPerTurn;
    }
}