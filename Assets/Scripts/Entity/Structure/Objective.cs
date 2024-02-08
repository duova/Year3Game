using Core;
using UnityEngine;

namespace Entity.Structure
{
    [RequireComponent(typeof(LineRenderer))]
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
        
        [SerializeField]
        private float damagePerSecond;
        
        [SerializeField]
        private float engagementRange;
        
        private bool _simulationTicker;

        private Entity _target;

        private LineRenderer _lineRenderer;
        
        protected override void Start()
        {
            base.Start();
            _lineRenderer = GetComponent<LineRenderer>();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            
            _lineRenderer.positionCount = 0;
            
            if (MatchManager.Instance.MatchState != MatchState.Simulation) return;
            
            //Only run on every other physics update to reduce lag from querying on tick.
            _simulationTicker = !_simulationTicker;
            
            if (_simulationTicker) return;

            _target = OrderedEnemyList.Count > 0 ? OrderedEnemyList[0] : null;

            if (!IsInRange(_target, engagementRange)) return;
            if (!_target) return;
            _target.AddHealth(-damagePerSecond / 30f);
            _lineRenderer.positionCount = 2;
            _lineRenderer.SetPositions(new []{transform.position, _target.transform.position});
        }
    }
}