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

        float sonarTimer = 0f;
        Vector2 lookForce;
        LayerMask mask;

        void Awake()
        {
            shark.useRigidbody = GetComponent<Rigidbody>();
            shark.useRigidbody.angularDrag = 1f;
            shark.useRigidbody.mass = 200f;
            shark.useRigidbody.useGravity = false;

            mainCollision = transform.Find("Collision").gameObject;
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
                if (Input.GetKeyDown(KeyCode.X))
                {
                    shark.isInFront = !shark.isInFront;

                    if(shark.isInFront)
                    {
                        shark.OnPilotModeBegin();
                    }
                    else
                    {
                        shark.OnPilotModeEnd();
                    }
                }

                if (shark.energyInterface.hasCharge)
                {
                    if (shark.isInFront)
                    {
                        if(GameInput.GetButtonDown(GameInput.Button.AltTool))
                        {
                            shark.ToggleLights(!shark.lights.GetLightsActive());
                        }

                        if (GameInput.GetButtonHeld(GameInput.Button.Deconstruct))
                        {
                            shark.weapons.AttemptShoot();
                        }

                        if(GameInput.GetButtonDown(GameInput.Button.Exit))
                        {
                            ExitVehicle();
                        }

                        float preBoost = shark.boostCharge;
                        shark.boostCharge += (GameInput.GetButtonHeld(GameInput.Button.Sprint) ?
                            2f : -2f) * Time.deltaTime;

                        shark.boostCharge = Mathf.Clamp01(shark.boostCharge);
                        float postBoost = shark.boostCharge;

                        shark.isBoosting = shark.boostCharge == 1f;
                        Camera.main.fieldOfView = Mathf.Lerp(MiscSettings.fieldOfView, MiscSettings.fieldOfView * 1.3f, shark.boostCharge);

                        shark.boostChargeDelta = postBoost - preBoost;

                        // Look rotation
                        lookForce = GameInput.GetLookDelta();
                    }
                    else
                    {
                        ResetCamera();
                    }
                }
                else
                {
                    shark.ToggleLights(false);
                    ResetCamera();
                }
            }
            else
            {
                ResetCamera();
            }

            Transform trans = shark.playerPosition.transform;
            Vector3 targetPos = shark.isInFront ? shark.chairFront.localPosition : shark.chairBack.localPosition;
            Quaternion targetRot = shark.isInFront ? shark.chairFront.localRotation : shark.chairBack.localRotation;
            trans.localPosition = Vector3.MoveTowards(trans.localPosition, targetPos, 5f*Time.deltaTime);
            trans.localRotation = Quaternion.RotateTowards(trans.localRotation, targetRot, 500f*Time.deltaTime);
        }

        void ResetCamera()
        {
            shark.boostCharge = 0f;
            shark.isBoosting = false;
            cam.Reset();
        }

        void FixedUpdate()
        {
            if (shark.GetPilotingMode() && shark.isInFront && shark.energyInterface.hasCharge)
            {
                bool isInWater = transform.position.y < Ocean.main.GetOceanLevel() && !shark.precursorOutOfWater;
                if (isInWater)
                {
                    if (shark.isBoosting)
                    {
                        shark.useRigidbody.AddForce(transform.forward * 2f, ForceMode.VelocityChange);
                    }
                    else
                    {
                        shark.useRigidbody.AddForce(transform.rotation * Vector3.ClampMagnitude(GameInput.GetMoveDirection(), 1f) * 3000f);
                    }
                }

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

                Player.main.ToNormalMode(false);

                Vector3 newPos = Player.main.transform.position;
                newPos += transform.up;
                newPos += transform.forward * 2f;

                Player.main.transform.position = newPos;
            }
        }
    }
}
