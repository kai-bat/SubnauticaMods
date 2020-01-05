using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Shark
{
    public class SharkCameraSmooth : MonoBehaviour
    {
        Vector2 smoothLookDelta = Vector2.zero;
        Vector2 rearCamTarget = Vector2.zero;

        public Shark shark;
        public bool shouldLook;

        void Update()
        {
            if (shouldLook && !Player.main.GetPDA().isOpen)
            {
                Vector2 look = GameInput.GetLookDelta();
                Quaternion cameraRot = MainCameraControl.main.transform.localRotation;

                cameraRot = Quaternion.Lerp(cameraRot, Quaternion.identity, 0.1f);
                smoothLookDelta = Vector2.Lerp(smoothLookDelta, look, 0.1f);
                Vector3 euler = cameraRot.eulerAngles;
                euler.x -= smoothLookDelta.y;
                euler.y += smoothLookDelta.x;
                rearCamTarget.x -= look.y;
                rearCamTarget.y += look.x;

                if (shark.isBoosting)
                {
                    euler.y += UnityEngine.Random.value * 0.7f;
                    euler.x += UnityEngine.Random.value * 0.7f;
                }

                rearCamTarget.x = Mathf.Clamp(rearCamTarget.x, -80, 80f);
                rearCamTarget.y = Mathf.Clamp(rearCamTarget.y, -80, 80f);

                rearCamTarget = Vector2.zero;

                euler = Vector3.ClampMagnitude(euler, 80f);

                cameraRot = Quaternion.Euler(euler);
                MainCameraControl.main.transform.localRotation = cameraRot;
            }
            else if(Player.main.GetPDA().isOpen)
            {
                Quaternion cameraRot = MainCameraControl.main.transform.localRotation;
                cameraRot = Quaternion.Lerp(cameraRot, Quaternion.identity, 0.1f);
                MainCameraControl.main.transform.localRotation = cameraRot;
                rearCamTarget = Vector2.zero;
            }
        }
    }
}
