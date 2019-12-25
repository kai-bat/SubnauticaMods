using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Shark
{
    public class CreatureVision : MonoBehaviour
    {
        public Renderer seeThroughRenderer;
        

        public void Awake()
        {
            var originalRenderer = GetComponent<SkinnedMeshRenderer>();

            if (originalRenderer)
            {
                GameObject newGO = new GameObject("precursorvisionmesh");
                newGO.transform.parent = originalRenderer.transform;
                newGO.transform.localPosition = Vector3.zero;
                newGO.transform.localEulerAngles = Vector3.zero;
                newGO.layer = 23;

                var newSkin = newGO.AddComponent<SkinnedMeshRenderer>();
                newSkin.sharedMesh = originalRenderer.sharedMesh;
                newSkin.material = MainPatch.seeThroughMat;
                newSkin.enabled = false;
                newSkin.rootBone = originalRenderer.rootBone;
                newSkin.bones = originalRenderer.bones;

                seeThroughRenderer = newSkin;
            }
            else
            {
                Destroy(this);
            }
        }

        public void Update()
        {
            seeThroughRenderer.enabled = SharkVisionControl.active;
        }
    }
}
