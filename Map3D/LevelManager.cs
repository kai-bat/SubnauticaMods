using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Map3D
{
    public class LevelManager : MonoBehaviour
    {
        public static Camera cam;
        public static RenderTexture tex;
        public static LevelManager main;

        public float distance = 100f;

        Vector2 oldMousePosition;
        VehicleInterface_Terrain hologram;

        public void CreateMap()
        {
            Console.WriteLine("Creating Hologram");
            GameObject seaglide = CraftData.GetPrefabForTechType(TechType.Seaglide);
            GameObject obj = Instantiate(seaglide.GetComponentInChildren<VehicleInterface_MapController>().interfacePrefab);

            obj.transform.parent = transform;
            obj.transform.localPosition = Vector3.zero;

            hologram = obj.GetComponentInChildren<VehicleInterface_Terrain>();
            hologram.mapWorldRadius = 100;
            hologram.hologramRadius = 10f;
            hologram.InitializeHologram();
        }

        public void Start()
        {
            GameObject camObj = new GameObject("camera");
            camObj.transform.position = transform.position;
            camObj.transform.parent = transform;

            cam = camObj.AddComponent<Camera>();
            cam.fieldOfView = 90;
            cam.nearClipPlane = 0.01f;
            cam.farClipPlane = 10000f;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Color.black;

            camObj.transform.localPosition = new Vector3(0f, 0f, -distance);
            cam.targetTexture = new RenderTexture(2000,2000,1);
            tex = cam.targetTexture;

            main = this;
        }

        public void Update()
        {
            distance += -Input.mouseScrollDelta.y;
            distance = Mathf.Clamp(distance, 20f, 200f);
            cam.transform.localPosition = new Vector3(0f, 0f, -distance);

            Vector2 mouseDelta = oldMousePosition - new Vector2(-Input.mousePosition.x, Input.mousePosition.y);
            oldMousePosition = new Vector2(-Input.mousePosition.x, Input.mousePosition.y);

            if (Player.main)
            {
                transform.position = Player.main.transform.position;
                if(Input.GetMouseButton(0) && Player.main.GetPDA().isOpen)
                {
                    Vector3 angles = transform.eulerAngles;
                    angles.y += mouseDelta.x;
                    angles.x += mouseDelta.y;

                    // Clamp
                    float distTo90 = Mathf.Abs(90 - angles.x);
                    float distTo270 = Mathf.Abs(270 - angles.x);

                    if(angles.x > 90 && angles.x < 270)
                    {
                        if(distTo90 < distTo270)
                        {
                            angles.x = 90;
                        }
                        else
                        {
                            angles.x = 270;
                        }
                    }

                    transform.eulerAngles = angles;
                }
                if(Player.main.GetPDA().isOpen && !hologram)
                {
                    CreateMap();
                }
            }

            if(hologram)
            {
                hologram.mapWorldRadius = 100;
                hologram.hologramRadius = 10f;
            }
        }
    }
}