using UnityEngine;
using System.Collections;

public class TestNotication : MonoBehaviour {

	// Use this for initialization
	void Start () {
        NotificationCenter.DefaultCenter().PostNotification(this, "testMessage");

    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
