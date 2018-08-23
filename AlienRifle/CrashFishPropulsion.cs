using System;
using UnityEngine;
using System.Reflection;

public class CrashFishPropulsion : MonoBehaviour, IPropulsionCannonAmmo
{
    Crash fish;
    MethodBase onstatemethod;

    void Start()
    {
        fish = GetComponent<Crash>();
        onstatemethod = typeof(Crash).GetMethod("OnState", BindingFlags.NonPublic);
    }

    void IPropulsionCannonAmmo.OnGrab()
    {
        onstatemethod.Invoke(fish,new object[] { Crash.State.FrozenInTime, true });
        fish.CancelInvoke("Inflate");
        fish.CancelInvoke("AnimateInflate");
        fish.CancelInvoke("Detonate");
        fish.CancelInvoke("OnCalmDown");
    }

    void IPropulsionCannonAmmo.OnShoot()
    {
        onstatemethod.Invoke(fish, new object[] { Crash.State.DetonateOnImpact, true });
    }

    void IPropulsionCannonAmmo.OnRelease()
    {
    }

    void IPropulsionCannonAmmo.OnImpact()
    {
        typeof(Crash).GetMethod("Destroy",BindingFlags.NonPublic).Invoke(fish,new object[] { });
    }

    bool IPropulsionCannonAmmo.GetAllowedToGrab()
    {
        return true;
    }

    bool IPropulsionCannonAmmo.GetAllowedToShoot()
    {
        return true;
    }
}
