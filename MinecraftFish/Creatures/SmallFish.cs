using Nautilus.Assets;
using Nautilus.Utility;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MinecraftFish.Creatures
{
    public class SmallFish
    {
        public GameObject output = null;

        public IEnumerator CreatePrefab(string assetPath, PrefabInfo info)
        {
            GameObject mainGO = Plugin.bundle.LoadAsset<GameObject>(assetPath);

            Renderer[] renderers = mainGO.GetComponentsInChildren<Renderer>();

            
            foreach (Renderer renderer in renderers)
            {
                MaterialUtils.ApplyUBERShader(renderer.material, 0f, 0f, 0f, MaterialUtils.MaterialType.Cutout);

                renderer.material.SetFloat("_MyCullVariable", 0f);

                if (renderer.name.ToLower().Contains("overlay"))
                {
                    renderer.material.SetFloat("_ZOffset", -1f);
                }
            }
            


            Rigidbody rb = mainGO.EnsureComponent<Rigidbody>();
            rb.useGravity = false;
            rb.angularDrag = 1f;

            WorldForces forces = mainGO.EnsureComponent<WorldForces>();
            forces.useRigidbody = rb;
            forces.aboveWaterDrag = 0f;
            forces.aboveWaterGravity = 9.81f;
            forces.handleDrag = true;
            forces.handleGravity = true;
            forces.underwaterDrag = 1f;
            forces.underwaterGravity = 0;
            forces.waterDepth = Ocean.GetOceanLevel();
            forces.enabled = false;
            forces.enabled = true;

            mainGO.EnsureComponent<EntityTag>().slotType = EntitySlot.Type.Creature;
            mainGO.EnsureComponent<PrefabIdentifier>().ClassId = info.ClassID;
            mainGO.EnsureComponent<TechTag>().type = info.TechType;

            mainGO.EnsureComponent<SkyApplier>().renderers = renderers;
            mainGO.EnsureComponent<LargeWorldEntity>().cellLevel = LargeWorldEntity.CellLevel.Near;
            LiveMixin live = mainGO.EnsureComponent<LiveMixin>();

            var peeperTask = CraftData.GetPrefabForTechTypeAsync(TechType.Peeper);

            yield return peeperTask;

            GameObject peeper = peeperTask.GetResult();

            LiveMixinData peeperData = peeper.GetComponent<LiveMixin>().data;

            live.data = ScriptableObject.CreateInstance<LiveMixinData>();
            live.data.maxHealth = 10f;
            live.health = 10f;
            live.data.loopingDamageEffect = peeperData.loopingDamageEffect;
            live.data.damageEffect = peeperData.damageEffect;
            live.data.deathEffect = peeperData.deathEffect;
            live.data.electricalDamageEffect = peeperData.electricalDamageEffect;

            Creature creature = mainGO.EnsureComponent<Creature>();
            creature.initialCuriosity = AnimationCurve.Linear(0f, 0.5f, 1f, 0.5f);
            creature.initialFriendliness = AnimationCurve.Linear(0f, 0.5f, 1f, 0.5f);
            creature.initialHunger = AnimationCurve.Linear(0f, 0.5f, 1f, 0.5f);

            mainGO.EnsureComponent<WaterParkCreature>().data = peeper.GetComponent<WaterParkCreature>().data;

            SwimBehaviour behaviour = mainGO.EnsureComponent<SwimBehaviour>();
            mainGO.EnsureComponent<SwimRandom>();

            Locomotion loco = mainGO.EnsureComponent<Locomotion>();
            loco.useRigidbody = rb;

            mainGO.EnsureComponent<EcoTarget>().type = EcoTargetType.Peeper;
            mainGO.EnsureComponent<CreatureUtils>();
            mainGO.EnsureComponent<VFXSchoolFishRepulsor>();

            SplineFollowing spline = mainGO.EnsureComponent<SplineFollowing>();
            spline.locomotion = loco;
            spline.useRigidbody = rb;
            spline.levelOfDetail = mainGO.EnsureComponent<BehaviourLOD>();
            spline.GoTo(mainGO.transform.position + mainGO.transform.forward, mainGO.transform.forward, 5f);
            behaviour.splineFollowing = spline;

            creature.ScanCreatureActions();

            mainGO.AddComponent<Pickupable>().isPickupable = true;

            CustomPrefab prefab = new CustomPrefab(info);

            prefab.SetGameObject(mainGO);
            prefab.Register();

            output = mainGO;

            Plugin.Logger.LogInfo("Finished making prefab for: " + info.ClassID);
        }
    }
}
