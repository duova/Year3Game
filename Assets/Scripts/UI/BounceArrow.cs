using Core;
using UnityEngine;

namespace UI
{
    public class BounceArrow : MonoBehaviour
    {
        private float _time;
        
        private Vector3 _defaultPos;
        
        private void Awake()
        {
            _defaultPos = transform.localPosition;
        }

        private void Update()
        {
            _time += Time.deltaTime;
            transform.localPosition =
                _defaultPos + new Vector3(0, 0, Mathf.Abs(Mathf.Sin(_time / Mathf.PI * 4)));
        }
    }
}