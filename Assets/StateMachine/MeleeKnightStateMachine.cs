using UnityEngine;
using System.Collections;
using System;


public struct AIInput
{
    public Vector2 AIInputV;
    public bool AIInputJump;
}
public class MeleeKnightStateMachine : FSMSystem {
    public MeleeAISubStateMachine ai;
    public MeleeMoveSubStateMachine move;
    public AIInput ai_input;
    public MeleeKnightStateMachine(SuperCharacterController controller)
    {
        ai_input = new AIInput();
        ai = new MeleeAISubStateMachine(controller,this);
        move = new MeleeMoveSubStateMachine(controller);
        AddState(new MeleeKnightState(this));
    }
}

class MeleeKnightState : FSMState
{
    MeleeKnightStateMachine stateMachine;
    public MeleeKnightState(MeleeKnightStateMachine fsm)
    {
        stateMachine = fsm;
    }
    public override void Act()
    {
        stateMachine.ai.CurrentState.Act();
        stateMachine.move.CurrentState.Act();
    }

    public override void Reason()
    {
        stateMachine.ai.CurrentState.Reason();
        stateMachine.move.CurrentState.Reason();
    }
}

public class MeleeAISubStateMachine : FSMSystem
{
    public MeleeKnightStateMachine fatherFSM;
    public MeleeAISubStateMachine(SuperCharacterController controller, MeleeKnightStateMachine fatherFSM)
    {
        AddState(new MeleeKnightIdleState(controller, this));
        this.fatherFSM = fatherFSM;
    }
}

//等待开发
public class MeleeMoveSubStateMachine : FSMSystem
{
    public MeleeMoveSubStateMachine(SuperCharacterController controller)
    {

    }
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
            aiFSM.fatherFSM.ai_input.AIInputV = new Vector2(0, 0);
            int r = UnityEngine.Random.Range(0, 100);
            if(r > 70)
            {
                remainTime = 1;
                aiFSM.fatherFSM.ai_input.AIInputV = new Vector2(UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1));
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


