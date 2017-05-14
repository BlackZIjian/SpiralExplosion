using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NavigationPath : MonoBehaviour {

    private NavMeshAgent agent;
    public Transform target;

    //使用栈，如果需要可插入寻路节点
    private Stack<Vector3> pathPoint = new Stack<Vector3>();

    //比较是否目标点是否更改
    private Vector3 finalDest;
    private Vector3 currentFinalDest;

    private Transform m_transform;
    // Use this for initialization
    void Start () {
        m_transform = this.transform;
        agent = GetComponent<NavMeshAgent>();
        
        agent.Stop();
        StartCoroutine(Move());
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator UpdateNewDest(Vector3 pos)
    {
        pathPoint.Clear();
        pathPoint.Push(pos);
        finalDest = pos;
        currentFinalDest = pos;
        yield return null;
    }

    IEnumerator Move()
    {
        //enable agent updates
        agent.Resume();
        agent.updateRotation = true;
        while (target == null)
            yield return null;

        currentFinalDest = target.position;
        finalDest = currentFinalDest;

        pathPoint.Push(currentFinalDest);
        agent.SetDestination(currentFinalDest);

        yield return StartCoroutine(WaitForDestination());
        StartCoroutine(NextWaypoint());
    }

    IEnumerator WaitForDestination()
    {
        yield return new WaitForEndOfFrame();
        while (agent.pathPending)
            yield return null;
        yield return new WaitForEndOfFrame();

        float remain = agent.remainingDistance;
        while (remain == Mathf.Infinity || remain - agent.stoppingDistance > float.Epsilon
        || agent.pathStatus != NavMeshPathStatus.PathComplete)
        {
            //超出stoppingDistance重新寻路
            if (Vector3.Distance(target.position,finalDest)>agent.stoppingDistance)
            {
                finalDest = target.position;
                break;
            }
            remain = agent.remainingDistance;
            yield return null;
        }
        pathPoint.Pop();
    }

    IEnumerator NextWaypoint()
    {
        if (currentFinalDest != finalDest)
        {
            yield return StartCoroutine(UpdateNewDest(finalDest));
        }
        if (pathPoint.Count == 0)
            yield return StartCoroutine(UpdateNewDest(target.position));
        agent.SetDestination(pathPoint.Peek());
        yield return StartCoroutine(WaitForDestination());

        StartCoroutine(NextWaypoint());
    }
}
