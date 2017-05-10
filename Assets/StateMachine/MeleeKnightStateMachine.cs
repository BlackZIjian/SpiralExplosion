using UnityEngine;
using System.Collections;
using System;

public class MeleeKnightStateMachine : FSMSystem {
    public MeleeAISubStateMachine ai;
    public MeleeMoveSubStateMachine move;
    public MeleeKnightStateMachine(SuperCharacterController controller)
    {
        ai = new MeleeAISubStateMachine(controller);
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
    public MeleeAISubStateMachine(SuperCharacterController controller)
    {
        AddState(new MeleeKnightIdleState(controller, this));
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
        controller.MoveHorizontal(new Vector2(0, 1), 0, 10);
        MeleeAISubStateMachine cfsm = (MeleeAISubStateMachine)fsm;
        //Animator ani = cfsm.CharacterAni;
        //ani.SetFloat("vx", 0);
        //ani.SetFloat("vy", controller.GetHorizontal().magnitude / cfsm.maxSpeed);
        controller.MoveVertical(30, 0);
    }
    public override void DoBeforeEntering()
    {
        controller.EnableClamping();
        controller.EnableSlopeLimit();
    }
}


