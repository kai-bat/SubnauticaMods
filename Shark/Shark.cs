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

        public override void Awake()
        {
            useRigidbody = GetComponent<Rigidbody>();
            worldForces = GetComponent<WorldForces>();
            base.Awake();
        }

        public override void Update()
        {
            base.Update();

            if (!worldForces.IsAboveWater() && GetPilotingMode())
            {
                isBoosting = Input.GetKey(KeyCode.LeftShift);
            }
        }

        public override void FixedUpdate()
        {
            if (!isBoosting)
            {
                base.FixedUpdate();
            }
            else
            {
                ErrorMessage.AddMessage("Boost Engaged!");
                useRigidbody.AddForce(transform.forward * 10f);
            }
        }
    }
}
