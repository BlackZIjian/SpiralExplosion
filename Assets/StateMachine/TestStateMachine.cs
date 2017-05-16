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
    }
    public override void Act()
    {
        foreach(Collider c in tfsm.controller.triggerData)
        {
            if(c.tag == "Weapon")
            {
                Debug.Log("被击中");
            }
        }
    }
    public override void Reason()
    {

    }
}
