using UnityEngine;
using System.Collections;
using System;

public class TestStateMachine : FSMSystem {
    public SuperCharacterController controller;
    void Start()
    {
        AddState(new TestState(this));
    }

}
public class TestState : FSMState
{
    TestStateMachine tfsm;
    public TestState(TestStateMachine fsm)
    {
        stateID = StateID.NullStateID;
        tfsm = fsm;
        TriggerManagement.AddObserver("character", new OnTrigger(test));
    }
    public override void Act()
    {
        foreach(Collider c in tfsm.controller.triggerData)
        {
            if(c.tag == "Weapon")
            {
                TriggerManagement.Triggering(new WeaponSuperControllerEventArgs(c.gameObject.transform,c,null), "character");
            }
        }
    }
    public override void Reason()
    {

    }
    public void test(TriggerEventArgs args)
    {
        WeaponSuperControllerEventArgs arg = (WeaponSuperControllerEventArgs)args;
        Debug.Log(arg.weapon.position);
    }
}
