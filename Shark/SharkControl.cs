using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SMLHelper.V2.MonoBehaviours;
using Object = UnityEngine.Object;

namespace Shark
{
    public class SharkControl : MonoBehaviour
    {
        public Shark shark;
        public SharkCameraSmooth cam;
        public GameObject mainCollision;
        public SharkSound sound;
        public SonarVision vision;

        Vector2 lookForce;

        void Awake()
        {
            shark.useRigidbody = GetComponent<Rigidbody>();
            shark.useRigidbody.angularDrag = 1f;
            shark.useRigidbody.mass = 200f;
            shark.useRigidbody.useGravity = false;

            mainCollision = transform.Find("Scaler/Collision").gameObject;
        }

        void OnDestroy()
        {
            ExitVehicle();
        }

        void Update()
        {
            bool isAboveWater = transform.position.y > Ocean.main.GetOceanLevel() || shark.precursorOutOfWater;
            if (isAboveWater != shark.wasAboveWater)
            {
                sound.DoSplash();
                shark.wasAboveWater = isAboveWater;
            }

            if (transform.parent)
            {
                transform.parent = null;
            }

            bool freecamEnabled = MainCameraControl.main.GetComponent<FreecamController>().GetActive();

            cam.shouldLook = shark.GetPilotingMode() && Time.timeScale > 0f && !freecamEnabled;

            // Process Controls
            if (shark.GetPilotingMode() && !DevConsole.instance.selected && !freecamEnabled)
            {
                if (shark.energyInterface.hasCharge || !GameModeUtils.RequiresPower())
                {
                    if (GameInput.GetButtonDown(GameInput.Button.AltTool))
                    {
                        shark.ToggleLights(!shark.lights.GetLightsActive());
                    }

                    if (GameInput.GetButtonHeld(GameInput.Button.RightHand))
                    {
                        shark.weapons.AttemptShoot();
                    }

                    if (GameInput.GetButtonDown(GameInput.Button.Exit))
                    {
                        ExitVehicle();
                    }

                    if(shark.boostHeat == 1f)
                    {
                        shark.boostCharge = 0f;
                        shark.overheated = true;
                    }

                    if(shark.boostHeat == 0f)
                    {
                        shark.overheated = false;
                    }

                    float preBoost = shark.boostCharge;
                    if (GameInput.GetButtonHeld(GameInput.Button.Sprint) && !shark.overheated)
                    {
                        shark.boostCharge += Time.deltaTime * 2f;
                    }
                    else
                    {
                        shark.boostCharge = 0f;
                    }

                    shark.boostCharge = Mathf.Clamp01(shark.boostCharge);
                    float postBoost = shark.boostCharge;

                    shark.isBoosting = shark.boostCharge == 1f;
                    
                    if(isAboveWater)
                    {
                        shark.isBoosting = false;
                    }

                    shark.boostHeat += shark.isBoosting ? Time.deltaTime / 5f : -Time.deltaTime / 5f;
                    shark.boostHeat = Mathf.Clamp01(shark.boostHeat);

                    shark.boostChargeDelta = postBoost - preBoost;

                    // Look rotation
                    lookForce = GameInput.GetLookDelta();
                }
                else
                {
                    shark.ToggleLights(false);
                }
            }
        }

        void FixedUpdate()
        {
            if (shark.GetPilotingMode() && shark.energyInterface.hasCharge && !DevConsole.instance.selected && !MainCameraControl.main.GetComponent<FreecamController>().GetActive())
            {
                bool isInWater = transform.position.y < Ocean.main.GetOceanLevel() && !shark.precursorOutOfWater;
                if (isInWater)
                {
                    if (shark.isBoosting)
                    {
                        shark.useRigidbody.AddForce(transform.forward * 10000f);
                    }
                    else
                    {
                        shark.useRigidbody.AddForce(transform.rotation * Vector3.ClampMagnitude(GameInput.GetMoveDirection(), 1f) * 3000f);
                    }
                }

                shark.StabilizeRoll();

                shark.useRigidbody.AddTorque(transform.up * lookForce.x * 0.01f, ForceMode.VelocityChange);
                shark.useRigidbody.AddTorque(transform.right * -lookForce.y * 0.01f, ForceMode.VelocityChange);
                shark.useRigidbody.AddTorque(transform.forward * -lookForce.x * 0.01f, ForceMode.VelocityChange);
            }
        }

        void ExitVehicle()
        {
            if (shark.GetPilotingMode())
            {
                Player.main.transform.parent = null;

                Player.main.ToNormalMode(true);
                Player.main.mode = Player.Mode.Normal;
                Player.main.armsController.SetWorldIKTarget(null, null);
                shark.window.SetActive(false);
            }
        }
    }
}
