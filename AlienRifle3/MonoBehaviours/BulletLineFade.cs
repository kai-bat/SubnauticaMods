using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AlienRifle3.MonoBehaviours
{
    public class BulletLineFade : MonoBehaviour
    {
        public LineRenderer line;

        public float lineWidth = 0.1f;
        public float timeToFade = 1f;

        public float vibrateInterval = 1/30f;
        public float nextVibrate = 0f;

        public float vibrateSize = 0.1f;

        float fadeProgress = 0f;
        public bool vibrate = false;

        public void Awake()
        {
            line = GetComponent<LineRenderer>();
            nextVibrate = Time.time + vibrateInterval;
        }

        public void Update()
        {
            fadeProgress += Time.deltaTime;

            if(nextVibrate < Time.time)
            {
                vibrate = !vibrate;
                nextVibrate = Time.time + vibrateInterval;
            }

            line.widthMultiplier = Mathf.Lerp(lineWidth + (vibrate ? vibrateSize : 0f), 0f, fadeProgress/timeToFade);
            if(fadeProgress >= timeToFade)
            {
                Destroy(gameObject);
            }
        }
    }
}