using System;
using Core;
using UnityEngine;

namespace Entity.Unit
{
    public class DefaultUnitColorChanger : MonoBehaviour
    {
        [SerializeField]
        private Material homeMaterial;

        [SerializeField]
        private Material awayMaterial;

        private Unit _unit;

        private MeshRenderer[] _renderers;

        private void Start()
        {
            _unit = transform.parent.GetComponent<Unit>();
            _renderers = GetComponentsInChildren<MeshRenderer>();
        }

        private void Update()
        {
            foreach (var rdr in _renderers)
            {
                if (_unit.Actor.Side == Side.Home)
                {
                    rdr.material = homeMaterial;
                }
                else
                {
                    rdr.material = awayMaterial;
                }
            }
        }
    }
}