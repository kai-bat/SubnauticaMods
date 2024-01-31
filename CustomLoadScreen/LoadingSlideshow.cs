using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

using Random = UnityEngine.Random;

namespace CustomLoadScreen
{
    public class LoadingSlideshow : MonoBehaviour
    {
        public Image image;
        public List<Sprite> backgrounds;

        public float duration = 4f;

        float switchTime = 0f;

        int lastimg = 0;

        public void Start()
        {
            int rand = Random.Range(0, backgrounds.Count);
            
            image.sprite = backgrounds[rand];

            lastimg = rand;
        }

        public void Update()
        {
            switchTime += Time.unscaledDeltaTime;

            if(switchTime >= duration)
            {
                switchTime = 0f;

                int rand = lastimg;

                while (rand == lastimg)
                {
                    rand = Random.Range(0, backgrounds.Count);
                }
                lastimg = rand;
                image.sprite = backgrounds[rand];
            }
        }
    }
}
