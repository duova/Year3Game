using System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class DeductCurrencyText : MonoBehaviour
    {
        public static DeductCurrencyText Instance { get; private set; }

        [SerializeField]
        private Vector3 offset;

        [SerializeField]
        private float upwardsMultiplier;
        
        [SerializeField]
        private float fadeMultiplier;

        private TMP_Text _textComp;

        private float _timeSinceDeduction;

        private Vector3 _pos;

        private void Awake()
        {
            Instance = this;
            _timeSinceDeduction = 999999f;
            _textComp = GetComponent<TMP_Text>();
        }

        public void Deduct(int amount)
        {
            _timeSinceDeduction = 0f;
            _textComp.text = "-$" + amount;
            _pos = Input.mousePosition;
        }

        public void NoMoney()
        {
            _timeSinceDeduction = 0f;
            _textComp.text = "Not enough $";
            _pos = Input.mousePosition;
        }

        private void Update()
        {
            _timeSinceDeduction += Time.unscaledDeltaTime;
            transform.localPosition =
                _pos - new Vector3(Screen.width / 2f, Screen.height / 2f) + offset + new Vector3(0, _timeSinceDeduction * upwardsMultiplier);
            var color = _textComp.color;
            color.a = Mathf.Max(1f - _timeSinceDeduction * fadeMultiplier, 0f);
            _textComp.color = color;
        }
    }
}