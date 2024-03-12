using Core;
using UnityEngine;

namespace UI
{
    public class BounceUI : MonoBehaviour
    {
        private float _time;
        
        private Vector3 _defaultPos;

        [SerializeField]
        private Vector3 vector;
        
        private void Awake()
        {
            _defaultPos = transform.localPosition;
        }

        private void Update()
        {
            _time += Time.deltaTime;
            transform.localPosition =
                _defaultPos + vector * Mathf.Abs(Mathf.Sin(_time / Mathf.PI * 4));
        }
    }
}