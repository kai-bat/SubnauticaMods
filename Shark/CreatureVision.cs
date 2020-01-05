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
            var originalRenderer = GetComponent<Renderer>();

            if (originalRenderer.GetType() == typeof(SkinnedMeshRenderer))
            {
                GameObject newGO = new GameObject("precursorvisionmesh");
                newGO.transform.parent = originalRenderer.transform;
                newGO.transform.localPosition = Vector3.zero;
                newGO.transform.localEulerAngles = Vector3.zero;
                newGO.layer = 23;

                var newSkin = newGO.AddComponent<SkinnedMeshRenderer>();
                var skinnedMesh = originalRenderer as SkinnedMeshRenderer;
                newSkin.sharedMesh = skinnedMesh.sharedMesh;
                newSkin.material = MainPatch.seeThroughMat;
                newSkin.enabled = false;
                newSkin.rootBone = skinnedMesh.rootBone;
                newSkin.bones = skinnedMesh.bones;

                seeThroughRenderer = newSkin;
            }
            /*
            else if(originalRenderer.GetType() == typeof(MeshRenderer))
            {
                GameObject newGO = new GameObject("precursorvisionmesh");
                newGO.transform.parent = originalRenderer.transform;
                newGO.transform.localPosition = Vector3.zero;
                newGO.transform.localEulerAngles = Vector3.zero;
                newGO.layer = 23;

                var newRend = newGO.AddComponent<MeshRenderer>();
                var newMesh = newGO.AddComponent<MeshFilter>();
                var oldMesh = originalRenderer.GetComponent<MeshFilter>();
                newMesh.sharedMesh = oldMesh.sharedMesh;
                newRend.material = MainPatch.seeThroughMat;
                newRend.enabled = false;

                seeThroughRenderer = newRend;
            }
            */
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
