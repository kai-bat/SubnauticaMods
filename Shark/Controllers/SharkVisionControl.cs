using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Shark
{
    public class SharkVisionControl : MonoBehaviour
    {
        public static Camera cam;
        public static bool active
        {
            get
            {
                Vehicle vehicle = Player.main.GetVehicle();
                if(!vehicle)
                {
                    return false;
                }

                if(!vehicle.GetComponent<Shark>())
                {
                    return false;
                }

                Shark shark = vehicle.GetComponent<Shark>();
                bool able = shark.GetPilotingMode() && shark.energyInterface.hasCharge;

                if(!able)
                {
                    return false;
                }

                return _enabled;
            }
        }

        public static bool _enabled;
        public static bool bloomSettingSave;

        public void Awake()
        {
            bloomSettingSave = UwePostProcessingManager.bloomEnabled;

            if(!cam)
            {
                Camera newCam = new GameObject("wallcam").AddComponent<Camera>();
                newCam.transform.parent = SNCameraRoot.main.mainCam.transform;
                newCam.transform.localPosition = Vector3.zero;
                newCam.transform.localEulerAngles = Vector3.zero;
                newCam.clearFlags = CameraClearFlags.Depth;
                newCam.depth = 1;
                newCam.cullingMask = 8388608;
                newCam.nearClipPlane = 0.01f;
                newCam.depth = 3;
                cam = newCam;
            }
        }

        public void Update()
        {
            cam.fieldOfView = SNCameraRoot.main.mainCam.fieldOfView;
        }
    }
}
