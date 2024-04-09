using System;
using System.Linq;
using Core;
using UnityEngine;

namespace Entity.Unit
{
    public class UnitColorChanger : MonoBehaviour
    {
        [SerializeField]
        private Material blueMaterial;

        [SerializeField]
        private Material orangeMaterial;

        [SerializeField]
        private MeshRenderer[] renderersToSkip;

        private Entity _entity;

        private MeshRenderer[] _renderers;
        
        private SkinnedMeshRenderer[] _skinnedRenderers;

        private TeamColor _color;

        private void Start()
        {
            _entity = transform.parent.GetComponent<Entity>();
            _renderers = GetComponentsInChildren<MeshRenderer>();
            _skinnedRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            _color = _entity.Actor.Side == Side.Home ? MatchManager.Instance.HomeColor : MatchManager.Instance.AwayColor;
        }

        private void Update()
        {
            foreach (var rdr in _renderers)
            {
                if (renderersToSkip.Contains(rdr)) continue;
                if (_color == TeamColor.Blue)
                {
                    rdr.material = blueMaterial;
                }
                else
                {
                    rdr.material = orangeMaterial;
                }
            }
            
            foreach (var rdr in _skinnedRenderers)
            {
                if (_color == TeamColor.Blue)
                {
                    rdr.material = blueMaterial;
                }
                else
                {
                    rdr.material = orangeMaterial;
                }
            }
        }
    }
}