using UnityEngine;
using System.Collections;
using System;
using Chronos;

public class CharacterStateMachine : FSMSystem {

    public Animator CharacterAni;
    public float maxSpeed;
    public float attackSpeed;
    public SuperCharacterController controller;
    public bool isContinueAttack = false;
    public float plucking = 0;
    public float pluckSpeed = 0;
    public Collider WeaponCollider;
    public bool isTriggerSome = false;
    public void OnTriggerSome(TriggerEventArgs args)
    {
        WeaponSuperControllerEventArgs wsceargs = args as WeaponSuperControllerEventArgs;
        if (wsceargs.isEnter)
            isTriggerSome = true;
        else
            isTriggerSome = false;
    }
    public void Start()
    {
        this.CharacterAni = controller.ani;
        this.maxSpeed = controller.maxSpeed;
        attackSpeed = controller.attackSpeed;
        AddState(new CharacterIdleState(controller,this));
        AddState(new CharacterWalkState(controller, this));
        AddState(new CharacterJumpState(controller, this));
        AddState(new CharacterFallState(controller, this));
        AddState(new CharacterAttack1State(controller, this));
        AddState(new CharacterAttack2State(controller, this));
        AddState(new CharacterAttack3State(controller, this));
        AddState(new CharacterPluckState(controller, this));
        AddState(new CharacterWaveSwordState(controller, this));
        TriggerManagement.AddObserver("character", new OnTrigger(OnTriggerSome));
    }

    public void OnAttackFinished()
    {
        if(isContinueAttack)
        {
            PerformTransition(Transition.ToNextAttack);
        }
        else
        {
            PerformTransition(Transition.ToCharacterIdle);
        }
    }

    public void OnWaveFinished()
    {
        PerformTransition(Transition.PullToIdle);
    }
    public void pausePull()
    {
        
        AnimatorStateInfo animatorInfo = CharacterAni.GetCurrentAnimatorStateInfo(0);
        if (animatorInfo.IsName("PullSword") && CurrentStateID == StateID.CharacterPluck)//注意这里指的不是动画的名字而是动画状态的名字
        {
            CharacterAni.speed = 0;
        }
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
        AddTransition(Transition.ToNextAttack, StateID.CharacterAttack1);
        AddTransition(Transition.ToPlucking, StateID.CharacterPluck);
    }
    public override void Reason()
    {
        if(InputController.GetKey<Vector2>("inputV").magnitude > 0.1f)
        {
            fsm.PerformTransition(Transition.CharacterIdleToWalk);
        }
        if(InputController.GetKey<bool>("Jump"))
        {
            fsm.PerformTransition(Transition.CharacterJump);
        }
        if(InputController.GetKey<bool>("Attack"))
        {
            fsm.PerformTransition(Transition.ToNextAttack);
        }
        if(InputController.GetKey<bool>("PluckAttack"))
        {
            fsm.PerformTransition(Transition.ToPlucking);
        }
        if (!controller.MaintainingGround())
        {
            fsm.PerformTransition(Transition.CharacterFall);
        }

    }

    public override void Act()
    {
        controller.MoveHorizontal(new Vector2(0, 1), 0, 5);
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
        CharacterStateMachine cfsm = (CharacterStateMachine)fsm;
        Animator ani = cfsm.CharacterAni;
        ani.SetBool("isAttackFinished", true);
        ani.SetBool("startAttack", false);
    }
    public override void DoBeforeLeaving()
    {
        CharacterStateMachine cfsm = (CharacterStateMachine)fsm;
        Animator ani = cfsm.CharacterAni;
        ani.SetBool("isAttackFinished", false);
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
        AddTransition(Transition.ToNextAttack, StateID.CharacterAttack1);
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
        if (InputController.GetKey<bool>("Attack"))
        {
            fsm.PerformTransition(Transition.ToNextAttack);
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
        ani.SetFloat("vx", 0);
        ani.SetFloat("vy", Mathf.Abs(vy));
    }
    public override void DoBeforeEntering()
    {
        controller.EnableClamping();
        controller.EnableSlopeLimit();
        CharacterStateMachine cfsm = (CharacterStateMachine)fsm;
        Animator ani = cfsm.CharacterAni;
        ani.SetBool("isAttackFinished", true);
        ani.SetBool("startAttack", false);
    }
    public override void DoBeforeLeaving()
    {
        CharacterStateMachine cfsm = (CharacterStateMachine)fsm;
        Animator ani = cfsm.CharacterAni;
        ani.SetBool("isAttackFinished", false);
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

public class CharacterAttack1State : FSMState
{
    public SuperCharacterController controller;
    public CharacterStateMachine cfsm;
    bool isAttackSuccess = false;
    float shakeTime =0;
    public CharacterAttack1State(SuperCharacterController c, FSMSystem f)
    {
        stateID = StateID.CharacterAttack1;
        fsm = f;
        controller = c;
        AddTransition(Transition.ToCharacterIdle, StateID.CharacterIdle);
        AddTransition(Transition.ToNextAttack, StateID.CharacterAttack2);
        cfsm = (CharacterStateMachine)fsm;
    }
    public override void Reason()
    {
        if(InputController.GetKey<bool>("Attack"))
        {
            cfsm.isContinueAttack = true;
        }
        
    }

    public override void Act()
    {
        if(controller.GetHorizontal().magnitude > cfsm.attackSpeed)
        {
            controller.MoveHorizontal(controller.GetHorizontal(), cfsm.attackSpeed, 5);
        }
        if(!isAttackSuccess && cfsm.isTriggerSome)
        {
            isAttackSuccess = true;
            //武器耐久减少之类的
        }
        if(cfsm.isTriggerSome)
        {
            shakeTime -= Timekeeper.instance.Clock("Controller").deltaTime;
            if(shakeTime <= 0)
            {
                shakeTime = 1f;
                TimeControlle.instance.PauseTime("Root", 0.3f);
            }
        }
    }
    public override void DoBeforeEntering()
    {
        cfsm.isContinueAttack = false;
        cfsm.CharacterAni.SetBool("startAttack", true);
        isAttackSuccess = false;
        shakeTime = 0;
    }
    public override void DoBeforeLeaving()
    {
    }
    
}

public class CharacterAttack2State : FSMState
{
    public SuperCharacterController controller;
    public CharacterStateMachine cfsm;
    bool isAttackSuccess = false;
    float shakeTime = 0;
    public CharacterAttack2State(SuperCharacterController c, FSMSystem f)
    {
        stateID = StateID.CharacterAttack2;
        fsm = f;
        controller = c;
        AddTransition(Transition.ToCharacterIdle, StateID.CharacterIdle);
        AddTransition(Transition.ToNextAttack, StateID.CharacterAttack3);
        cfsm = (CharacterStateMachine)fsm;
    }
    public override void Reason()
    {
        if (InputController.GetKey<bool>("Attack"))
        {
            cfsm.isContinueAttack = true;
        }
    }

    public override void Act()
    {
        if (controller.GetHorizontal().magnitude > cfsm.attackSpeed)
        {
            controller.MoveHorizontal(controller.GetHorizontal(), cfsm.attackSpeed, 5);
        }
        if (!isAttackSuccess && cfsm.isTriggerSome)
        {
            isAttackSuccess = true;
            //武器耐久减少之类的
        }
        if (cfsm.isTriggerSome)
        {
            shakeTime -= Timekeeper.instance.Clock("Controller").deltaTime;
            if (shakeTime <= 0)
            {
                shakeTime = 1f;
                TimeControlle.instance.PauseTime("Root", 0.3f);
            }
        }
    }
    public override void DoBeforeEntering()
    {
        cfsm.isContinueAttack = false;
        isAttackSuccess = false;
        shakeTime = 0;
    }
    public override void DoBeforeLeaving()
    {
    }
}

public class CharacterAttack3State : FSMState
{
    public SuperCharacterController controller;
    public CharacterStateMachine cfsm;
    bool isAttackSuccess = false;
    float shakeTime = 0;
    public CharacterAttack3State(SuperCharacterController c, FSMSystem f)
    {
        stateID = StateID.CharacterAttack3;
        fsm = f;
        controller = c;
        AddTransition(Transition.ToCharacterIdle, StateID.CharacterIdle);
        AddTransition(Transition.ToNextAttack, StateID.CharacterIdle);
        cfsm = (CharacterStateMachine)fsm;
    }
    public override void Reason()
    {
        if (InputController.GetKey<bool>("Attack"))
        {
            cfsm.isContinueAttack = true;
        }
    }

    public override void Act()
    {
        if (controller.GetHorizontal().magnitude > cfsm.attackSpeed)
        {
            controller.MoveHorizontal(controller.GetHorizontal(), cfsm.attackSpeed, 5);
        }
        if (!isAttackSuccess && cfsm.isTriggerSome)
        {
            isAttackSuccess = true;
            //武器耐久减少之类的
        }
        if (cfsm.isTriggerSome)
        {
            shakeTime -= Timekeeper.instance.Clock("Controller").deltaTime;
            if (shakeTime <= 0)
            {
                shakeTime = 1f;
                TimeControlle.instance.PauseTime("Root", 0.3f);
            }
        }
    }
    public override void DoBeforeEntering()
    {
        cfsm.isContinueAttack = false;
        isAttackSuccess = false;
        shakeTime = 0; 
    }
    public override void DoBeforeLeaving()
    {
    }
}

public class CharacterPluckState : FSMState
{
    public SuperCharacterController controller;
    public CharacterStateMachine cfsm;
    public CharacterPluckState(SuperCharacterController c, FSMSystem f)
    {
        stateID = StateID.CharacterPluck;
        fsm = f;
        controller = c;
        cfsm = (CharacterStateMachine)fsm;
        AddTransition(Transition.ToPulling, StateID.CharacterWaveSword);
    }
    public override void Reason()
    {
        //转移到挥剑状态
        if (!InputController.GetKey<bool>("PluckAttack"))
        {
            cfsm.PerformTransition(Transition.ToPulling);
        }
    }

    public override void Act()
    {
        cfsm.plucking += controller.time.deltaTime * cfsm.pluckSpeed;
    }
    public override void DoBeforeEntering()
    {
        cfsm.plucking = 0;
        cfsm.CharacterAni.SetBool("startPluck", true);
    }
    public override void DoBeforeLeaving()
    {
        cfsm.CharacterAni.SetBool("startPluck", false);
    }
}

public class CharacterWaveSwordState : FSMState
{
    public SuperCharacterController controller;
    public CharacterStateMachine cfsm;
    public CharacterWaveSwordState(SuperCharacterController c, FSMSystem f)
    {
        stateID = StateID.CharacterWaveSword;
        fsm = f;
        controller = c;
        cfsm = (CharacterStateMachine)fsm;
        AddTransition(Transition.PullToIdle, StateID.CharacterIdle);
    }
    public override void Reason()
    {

    }

    public override void Act()
    {
        //进行碰撞检测，如果碰到敌人的武器，则时间减缓，破碎
    }
    public override void DoBeforeEntering()
    {
        AnimatorStateInfo animatorInfo = cfsm.CharacterAni.GetCurrentAnimatorStateInfo(0);
        if (animatorInfo.IsName("PullSword") || animatorInfo.IsName("TakeSword"))//注意这里指的不是动画的名字而是动画状态的名字
        {
            cfsm.CharacterAni.speed = 1;
        }
    }
    public override void DoBeforeLeaving()
    {
        cfsm.plucking = 0;
    }
}



