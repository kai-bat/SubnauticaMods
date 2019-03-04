using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Shark
{
    [RequireComponent(typeof(Shark))]
    public class BoostDisplay : MonoBehaviour
    {
        Shark shark;
        TextMesh text;

        public void Awake()
        {
            shark = GetComponent<Shark>();
        }

        public void Update()
        {
            if(text)
            {
                text.text = (shark.boostCharge * 100f).ToString();
            }
        }
    }
}
