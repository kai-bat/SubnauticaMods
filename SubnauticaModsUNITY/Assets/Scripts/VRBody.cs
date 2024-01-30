using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRBody : MonoBehaviour
{
    public Transform vrcamera;
    public float height;

    public Transform leftHand;
    public Transform rightHand;

    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
        anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
        anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
        anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);

        anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHand.position);
        anim.SetIKPosition(AvatarIKGoal.RightHand, rightHand.position);
        anim.SetIKRotation(AvatarIKGoal.LeftHand, leftHand.rotation);
        anim.SetIKRotation(AvatarIKGoal.RightHand, rightHand.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 euler = vrcamera.eulerAngles;
        euler.x = 0f;
        euler.z = 0f;

        Vector3 vector = Quaternion.Euler(euler) * Vector3.forward;

        transform.position = vrcamera.position - vector * 0.1f;
        transform.position = new Vector3(transform.position.x, height, transform.position.z);

        transform.eulerAngles = new Vector3(0f, vrcamera.eulerAngles.y, 0f);
    }
}
