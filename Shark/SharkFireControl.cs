using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Shark
{
    public class SharkFireControl : MonoBehaviour
    {
        public bool currentSide = true;
        public bool upgradeInstalled
        {
            get { return weaponParent.activeSelf; }
            set { weaponParent.SetActive(value);  }
        }
        public float cooldown = 0f;

        public GameObject weaponParent;

        public void AttemptShoot()
        {
            if(!upgradeInstalled)
            {
                return;
            }
        }
    }
}
