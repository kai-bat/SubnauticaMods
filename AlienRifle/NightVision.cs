using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AlienRifle
{
    public class NightVision : MonoBehaviour
    {
        public bool activated = false;

        float luminance = 0.1f;
        float noiseFactor = 0.003f;

        Material mat;

        void Start ()
        {
            mat = AlienRifleMod.bund.LoadAsset<Material>("NVMat.mat");
            mat.SetVector("lum", new Vector4(luminance, luminance, luminance, luminance));
            mat.SetFloat("noiseFactor", noiseFactor);
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if(activated)
            {
                mat.SetFloat("time", Mathf.Sin(Time.time * Time.deltaTime));
                Graphics.Blit(source, destination, mat);
                RenderSettings.ambientLight = Color.white;
            }
            else
            {
                Graphics.Blit(source, destination);
                RenderSettings.ambientLight = Color.black;
            }
        }
    }
}
