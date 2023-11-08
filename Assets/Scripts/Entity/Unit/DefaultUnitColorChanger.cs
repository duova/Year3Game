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

        private MeshRenderer _renderer;

        private void Start()
        {
            _unit = GetComponent<Unit>();
            _renderer = GetComponent<MeshRenderer>();
        }

        private void Update()
        {
            if (_unit.Actor.Side == Side.Home)
            {
                _renderer.material = homeMaterial;
            }
            else
            {
                _renderer.material = awayMaterial;
            }
        }
    }
}