using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AlienRifle
{
    class LineFade : MonoBehaviour
    {
        LineRenderer line;

        void Start()
        {
            line = GetComponent<LineRenderer>();
        }

        void Update()
        {
            line.material.color = Color.Lerp(line.material.color, Color.clear, 0.1f);
        }
    }
}
