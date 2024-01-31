using Nautilus.Assets;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Nautilus.Utility;

namespace MinecraftFish.Creatures
{
    public static class CodFish
    {
        public static PrefabInfo Info = PrefabInfo.WithTechType("CodFish", "Cod", "Blocky cod fish")
            .WithIcon(ImageUtils.LoadSpriteFromFile(Plugin.modFolder + "cod.png"));

        public static IEnumerator Register()
        {
            GameObject mainGO = new GameObject("CodFish");
            mainGO.SetActive(false);
            GameObject modelPrefab = UnityEngine.Object.Instantiate(Plugin.bundle.LoadAsset<GameObject>("Assets/Prefabs/CodFish.prefab"));
            modelPrefab.transform.SetParent(mainGO.transform, false);
            modelPrefab.transform.localScale = Vector3.one*0.1f;
            modelPrefab.transform.localPosition = Vector3.zero;
            modelPrefab.transform.localRotation = Quaternion.identity;

            Rigidbody oldRB = modelPrefab.GetComponent<Rigidbody>();
            if(oldRB != null)
            {
                UnityEngine.Object.Destroy(oldRB);
            }

            Renderer[] renderers = mainGO.GetComponentsInChildren<Renderer>();

            MaterialUtils.ApplySNShaders(mainGO, 0f, 0f, 0f);

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
            mainGO.EnsureComponent<PrefabIdentifier>().ClassId = Info.ClassID;
            mainGO.EnsureComponent<TechTag>().type = Info.TechType;

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
            SwimRandom swim = mainGO.EnsureComponent<SwimRandom>();
            swim.swimVelocity = 7f;
            swim.swimRadius = Vector3.one*10f;
            swim.swimInterval = 0.7f;

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

            CustomPrefab prefab = new CustomPrefab(Info);

            prefab.SetGameObject(mainGO);
            prefab.Register();

            SpawnFish.fishes.Add(Info.TechType);

            yield return null;
        }
    }
}
