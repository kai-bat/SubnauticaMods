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
        public bool overheated = false;
        public float boostCharge = 0f;
        public float boostChargeDelta;
        public float boostHeat = 0f;

        public ToggleLights lights;
        public DealDamageOnImpact impactdmg;
        public SharkGunControl weapons;
        public SharkVisionControl vision;
        public SharkBlinkControl blink;
        public SharkDrillControl drill;
        public SharkShieldControl shield;
        public SharkFXControl fxControl;
        public SharkBladeControl bladeControl;
        public StorageContainer storage;

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
        public static TechType blinkTechType;
        public static TechType drillTechType;

        public static TechType internalBattery;
        public static TechType depletedIonCube;
        public static TechType ionFragment;

        public static TechType sharkEngine;
        public static TechType sharkComputer;
        public static TechType sharkHull;

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
            drill.upgradeInstalled = modules.GetCount(drillTechType) > 0;
            bladeControl.upgradeInstalled = modules.GetCount(ramTechType) > 0;
            if(modules.GetCount(visionTechType) == 0)
            {
                SharkVisionControl._enabled = false;
                UwePostProcessingManager.ToggleBloom(SharkVisionControl.bloomSettingSave);
            }

            impactdmg.mirroredSelfDamage = modules.GetCount(ramTechType) == 0;
        }

        public override void OnUpgradeModuleToggle(int slotID, bool active)
        {
            drill.active = GetSlotBinding(slotID) == drillTechType && active;
        }

        public override void OnUpgradeModuleUse(TechType techType, int slotID)
        {
            if (techType == visionTechType)
            {
                SharkVisionControl._enabled = !SharkVisionControl._enabled;
                if(SharkVisionControl._enabled)
                {
                    SharkVisionControl.bloomSettingSave = UwePostProcessingManager.bloomEnabled;
                    UwePostProcessingManager.ToggleBloom(false);
                }
                else
                {
                    UwePostProcessingManager.ToggleBloom(SharkVisionControl.bloomSettingSave);
                }
            }
            else if(techType == blinkTechType)
            {
                blink.AttemptBlink(this);
            }
            else if(techType == shieldTechType)
            {
                shield.active = !shield.active;
            }
        }

        public override void SlotLeftHeld()
        {
            if(GetSlotBinding(activeSlot) == drillTechType)
            {
                drill.TryDrill();
                drill.drilling = true;
            }
            base.SlotLeftHeld();
        }

        public override void SlotLeftUp()
        {
            drill.drilling = false;
            base.SlotLeftUp();
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

            window.SetActive(SharkVisionControl.active);

            bladeControl.deployed = isBoosting;
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
