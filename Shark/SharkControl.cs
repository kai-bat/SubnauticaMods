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

        void Update()
        {
            Fixer fixer = shark.GetComponent<Fixer>();
            if (fixer)
            {
                Object.Destroy(fixer);
            }

            if (shark.GetPilotingMode() && !shark.worldForces.IsAboveWater())
            {
                shark.isBoosting = GameInput.GetButtonHeld(GameInput.Button.RightHand);

                //Boost/Regular movement
                if(shark.isBoosting)
                {
                    shark.useRigidbody.AddForce(transform.forward, ForceMode.VelocityChange);
                }
                else
                {
                    shark.useRigidbody.AddForce(transform.rotation * GameInput.GetMoveDirection() * 2500f);
                }

                // Look rotation
                Vector2 look = GameInput.GetLookDelta();

                shark.useRigidbody.AddTorque(transform.up * look.x * 0.01f, ForceMode.VelocityChange);
                shark.useRigidbody.AddTorque(transform.right * -look.y * 0.01f, ForceMode.VelocityChange);
                shark.useRigidbody.AddTorque(transform.forward * -look.x * 0.01f, ForceMode.VelocityChange);

                Quaternion cameraRot = MainCameraControl.main.transform.localRotation;

                smoothLook = Vector2.Lerp(smoothLook, look, 0.1f);

                cameraRot = Quaternion.Lerp(cameraRot, Quaternion.identity, 0.1f);
                Vector3 euler = cameraRot.eulerAngles;
                euler.x -= smoothLook.y;
                euler.y += smoothLook.x;

                cameraRot = Quaternion.Euler(euler);
                MainCameraControl.main.transform.localRotation = cameraRot;

                // Don't stabilize when shift is held
                if (!GameInput.GetButtonHeld(GameInput.Button.Sprint))
                {
                    shark.StabilizeRoll();
                }

                // Exit vehicle
                if(GameInput.GetButtonDown(GameInput.Button.Exit))
                {
                    ExitVehicle();
                }

                if(GameInput.GetButtonDown(GameInput.Button.AltTool))
                {
                    SNCameraRoot.main.SonarPing();
                }
            }
        }

        void ExitVehicle()
        {
            if (shark.GetPilotingMode())
            {
                if (!Player.main.ToNormalMode(true))
                {
                    Player.main.ToNormalMode(false);
                }

                Vector3 newPos = Player.main.transform.localPosition;
                newPos.y += 1f;
                newPos.z += 2f;

                Player.main.transform.localPosition = newPos;

                Player.main.transform.parent = null;
            }
        }
    }
}
