using UnityEngine;
using System.Collections;

public class TestReceiver : MonoBehaviour {

	// Use this for initialization
	void Start () {
        NotificationCenter.DefaultCenter().AddObserver(this, "testMessage");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void testMessage(Notification noti)
    {
        Debug.Log(noti.sender);
    }
}
