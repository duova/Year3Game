﻿using System;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Core
{
    public class TutorialManager : MonoBehaviour
    {
        public static TutorialManager Instance { get; private set; }

        [SerializeField]
        private GameObject[] sectionObjects;

        private int _currentSection = -1;

        [SerializeField]
        public HoverText garageHoverText;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            foreach (var section in sectionObjects)
            {
                section.SetActive(false);
            }
            ConditionalGoToSection(-1, 0);
        }
        
        public void ConditionalGoToSection(int previousRequiredSection, int section)
        {
            if (section >= sectionObjects.Length) return;
            if (_currentSection != previousRequiredSection && _currentSection != -1) return;
            if (_currentSection >= 0)
            {
                sectionObjects[_currentSection].SetActive(false);
            }

            if (section >= 0)
            {
                sectionObjects[section].SetActive(true);
            }

            _currentSection = section;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }
    }
}