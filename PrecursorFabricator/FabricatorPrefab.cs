using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SMLHelper.V2.Assets;
using SMLHelper.V2.Handlers;
using FMODUnity;

namespace PrecursorFabricator
{
    public class FabricatorPrefab : ModPrefab
    {
        public static GameObject prefabCache;

        public FabricatorPrefab(string classId, string prefabFileName, TechType techType = TechType.None) : base(classId, prefabFileName, techType)
        {
        }

        public override GameObject GetGameObject()
        {
            if(prefabCache)
            {
                return prefabCache;
            }

            GameObject prefab = new GameObject("Fab Parent");
            GameObject obj = GameObject.Instantiate<GameObject>(MainPatch.bundle.LoadAsset<GameObject>("PrecursorFabricator.prefab"));
            obj.name = "model";
            obj.transform.parent = prefab.transform;
            obj.transform.localPosition = Vector3.zero;
            foreach(MeshRenderer renderer in obj.GetComponentsInChildren<MeshRenderer>())
            {
                renderer.material.shader = Shader.Find("MarmosetUBER");

            }

            var sky = prefab.AddComponent<SkyApplier>();
            sky.renderers = obj.GetComponentsInChildren<Renderer>();
            sky.anchorSky = Skies.Auto;
            prefab.AddComponent<TechTag>().type = TechType;
            prefab.AddComponent<PrefabIdentifier>().ClassId = ClassID;

            Rigidbody body = prefab.AddComponent<Rigidbody>();
            body.isKinematic = true;

            prefab.AddComponent<LargeWorldEntity>().cellLevel = LargeWorldEntity.CellLevel.Medium;

            Constructable con = prefab.AddComponent<Constructable>();
            con.techType = TechType;
            con.allowedInBase = true;
            con.allowedInSub = true;
            con.allowedOnWall = true;
            con.allowedOutside = false;
            con.allowedOnCeiling = false;
            con.allowedOnGround = false;
            con.model = obj;
            con.rotationEnabled = false;
            prefab.AddComponent<ConstructableBounds>();

            Fabricator originalFab = CraftData.GetPrefabForTechType(TechType.Fabricator).GetComponent<Fabricator>();

            CrafterLogic logic = prefab.AddComponent<CrafterLogic>();
            Fabricator fab = prefab.AddComponent<Fabricator>();
            fab.animator = prefab.GetComponentInChildren<Animator>();
            fab.animator.runtimeAnimatorController = originalFab.animator.runtimeAnimatorController;
            fab.animator.avatar = originalFab.animator.avatar;
            fab.sparksPS = originalFab.sparksPS;
            fab.craftTree = MainPatch.fabType;

            Material beamMat = new Material(originalFab.leftBeam.GetComponent<MeshRenderer>().material);
            beamMat.color = Color.green;

            fab.leftBeam = obj.transform.Find("FabricatorMain/submarine_fabricator_01/fabricator/overhead/printer_left/fabricatorBeam").gameObject;
            fab.leftBeam.GetComponent<MeshRenderer>().material = beamMat;
            fab.rightBeam = obj.transform.Find("FabricatorMain/submarine_fabricator_01/fabricator/overhead/printer_right/fabricatorBeam 1").gameObject;
            fab.rightBeam.GetComponent<MeshRenderer>().material = beamMat;
            fab.animator.SetBool(AnimatorHashID.open_fabricator, false);
            fab.crafterLogic = logic;
            fab.ghost = prefab.AddComponent<CrafterGhostModel>();
            fab.ghost.itemSpawnPoint = obj.transform.Find("FabricatorMain/submarine_fabricator_01/fabricator/printBed/spawnPoint");

            fab.openSound = originalFab.openSound;
            fab.closeSound = originalFab.closeSound;
            fab.soundOrigin = fab.transform;
            fab.fabricateSound = obj.AddComponent<StudioEventEmitter>();
            fab.fabricateSound.Event = originalFab.fabricateSound.Event;

            fab.fabLight = obj.transform.Find("FabLight").GetComponent<Light>();

            prefabCache = prefab;

            return prefab;
        }
    }
}
