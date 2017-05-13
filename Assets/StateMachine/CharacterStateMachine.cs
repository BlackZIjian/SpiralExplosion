using UnityEngine;
using System.Collections;
using System;

public class CharacterStateMachine : FSMSystem {

    public Animator CharacterAni;
    public float maxSpeed;
    public SuperCharacterController controller;
    public void Start()
    {
        this.CharacterAni = controller.ani;
        this.maxSpeed = controller.maxSpeed;
        AddState(new CharacterIdleState(controller,this));
        AddState(new CharacterWalkState(controller, this));
        AddState(new CharacterJumpState(controller, this));
        AddState(new CharacterFallState(controller, this));
    }
}

public class CharacterIdleState : FSMState
{
    public SuperCharacterController controller;
    public CharacterIdleState(SuperCharacterController c,FSMSystem f)
    {
        fsm = f;
        controller = c;
        stateID = StateID.CharacterIdle;
        AddTransition(Transition.CharacterIdleToWalk, StateID.CharacterWalk);
        AddTransition(Transition.CharacterJump, StateID.CharacterJump);
        AddTransition(Transition.CharacterFall, StateID.CharacterFall);
    }
    public override void Reason()
    {
        
        if(InputController.GetKey<Vector2>("inputV").magnitude > 0.1f)
        {
            fsm.PerformTransition(Transition.CharacterIdleToWalk);
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            fsm.PerformTransition(Transition.CharacterJump);
        }
        if(!controller.MaintainingGround())
        {
            fsm.PerformTransition(Transition.CharacterFall);
        }
    }

    public override void Act()
    {
        controller.MoveHorizontal(new Vector2(0, 1), 0, 10);
        CharacterStateMachine cfsm= (CharacterStateMachine)fsm;
        Animator ani = cfsm.CharacterAni;
        ani.SetFloat("vx", 0);
        ani.SetFloat("vy", controller.GetHorizontal().magnitude / cfsm.maxSpeed);
        controller.MoveVertical(30, 0);
    }
    public override void DoBeforeEntering()
    {
        controller.EnableClamping();
        controller.EnableSlopeLimit();
    }
}

public class CharacterWalkState : FSMState
{
    public SuperCharacterController controller;
    public CharacterWalkState(SuperCharacterController c, FSMSystem f)
    {
        fsm = f;
        controller = c;
        stateID = StateID.CharacterWalk;
        AddTransition(Transition.CharacterWalkToIdle, StateID.CharacterIdle);
        AddTransition(Transition.CharacterJump, StateID.CharacterJump);
        AddTransition(Transition.CharacterFall, StateID.CharacterFall);
    }

    public override void Reason()
    {
        if (!controller.MaintainingGround())
        {
            fsm.PerformTransition(Transition.CharacterFall);
        }
        if (InputController.GetKey<bool>("Jump"))
        {
            fsm.PerformTransition(Transition.CharacterJump);
        }
        if (InputController.GetKey<Vector2>("inputV").magnitude <= 0.1f)
        {
            fsm.PerformTransition(Transition.CharacterWalkToIdle);
        }
    }

    public override void Act()
    {
        CharacterStateMachine cfsm = (CharacterStateMachine)fsm;
        Animator ani = cfsm.CharacterAni;
        float walkSpeed = cfsm.maxSpeed;
        float walkAcc = 5;
        float angleDelta = 30;
        Vector2 inputV = InputController.GetKey<Vector2>("inputV");
        Transform camera = Camera.main.transform;//模拟照相机
        Vector3 screenForword = controller.transform.position - camera.position;
        Vector3 pScreenForword = Math3d.ProjectVectorOnPlane(controller.up, screenForword);
        Quaternion inputQua = Quaternion.FromToRotation(new Vector3(0,0,1), new Vector3(inputV.x, 0, inputV.y));
        if(inputV == new Vector2(0,-1))
        {
            inputQua = Quaternion.AngleAxis(180, controller.up);
        }
        Vector3 target = inputQua * pScreenForword;
        Quaternion finalRoante = Quaternion.FromToRotation(Vector3.forward, target);
        controller.Ronate(finalRoante, angleDelta);
        Vector2 direction = new Vector2(0, 1);
        controller.MoveHorizontal(direction, walkSpeed, walkAcc);

      
        float finalAngle;
        Vector3 tv;
        float vy = controller.GetHorizontal().magnitude / cfsm.maxSpeed;
        float vx = vy;
        finalRoante.ToAngleAxis(out finalAngle, out tv);
        if(tv.y < 0)
        {
            vx = -vx;
        }
        if (finalAngle <= 5)
            vx = 0;
        ani.SetFloat("vx", vx);
        ani.SetFloat("vy", vy);
    }
    public override void DoBeforeEntering()
    {
        controller.EnableClamping();
        controller.EnableSlopeLimit();
    }
}

public class CharacterJumpState : FSMState
{
    public SuperCharacterController controller;
    public CharacterJumpState(SuperCharacterController c, FSMSystem f)
    {
        stateID = StateID.CharacterJump;
        fsm = f;
        controller = c;
        AddTransition(Transition.CharacterFall, StateID.CharacterFall);
    }
    public override void Reason()
    {
        float jumpHeight = 0;
        float gravity = 0;
        if (controller.GetVertical() >= Mathf.Sqrt(2 * gravity * jumpHeight) || !controller.MaintainingGround())
        {
            fsm.PerformTransition(Transition.CharacterFall);
        }
    }

    public override void Act()
    {
        float jumpForce = 3000;
        float jumpHeight = 5;
        float gravity = 9.8f;
        controller.MoveVertical(jumpForce, Mathf.Sqrt(2 * gravity * jumpHeight));
    }
    public override void DoBeforeEntering()
    {
        controller.DisableClamping();
        controller.DisableSlopeLimit();
    }
}

public class CharacterFallState : FSMState
{
    public SuperCharacterController controller;
    public CharacterFallState(SuperCharacterController c, FSMSystem f)
    {
        stateID = StateID.CharacterFall;
        fsm = f;
        controller = c;
        AddTransition(Transition.CharacterLand, StateID.CharacterIdle);
    }
    public override void Reason()
    {
        if (Vector3.Angle(controller.GetVertical() * controller.up, controller.up) > 90 && controller.AcquiringGround())
        {
            fsm.PerformTransition(Transition.CharacterLand);
        }
    }

    public override void Act()
    {
        float gravity = 9.8f;
        float maxDownSpeed = 10;
        controller.MoveVertical(gravity, -maxDownSpeed);
    }
    public override void DoBeforeEntering()
    {
        controller.DisableClamping();
        controller.DisableSlopeLimit();
    }
}

