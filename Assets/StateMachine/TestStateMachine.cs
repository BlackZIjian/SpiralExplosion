using UnityEngine;
using System.Collections;
using System;

public class TestStateMachine : FSMSystem {
    public SuperCharacterController controller;
    Transform nowWeapon = null;
    void Start()
    {
        AddState(new TestState(this));
    }
    void LateUpdate()
    {
        foreach (Collider c in controller.triggerData)
        {
            if (c.tag == "Weapon")
            {
                if (nowWeapon == null)
                {
                    nowWeapon = c.gameObject.transform;
                    TriggerManagement.Triggering(new WeaponSuperControllerEventArgs(nowWeapon, true), "character");
                    return;
                }
                if (nowWeapon == c.gameObject.transform)
                {
                    return;
                }
            }
        }
        if (nowWeapon != null)
        {
            TriggerManagement.Triggering(new WeaponSuperControllerEventArgs(nowWeapon, false), "character");
            nowWeapon = null;
        }
    }
}
public class TestState : FSMState
{
    TestStateMachine tfsm;
    Transform nowWeapon = null;
    public TestState(TestStateMachine fsm)
    {
        stateID = StateID.NullStateID;
        tfsm = fsm;
    }
    public override void Act()
    {
        
    }
    public override void Reason()
    {

    }
}
