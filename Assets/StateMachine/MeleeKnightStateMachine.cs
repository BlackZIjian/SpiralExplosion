using UnityEngine;
using System.Collections;
using System;


public struct AIInput
{
    public Vector2 AIInputV;
    public bool AIInputJump;
}

public class MeleeAISubStateMachine : FSMSystem
{
    public MeleeMoveSubStateMachine moveFSM;
    public SuperCharacterController MeleeController;
    public AIInput ai_input = new AIInput();
    public void Start()
    {
        moveFSM = gameObject.AddComponent<MeleeMoveSubStateMachine>();
        MeleeController.fsm = moveFSM;
        AddState(new MeleeKnightIdleState(MeleeController, this));
        
    }
    public void Update()
    {
        CurrentState.Reason();
        CurrentState.Act();
    }
}

//等待开发
public class MeleeMoveSubStateMachine : FSMSystem
{
    
}

//正在开发
public class MeleeKnightIdleState : FSMState
{
    public SuperCharacterController controller;
    float ronateSpeed;
    SightCone sight;
    float remainTime = 0;
    
    public MeleeKnightIdleState(SuperCharacterController c, FSMSystem f)
    {
        fsm = f;
        controller = c;
        stateID = StateID.MeleeKnightIdle;
        AddTransition(Transition.MeleeKnightSeeSomeOne, StateID.MeleeKnightSeeSomeOne);
    }
    public override void Reason()
    {

    }

    public override void Act()
    {
        MeleeAISubStateMachine aiFSM = (MeleeAISubStateMachine)fsm;
        if(remainTime <= 0)
        {
            aiFSM.ai_input.AIInputV = new Vector2(0, 0);
            int r = UnityEngine.Random.Range(0, 100);
            if(r > 70)
            {
                remainTime = 1;
                aiFSM.ai_input.AIInputV = new Vector2(UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1));
            }
        }
        else
        {
            remainTime -= controller.deltaTime;
        }
        
    }
    public override void DoBeforeEntering()
    {
        sight = controller.gameObject.GetComponent<SightCone>();
    }
}


