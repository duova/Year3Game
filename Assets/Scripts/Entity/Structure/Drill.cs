using System;
using Core;
using TMPro;
using UnityEngine;

namespace Entity.Structure
{
    public class Drill : Entity
    {
        [SerializeField]
        private int roundsToDisableFor = 2;
        
        //0 is not disabled
        public int roundsDisabled;

        [SerializeField]
        private TMP_Text disabledText;

        [SerializeField]
        private GameObject workingModelObject;
        
        [SerializeField]
        private GameObject brokenModelObject;

        protected override void Start()
        {
            base.Start();
            DestroyDisabled = true;
            disabledText.text = "";
            workingModelObject.SetActive(true);
            brokenModelObject.SetActive(false);
        }

        public void Update()
        {
            if (Health > 0.1f) return;
            Disable();
        }

        public void Disable()
        {
            if (TutorialManager.Instance)
            {
                TutorialManager.Instance.ConditionalGoToSection(9, 10);
            }
            
            if (roundsDisabled == 0)
            {
                roundsDisabled = roundsToDisableFor;
                disabledText.text = "Back in " + roundsDisabled;
                //Change model.
                workingModelObject.SetActive(false);
                brokenModelObject.SetActive(true);
            }
        }
        
        public override void BeginSimulation()
        {
        }

        public override void EndSimulation()
        {
            if (roundsDisabled == 0)
            {
                //Resource gain animation.
                Actor.Currency += SpawnLocation.Node.CurrencyPerTurn;
                if (PlayerController.Instance.Actor == Actor)
                {
                    PlayerController.Instance.gainedCurrencyThisRound += SpawnLocation.Node.CurrencyPerTurn;
                }
            }

            if (roundsDisabled == 1)
            {
                roundsDisabled = 0;
                SetHealth(MaxHealth);
                //Change model.
                workingModelObject.SetActive(true);
                brokenModelObject.SetActive(false);
            }
            if (roundsDisabled > 0) roundsDisabled--;
            if (roundsDisabled != 0)
            {
                disabledText.text = "Back in " + roundsDisabled;
            }
            else
            {
                disabledText.text = "";
            }
        }
    }
}