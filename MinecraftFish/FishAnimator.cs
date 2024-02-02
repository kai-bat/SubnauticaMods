using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MinecraftFish
{
    public class FishAnimator : MonoBehaviour
    {
        SwimRandom swim;
        Animator animator;
        Rigidbody rb;
        LiveMixin live;

        public string speedFloatName;

        public void Awake()
        {
            swim = GetComponent<SwimRandom>();
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody>();
            live = GetComponent<LiveMixin>();
        }

        public void Update()
        {
            float speed = live.IsAlive() ? Mathf.Clamp01(rb.velocity.magnitude / swim.swimVelocity) : 0f;

            animator.SetFloat(speedFloatName, speed);
        }
    }
}
