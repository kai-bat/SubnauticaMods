using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Shark
{
    public class SharkShieldControl : MonoBehaviour
    {
        public bool active;

        public GameObject shieldRenderer;

        public void Update()
        {
            shieldRenderer.SetActive(active);
        }
    }
}
