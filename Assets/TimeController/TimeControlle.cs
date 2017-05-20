using UnityEngine;
using System.Collections;
using Chronos;

public class TimeControlle : BaseBehaviour{

    public static TimeControlle instance;
    public void Awake()
    {
        instance = this;
    }

	public void PauseTime(string key,float time)
    {
        Timekeeper.instance.Clock(key).localTimeScale = 0.13f;
        StartCoroutine(continuePause(time, key));
    }
    IEnumerator continuePause(float waitTime,string key)
    {
        yield return new WaitForSeconds(waitTime);
        Timekeeper.instance.Clock(key).localTimeScale = 1;
    }
}
