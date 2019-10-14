using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Shark
{
    public class Shark : Vehicle
    {
        public bool isBoosting = false;
        public float boostCharge = 0f;
        public float boostChargeDelta;

        public ToggleLights lights;

        public bool isInFront = true;

        public Transform chairFront;
        public Transform chairBack;

        public FMODAsset chargeFinished;
        public FMOD_CustomLoopingEmitter chargeUp;
        public FMOD_CustomLoopingEmitter boost;
        public FMOD_CustomLoopingEmitter normalMove;
        public FMOD_CustomEmitter sonarPing;
        public FMODAsset splash;

        public override void Awake()
        {
            
        }

        public override void Update()
        {
            
        }

        public override void FixedUpdate()
        {
            bool pilotingMode = GetPilotingMode();
            if (pilotingMode != lastPilotingState)
            {
                if (pilotingMode)
                {
                    OnPilotModeBegin();
                }
                else
                {
                    OnPilotModeEnd();
                }
                lastPilotingState = pilotingMode;
            }
        }

        public override void OnPilotModeBegin()
        {
            Player.main.armsController.SetWorldIKTarget(leftHandPlug, rightHandPlug);
        }

        public override void OnPilotModeEnd()
        {
            OnPilotModeEnd();
        }

        public void ToggleLights(bool on)
        {
            lights.SetLightsActive(on);
        }
    }
}
