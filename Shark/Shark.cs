using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Shark
{
    public class Shark : Vehicle
    {
        public override string vehicleDefaultName
        {
            get
            {
                return "5H-4RK Submersible";
            }
        }

        public bool isBoosting = false;
        public float boostCharge = 0f;
        public float boostChargeDelta;


        public ToggleLights lights;
        public SharkFireControl weapons;
        public SharkVisionControl vision;
        public SharkBlinkControl blink;

        public GameObject window;

        public FMODAsset chargeFinished;
        public FMOD_CustomLoopingEmitter chargeUp;
        public FMOD_CustomLoopingEmitter boost;
        public FMOD_CustomLoopingEmitter normalMove;
        public FMODAsset splash;

        public static TechType laserTechType;
        public static TechType ramTechType;
        public static TechType visionTechType;
        public static TechType shieldTechType;
        public static TechType mineTechType;
        public static TechType blinkTechType;

        public static TechType internalBattery;
        public static TechType depletedIonCube;

        public GameObject clipTest;

        public override string[] slotIDs
        {
            get
            {
                return new string[] {
                    "SharkSlot1",
                    "SharkSlot2",
                    "SharkSlot3",
                    "SharkSlot4"
                };
            }
        }

        public override void OnUpgradeModuleChange(int slotID, TechType techType, bool added)
        {
            weapons.upgradeInstalled = modules.GetCount(laserTechType) > 0;
            if(modules.GetCount(visionTechType) == 0)
            {
                SharkVisionControl._enabled = false;
            }
        }

        public override void OnUpgradeModuleToggle(int slotID, bool active)
        {
        }

        public override void OnUpgradeModuleUse(TechType techType, int slotID)
        {
            if (techType == visionTechType)
            {
                SharkVisionControl._enabled = !SharkVisionControl._enabled;
            }
            else if(techType == blinkTechType)
            {
                blink.AttemptBlink(this);
            }
        }

        public override void Start()
        {
            //base.Start();
        }

        public override void Awake()
        {
            base.Awake();
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

            docked = false;

            window.SetActive(SharkVisionControl.active);
        }

        public override void EnterVehicle(Player player, bool teleport, bool playEnterAnimation = true)
        {
            base.EnterVehicle(player, true, false);
        }

        public override void OnPilotModeBegin()
        {
            Player.main.armsController.SetWorldIKTarget(leftHandPlug, rightHandPlug);
            Player.main.inSeamoth = true;
            if (!vision)
            {
                vision = gameObject.AddComponent<SharkVisionControl>();
            }
            base.OnPilotModeBegin();
        }

        public override void OnPilotModeEnd()
        {
            Player.main.armsController.SetWorldIKTarget(null, null);
            Player.main.inSeamoth = false;
            isBoosting = false;
            boostCharge = 0f;
            boostChargeDelta = 0f;
            base.OnPilotModeEnd();
        }

        public void ToggleLights(bool on)
        {
            lights.SetLightsActive(on);
        }
    }
}
