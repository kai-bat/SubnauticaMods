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

        void Update ()
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

            cam.shouldLook = shark.GetPilotingMode() && Time.timeScale > 0f;

            // Process Controls
            if(shark.GetPilotingMode())
            {
                if (shark.energyInterface.hasCharge)
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

                    float preBoost = shark.boostCharge;
                    shark.boostCharge += (GameInput.GetButtonHeld(GameInput.Button.Sprint) ?
                        2f : -2f) * Time.deltaTime;

                    shark.boostCharge = Mathf.Clamp01(shark.boostCharge);
                    float postBoost = shark.boostCharge;

                    shark.isBoosting = shark.boostCharge == 1f;

                    if(isAboveWater)
                    {
                        shark.isBoosting = false;
                    }

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
            if (shark.GetPilotingMode() && shark.energyInterface.hasCharge)
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
