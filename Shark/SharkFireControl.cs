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
        public bool upgradeInstalled;
        public float cooldown = 0.25f;
        public GameObject leftSide;
        public GameObject rightSide;

        public GameObject weaponFXParent;
        public GameObject weaponModel;

        float nextShot = 0f;

        public void Awake()
        {
            leftSide = weaponFXParent.transform.Find("LeftBarrel").gameObject;
            rightSide = weaponFXParent.transform.Find("RightBarrel").gameObject;
        }

        public void Update()
        {
            weaponModel.SetActive(upgradeInstalled);
        }

        public void AttemptShoot()
        {
            if(!upgradeInstalled)
            {
                return;
            }

            if(Time.time >= nextShot)
            {
                ShootSide(currentSide ? leftSide : rightSide);
                nextShot = Time.time + cooldown;
                currentSide = !currentSide;
            }
        }

        public void ShootSide(GameObject side)
        {
            side.GetComponentInChildren<ParticleSystem>().Play();

            GameObject prefab = weaponFXParent.transform.Find("Bullet").gameObject;

            GameObject inst = Instantiate(prefab);
            inst.transform.parent = null;
            inst.transform.position = side.transform.position;
            inst.transform.rotation = side.transform.rotation;

            inst.SetActive(true);

            inst.AddComponent<BulletControl>();
            Destroy(inst, 5f);
        }
    }
}
