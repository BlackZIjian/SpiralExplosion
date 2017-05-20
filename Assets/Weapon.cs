using UnityEngine;
using System.Collections;
using System;

public class Weapon : MonoBehaviour {
    public Transform owner;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
       
	}
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Transform attackDirection = transform.FindChild("AttackDirection");
        try
        {
            Gizmos.DrawLine(attackDirection.position, attackDirection.position + attackDirection.forward);
        }
        catch
        {
            
        }
    }
    public Vector3 GetAttackDirection()
    {
        Transform attackDirection = transform.FindChild("AttackDirection");
        try
        {
            return attackDirection.forward;
        }
        catch(Exception e)
        {
            throw e;
        }
    }
}
