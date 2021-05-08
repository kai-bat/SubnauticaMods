using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Shark
{
    public class SharkDrillable : MonoBehaviour, IManagedUpdateBehaviour, IManagedBehaviour
    {
        public Drillable drill;
        public Shark drillingShark;

        public int managedUpdateIndex { get; set; }

        public void Awake()
        {
            drill = GetComponent<Drillable>();
        }

        public void OnDestroy()
        {
            BehaviourUpdateUtils.Deregister(this);
        }

        public void OnDisable()
        {
            BehaviourUpdateUtils.Deregister(this);
        }

        public void Drill(Vector3 position, Shark shark, out GameObject hitObj)
        {
            float totalhp = 0f;
            foreach(float hp in drill.health)
            {
                totalhp += hp;
            }
            drillingShark = shark;
            int meshindex = drill.FindClosestMesh(position, out Vector3 zero);
            hitObj = drill.renderers[meshindex].gameObject;
            drill.timeLastDrilled = Time.time;
            if(totalhp > 0)
            {
                float meshhealth = drill.health[meshindex];
                drill.health[meshindex] = Mathf.Max(0f, drill.health[meshindex] - 5f);
                totalhp -= meshhealth - drill.health[meshindex];
                if (meshhealth > 0f && drill.health[meshindex] <= 0f)
                {
                    drill.renderers[meshindex].gameObject.SetActive(false);
                    drill.SpawnFX(drill.breakFX, zero);
                    if (UnityEngine.Random.value < drill.kChanceToSpawnResources)
                    {
                        drill.SpawnLoot(zero);
                    }
                }
                if(totalhp <= 0f)
                {
                    drill.SpawnFX(drill.breakAllFX, zero);
                    if (typeof(Drillable).GetField("onDrilled").GetValue(drill) is Drillable.OnDrilled onDrilled)
                    {
                        onDrilled(drill);
                    }
                    if (drill.deleteWhenDrilled)
                    {
                        float time = drill.lootPinataOnSpawn ? 6f : 0f;
                        drill.Invoke("DestroySelf", time);
                    }
                }
            }
            BehaviourUpdateUtils.Register(this);
        }

        public string GetProfileTag()
        {
            return "SharkDrillable";
        }

        public void ManagedUpdate()
        {
            if (drill.timeLastDrilled + 0.5f > Time.time)
            {
                drill.modelRoot.transform.position = transform.position + new Vector3(Mathf.Sin(Time.time * 60f), Mathf.Cos(Time.time * 58f + 0.5f), Mathf.Cos(Time.time * 64f + 2f)) * 0.011f;
            }
            if (drill.lootPinataObjects.Count > 0 && drillingShark)
            {
                List<GameObject> list = new List<GameObject>();
                foreach (GameObject loot in drill.lootPinataObjects)
                {
                    if (loot == null)
                    {
                        list.Add(loot);
                    }
                    else
                    {
                        Vector3 b = drillingShark.transform.position;
                        loot.transform.position = Vector3.Lerp(loot.transform.position, b, Time.deltaTime * 5f);
                        if (Vector3.Distance(loot.transform.position, b) < 5f)
                        {
                            Pickupable pickupable = loot.GetComponentInChildren<Pickupable>();
                            if (pickupable)
                            {
                                if (!drillingShark.drill.storageContainer.container.HasRoomFor(pickupable))
                                {
                                    if (Player.main.GetVehicle() == drillingShark)
                                    {
                                        ErrorMessage.AddMessage(Language.main.Get("ContainerCantFit"));
                                    }
                                }
                                else
                                {
                                    string arg = Language.main.Get(pickupable.GetTechName());
                                    ErrorMessage.AddMessage(Language.main.GetFormat<string>("VehicleAddedToStorage", arg));
                                    uGUI_IconNotifier.main.Play(pickupable.GetTechType(), uGUI_IconNotifier.AnimationType.From, null);
                                    pickupable = pickupable.Initialize();
                                    InventoryItem item = new InventoryItem(pickupable);
                                    drillingShark.drill.storageContainer.container.UnsafeAdd(item);
                                    pickupable.PlayPickupSound();
                                }
                                list.Add(loot);
                            }
                        }
                    }
                }
                if (list.Count > 0)
                {
                    foreach (GameObject item2 in list)
                    {
                        drill.lootPinataObjects.Remove(item2);
                    }
                }
            }
        }
    }
}
