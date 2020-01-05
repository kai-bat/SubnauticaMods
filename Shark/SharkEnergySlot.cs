using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Shark
{
    public class SharkEnergySlot : HandTarget, IHandTarget
    {
        public Shark shark;
        public Material crystalMat;
        public Color baseCol;
        public Color squareCol;
        public Color detailCol;
        public Vector4 noiseSpeed;
        public float squareSpeed;

        Color lerpToCol = new Color(0.2f, 0.2f, 0.2f);

        public override void Awake()
        {
            base.Awake();

            crystalMat = transform.parent.Find("PowerCube").GetComponent<MeshRenderer>().material;
            baseCol = crystalMat.color;
            squareCol = crystalMat.GetColor("_SquaresColor");
            detailCol = crystalMat.GetColor("_DetailsColor");
            squareSpeed = crystalMat.GetFloat("_SquaresSpeed");
            noiseSpeed = crystalMat.GetVector("_NoiseSpeed");
        }

        public void Update()
        {
            EnergyMixin mixin = shark.energyInterface.sources[0];

            crystalMat.color = Color.Lerp(baseCol, lerpToCol, 1f - mixin.charge / mixin.capacity);
            crystalMat.SetColor("_SquaresColor", Color.Lerp(squareCol, lerpToCol, 1f - mixin.charge / mixin.capacity));
            crystalMat.SetColor("_DetailsColor", Color.Lerp(detailCol, lerpToCol, 1f - mixin.charge / mixin.capacity));
            crystalMat.SetFloat("_SquaresSpeed", Mathf.Lerp(squareSpeed, 0f, 1f - mixin.charge / mixin.capacity));
            crystalMat.SetVector("_NoiseSpeed", Vector4.Lerp(noiseSpeed, Vector4.zero, 1f - mixin.charge / mixin.capacity));
        }

        public void OnHandClick(GUIHand hand)
        {
            EnergyMixin mixin = shark.energyInterface.sources[0];

            if(mixin.HasItem() && mixin.charge == 0f)
            {
                InventoryItem item = mixin.batterySlot.storedItem;

                mixin.batterySlot.RemoveItem();

                Destroy(item.item.gameObject);

                Pickupable ionCube = CraftData.InstantiateFromPrefab(Shark.depletedIonCube).GetComponent<Pickupable>();
                ionCube.transform.position = transform.position + transform.up;

                if(Player.main.HasInventoryRoom(ionCube))
                {
                    Inventory.main.Pickup(ionCube);
                }
            }

            if(!mixin.HasItem())
            {
                if(Inventory.main.GetPickupCount(TechType.PrecursorIonCrystal) >= 1)
                {
                    Inventory.main.container.RemoveItem(TechType.PrecursorIonCrystal);

                    Pickupable battery = CraftData.InstantiateFromPrefab(Shark.internalBattery).GetComponent<Pickupable>();

                    InventoryItem item = mixin.batterySlot.AddItem(battery);
                    mixin.NotifyHasBattery(item);
                }
            }
        }

        public void OnHandHover(GUIHand hand)
        {
            HandReticle reticle = HandReticle.main;
            if(!reticle)
            {
                return;
            }

            EnergyMixin mixin = shark.energyInterface.sources[0];

            if (mixin.HasItem())
            {
                if (mixin.charge > 0f)
                {
                    isValidHandTarget = false;
                    reticle.SetInteractText("Ion Cube", $"{Mathf.Round(mixin.charge / mixin.capacity * 100f)}% Remaining", false, false, HandReticle.Hand.None);
                }
                else
                {
                    isValidHandTarget = true;
                    reticle.SetInteractText("Ion Cube", $"Click to remove", false, false, HandReticle.Hand.Left);
                }
            }
            else
            {
                isValidHandTarget = true;
                reticle.SetInteractText("Insert Ion Cube", false, HandReticle.Hand.Left);
            }
        }
    }
}
