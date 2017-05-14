using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


public class ViewCast
{
    public static List<Transform> GetConeCast(float maxDis,float angle,Vector3 direction,Vector3 center,LayerMask layer)
    {
        List<Transform> results = new List<Transform>();
        Collider[] colliders = Physics.OverlapSphere(center, maxDis, layer);
        float cos = Mathf.Cos(angle / 2 / 180 * Mathf.PI);
       
        foreach (Collider c in colliders)
        {
            Vector3 target = c.gameObject.transform.position;
            if (Vector3.Dot(direction,target) / direction.magnitude / target.magnitude >= cos )
            {
                results.Add(c.gameObject.transform);
            }
        }
        return results;
    }
    public static bool isConeCast(float maxDis, float angle, Vector3 direction, Vector3 center, LayerMask layer)
    {
        Collider[] colliders = Physics.OverlapSphere(center, maxDis, layer);
        float cos = Mathf.Cos(angle / 2 / 180 * Mathf.PI);

        foreach (Collider c in colliders)
        {
            Vector3 target = c.gameObject.transform.position;
            if (Vector3.Dot(direction, target) / direction.magnitude / target.magnitude >= cos)
            {
                return true;
            }
        }
        return false;
    }
}
public struct AIInput
{
    public Vector2 AIInputV;
    public bool AIInputJump;
}

public class MeleeAISubStateMachine : FSMSystem
{
    public MeleeMoveSubStateMachine moveFSM;
    public SuperCharacterController MeleeController;
    public LayerMask canSee;
    public AIInput ai_input = new AIInput();
    public bool enableSuperMove = true;
    public float maxSeeDis = 10;
    public float angle = 30;
    public float jiejinDis = 3;
    public void Start()
    {
        moveFSM = gameObject.AddComponent<MeleeMoveSubStateMachine>();
        MeleeController.fsm = moveFSM;
        AddState(new MeleeKnightIdleState(MeleeController, this));
        
    }
    public void Update()
    {
        if(time.deltaTime == 0)
        {
            return;
        }
        CurrentState.Reason();
        CurrentState.Act();
        if(enableSuperMove)
        {
            moveFSM.CurrentState.Reason();
            moveFSM.CurrentState.Act();
        }
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
    MeleeAISubStateMachine aiFSM;
    float remainTime = 0;

    public MeleeKnightIdleState(SuperCharacterController c, FSMSystem f)
    {
        fsm = f;
        aiFSM = (MeleeAISubStateMachine)fsm;
        controller = c;
        stateID = StateID.MeleeKnightIdle;
        AddTransition(Transition.MeleeKnightSeeSomeOne, StateID.MeleeKnightSeeSomeOne);
    }
    public override void Reason()
    {
        if(ViewCast.isConeCast(aiFSM.maxSeeDis, aiFSM.angle, controller.lookDirection,controller.transform.position,aiFSM.canSee))
        {
            aiFSM.PerformTransition(Transition.MeleeKnightSeeSomeOne);
        }
    }

    public override void Act()
    {
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
        controller.enabled = true;
        aiFSM.enableSuperMove = true;
    }
}

public class MeleeKnightSeeSomeoneState : FSMState
{
    public SuperCharacterController controller;
    MeleeAISubStateMachine aiFSM;
    Vector3 lastPos;

    public MeleeKnightSeeSomeoneState(SuperCharacterController c, FSMSystem f)
    {
        fsm = f;
        aiFSM = (MeleeAISubStateMachine)fsm;
        controller = c;
        stateID = StateID.MeleeKnightSeeSomeOne;
    }
    public override void Reason()
    {
        List<Transform> canSees = ViewCast.GetConeCast(aiFSM.maxSeeDis, aiFSM.angle, controller.lookDirection, controller.transform.position, aiFSM.canSee);
        int maxLevel = -1;
        foreach(Transform trans in canSees)
        {
            float dis = Vector3.Distance(controller.transform.position, trans.position);
            int level = 2;//看不清
            if ( dis < aiFSM.maxSeeDis )
            {
                level = -1;//get看见物体的信息
            }
            if(level > maxLevel)
            {
                lastPos = trans.position;
            }
        }
        if(Vector3.Distance(controller.transform.position, lastPos) < aiFSM.jiejinDis)
        {
            //根据看见物体信息转移状态
        }
    }

    public override void Act()
    {
        //开启寻路,寻向lastPos

    }
    public override void DoBeforeEntering()
    {
        controller.enabled = true;
        aiFSM.enableSuperMove = true;
        lastPos = controller.transform.position;
    }
}


