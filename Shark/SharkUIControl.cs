using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Shark
{
    public class SharkUIControl : MonoBehaviour
    {
        public Shark shark;

        public Canvas canvas;

        public RectTransform powerScale;
        public RectTransform chargeScale;
        public RectTransform duraScale;
        public Image chargeOutline;

        public void Awake()
        {
            canvas = transform.Find("Scaler/Canvas").GetComponent<Canvas>();

            powerScale = canvas.transform.Find("Panel/Power/Meter/Scale") as RectTransform;
            chargeScale = canvas.transform.Find("Panel/Charge/Meter/Scale") as RectTransform;
            chargeOutline = canvas.transform.Find("Panel/Charge/Meter/Outline").GetComponent<Image>();
            duraScale = canvas.transform.Find("Panel/Durability/Meter/Scale") as RectTransform;
        }

        public void Update()
        {
            chargeScale.localScale = new Vector3(shark.boostCharge, 1f, 1f);
            chargeOutline.enabled = shark.isBoosting;
            shark.energyInterface.GetValues(out float charge, out float capacity);
            if (GameModeUtils.RequiresPower())
            {
                powerScale.localScale = new Vector3(charge / capacity, 1f, 1f);
            }
            else
            {
                powerScale.localScale = Vector3.one;
            }

            duraScale.localScale = new Vector3(shark.liveMixin.health / shark.liveMixin.maxHealth, 1f, 1f);
        }
    }
}
