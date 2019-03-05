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

        void Awake()
        {
            shark.useRigidbody = GetComponent<Rigidbody>();
            shark.useRigidbody.angularDrag = 1f;
        }

        void OnDestroy()
        {
            if(shark.GetPilotingMode())
            {
                if(!Player.main.ToNormalMode(true))
                {
                    Player.main.ToNormalMode(false);
                }

                Player.main.transform.parent = null;
            }
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
                shark.isBoosting = GameInput.GetButtonHeld(GameInput.Button.Sprint);

                if(shark.isBoosting)
                {
                    shark.useRigidbody.AddForce(transform.forward * 10f, ForceMode.VelocityChange);
                }
                else
                {
                    shark.useRigidbody.AddForce(transform.rotation * GameInput.GetMoveDirection() * 5f);
                }

                Vector2 look = GameInput.GetLookDelta();

                shark.useRigidbody.AddTorque(transform.up * look.x * 0.01f, ForceMode.VelocityChange);
                shark.useRigidbody.AddTorque(transform.right * -look.y * 0.01f, ForceMode.VelocityChange);
                shark.useRigidbody.AddTorque(transform.forward * -look.x * 0.01f, ForceMode.VelocityChange);

                shark.StabilizeRoll();
            }
        }
    }
}
