using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Shark
{
    public class SmoothVehicleCamera : MonoBehaviour
    {
        Vector2 smoothLook = Vector2.zero;

        public Vehicle vehicle;

        void Update()
        {
            if(vehicle.GetType().IsAssignableFrom(typeof(Shark)))
            {
                if(!((Shark)vehicle).isInFront)
                {
                    return;
                }
            }
 
            if (vehicle.GetPilotingMode() && Time.timeScale > 0f)
            {
                Vector2 look = GameInput.GetLookDelta();
                Quaternion cameraRot = MainCameraControl.main.transform.localRotation;

                smoothLook = Vector2.Lerp(smoothLook, look, 0.1f);

                cameraRot = Quaternion.Lerp(cameraRot, Quaternion.identity, 0.1f);
                Vector3 euler = cameraRot.eulerAngles;
                euler.x -= smoothLook.y;
                euler.y += smoothLook.x;

                if(vehicle.GetType().IsAssignableFrom(typeof(Shark)))
                {
                    if(((Shark)vehicle).isBoosting)
                    {
                        euler.y += UnityEngine.Random.value * 0.7f;
                        euler.x += UnityEngine.Random.value * 0.7f;
                    }
                }

                cameraRot = Quaternion.Euler(euler);
                MainCameraControl.main.transform.localRotation = cameraRot;
            }
        }
    }
}
