using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SetFoot))]
public class StaticObject : MonoBehaviour {
    bool hasAdded = false;
    Vector3 foot;
	// Use this for initialization
	void Start () {
        foot = transform.FindChild("__mfoot").position;
	}
	
	// Update is called once per frame
	void Update () {
        
	
	}

    
}
