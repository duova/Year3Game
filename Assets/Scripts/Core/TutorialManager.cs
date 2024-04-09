using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.Serialization;

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

        [SerializeField]
        private GameObject initialUnitPrefab;

        [SerializeField]
        private GameObject initialUnitModulePrefab;

        [SerializeField]
        private string initialUnitName;

        public int GetSection()
        {
            return _currentSection;
        }

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
            GarageManager.Instance.GoToGarage();
            GarageManager.Instance.SelectEntity(initialUnitPrefab);
            GarageManager.Instance.SelectModule(initialUnitModulePrefab);
            GarageManager.Instance.LeaveGarage();
            GarageManager.Instance.EntitySaves[0].Name = initialUnitName;
            GarageManager.Instance.UpdateSlotMenu();
            ConditionalGoToSection(-1, 0);
        }
        
        public void ConditionalGoToSection(int previousRequiredSection, int section)
        {
            if (section >= sectionObjects.Length) return;
            if (_currentSection != previousRequiredSection) return;
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

        public void UnconditionalGoToSection(int section)
        {
            ConditionalGoToSection(_currentSection, section);
        }

        public void EndTutorial()
        {
            _currentSection = 999;
            foreach (var section in sectionObjects)
            {
                section.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }
    }
}