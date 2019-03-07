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

        float sonarTimer = 0f;
        Vector2 smoothLook = Vector2.zero;

        void Awake()
        {
            shark.useRigidbody = GetComponent<Rigidbody>();
            shark.useRigidbody.angularDrag = 1f;
            shark.useRigidbody.mass = 200f;
        }

        void OnDestroy()
        {
            ExitVehicle();
        }

        void Update ()
        {
            if (transform.parent)
            {
                transform.parent = null;
            }
        }

        void FixedUpdate()
        {
            float energyToConsume = 0f;

            //Stuff to do while piloting
            if (shark.GetPilotingMode())
            {
                if (Input.GetKeyDown(KeyCode.X))
                {
                    shark.isInFront = !shark.isInFront;
                }

                if (shark.energyInterface.hasCharge && shark.isInFront)
                {
                    shark.boostCharge += GameInput.GetButtonHeld(GameInput.Button.RightHand) ?
                            0.05f : -0.05f;

                    shark.boostCharge = Mathf.Clamp01(shark.boostCharge);

                    shark.isBoosting = shark.boostCharge == 1f;
                    Camera.main.fieldOfView = Mathf.Lerp(MiscSettings.fieldOfView, MiscSettings.fieldOfView * 1.3f, shark.boostCharge);
                }
                else
                {
                    shark.boostCharge = 0f;
                    shark.isBoosting = false;
                    Camera.main.fieldOfView = MiscSettings.fieldOfView;
                }

                //Stuff to do in water
                if (shark.useRigidbody.worldCenterOfMass.y < Ocean.main.GetOceanLevel() && 
                    !shark.precursorOutOfWater && 
                    shark.energyInterface.hasCharge)
                {

                    //Boost/Regular movement
                    if (shark.isBoosting)
                    {
                        shark.useRigidbody.AddForce(transform.forward * 2f, ForceMode.VelocityChange);
                        energyToConsume += 0.1f;
                    }
                    else
                    {
                        shark.useRigidbody.AddForce(transform.rotation * GameInput.GetMoveDirection() * 3000f);
                        energyToConsume += GameInput.GetMoveDirection().magnitude * 0.001f;
                    }

                    // Don't stabilize when shift is held
                    if (!GameInput.GetButtonHeld(GameInput.Button.Sprint))
                    {
                        shark.StabilizeRoll();
                    }

                    // Look rotation
                    Vector2 look = GameInput.GetLookDelta();

                    shark.useRigidbody.AddTorque(transform.up * look.x * 0.01f, ForceMode.VelocityChange);
                    shark.useRigidbody.AddTorque(transform.right * -look.y * 0.01f, ForceMode.VelocityChange);
                    shark.useRigidbody.AddTorque(transform.forward * -look.x * 0.01f, ForceMode.VelocityChange);
                }

                // Exit vehicle
                if(GameInput.GetButtonDown(GameInput.Button.Exit))
                {
                    ExitVehicle();
                }

                if(GameInput.GetButtonHeld(GameInput.Button.Deconstruct) && shark.energyInterface.hasCharge)
                {
                    if (Time.time > sonarTimer)
                    {
                        SNCameraRoot.main.SonarPing();
                        sonarTimer = Time.time + 4f;
                        energyToConsume += 5f;
                    }
                }

                if(GameInput.GetButtonDown(GameInput.Button.AltTool))
                {
                    shark.ToggleLights(!shark.lightsEnabled);
                }
                if(!shark.energyInterface.hasCharge && shark.lightsEnabled)
                {
                    shark.ToggleLights(false);
                }
            }
            else
            {
                shark.boostCharge = 0f;
                shark.isBoosting = false;
                Camera.main.fieldOfView = MiscSettings.fieldOfView;
            }
            shark.energyInterface.ConsumeEnergy(energyToConsume);

            Transform trans = shark.playerPosition.transform;
            Vector3 targetPos = shark.isInFront ? shark.chairFront.localPosition : shark.chairBack.localPosition;
            Quaternion targetRot = shark.isInFront ? shark.chairFront.localRotation : shark.chairBack.localRotation;
            trans.localPosition = Vector3.MoveTowards(trans.localPosition, targetPos, 0.1f);
            trans.localRotation = Quaternion.RotateTowards(trans.localRotation, targetRot, 10f);
        }

        void ExitVehicle()
        {
            if (shark.GetPilotingMode())
            {
                if (!Player.main.ToNormalMode(true))
                {
                    Player.main.ToNormalMode(false);
                }

                Vector3 newPos = Player.main.transform.position;
                newPos += Camera.main.transform.up;
                newPos += Camera.main.transform.forward;

                Player.main.transform.position = newPos;

                Player.main.transform.parent = null;
            }
        }
    }
}
