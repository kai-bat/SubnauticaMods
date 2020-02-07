using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Shark
{
    public class SharkBladeControl : MonoBehaviour
    {
        public Material bladeMat;
        public Texture alpha;
        public bool deployed = false;

        float deployLerp = 0f;

        public void Awake()
        {
            bladeMat.EnableKeyword("FX_BUILDING");
            bladeMat.SetTexture(ShaderPropertyID._EmissiveTex, alpha);
            bladeMat.SetFloat(ShaderPropertyID._Cutoff, 0.4f);
            bladeMat.SetColor(ShaderPropertyID._BorderColor, new Color(0.4f, 1f, 0.4f, 1f));
            bladeMat.SetFloat(ShaderPropertyID._Built, 0f);
            bladeMat.SetFloat(ShaderPropertyID._Cutoff, 0.42f);
            bladeMat.SetVector(ShaderPropertyID._BuildParams, new Vector4(2f, 0.7f, 3f, -0.25f));
            bladeMat.SetFloat(ShaderPropertyID._NoiseStr, 0.25f);
            bladeMat.SetFloat(ShaderPropertyID._NoiseThickness, 0.49f);
            bladeMat.SetFloat(ShaderPropertyID._BuildLinear, 0f);
            bladeMat.SetFloat(ShaderPropertyID._MyCullVariable, 0f);
            bladeMat.EnableKeyword("FX_CONSTRUCTING_ALPHA");
        }

        public void Update()
        {
            bladeMat.EnableKeyword("FX_BUILDING");
            bladeMat.SetTexture(ShaderPropertyID._EmissiveTex, alpha);
            bladeMat.SetFloat(ShaderPropertyID._Cutoff, 0.4f);
            bladeMat.SetColor(ShaderPropertyID._BorderColor, new Color(0.4f, 1f, 0.4f, 1f));
            bladeMat.SetFloat(ShaderPropertyID._Built, 0f);
            bladeMat.SetFloat(ShaderPropertyID._Cutoff, 0.42f);
            bladeMat.SetVector(ShaderPropertyID._BuildParams, new Vector4(2f, 0.7f, 3f, -0.25f));
            bladeMat.SetFloat(ShaderPropertyID._NoiseStr, 0.25f);
            bladeMat.SetFloat(ShaderPropertyID._NoiseThickness, 0.49f);
            bladeMat.SetFloat(ShaderPropertyID._BuildLinear, 0f);
            bladeMat.SetFloat(ShaderPropertyID._MyCullVariable, 0f);
            bladeMat.EnableKeyword("FX_CONSTRUCTING_ALPHA");
            if (Input.GetKeyDown(KeyCode.J))
            {
                deployed = !deployed;
                ErrorMessage.AddMessage("DEPLOYED NOW: " + deployed);
            }

            if (deployed)
            {
                deployLerp = Mathf.MoveTowards(deployLerp, 1f, Time.deltaTime);
            }
            else
            {
                deployLerp = Mathf.MoveTowards(deployLerp, 0f, Time.deltaTime);
            }

            Shader.SetGlobalFloat(ShaderPropertyID._SubConstructProgress, deployLerp);
        }
    }
}
