using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Shark
{
    public class BulletControl : MonoBehaviour
    {
        public Rigidbody body;

        public void Update()
        {
            body.velocity = body.velocity.normalized * 150f;
        }

        public void OnCollisionEnter(Collision col)
        {
            LiveMixin mixin = UWE.Utils.GetComponentInHierarchy<LiveMixin>(col.gameObject);



            if (mixin)
            {
                mixin.TakeDamage(100f, transform.position, DamageType.LaserCutter);
                Destroy(GetComponent<SphereCollider>());
                Destroy(body);
                Destroy(gameObject, 0.2f);
            }
        }
    }
}
