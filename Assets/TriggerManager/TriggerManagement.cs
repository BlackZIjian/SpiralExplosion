using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TriggerEventArgs : EventArgs
{

}
public class WeaponSuperControllerEventArgs : TriggerEventArgs
{
    public Transform weapon;
    public Collider weaponCollider;
    public CollisionSphere characterSphere;
    public WeaponSuperControllerEventArgs(Transform weapon, Collider weaponCollider, CollisionSphere characterSphere)
    {
        this.weapon = weapon;
        this.weaponCollider = weaponCollider;
        this.characterSphere = characterSphere;
    }
}
public delegate void OnTrigger(TriggerEventArgs args);
public class TriggerManagement : MonoBehaviour {
    private static Dictionary<string, OnTrigger> observers = new Dictionary<string, OnTrigger>();
    public static void Triggering(TriggerEventArgs args ,params string[] observerLayers)
    {
        foreach(string s in observerLayers)
        {
            OnTrigger ot;
           if(observers.TryGetValue(s,out ot))
            {
                ot(args);
            }
           
        }
    }
    public static void AddObserver(string layer,OnTrigger func)
    {
        OnTrigger ot;
        observers.TryGetValue(layer, out ot);
        if(ot != null)
        {
            ot += func;
        }
        else
        {
            ot = func;
        }
        observers[layer] = ot;
    }
}
