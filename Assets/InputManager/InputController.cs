using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class InputController : BaseBehaviour {
    //public KeyCode Forward;
    //public KeyCode Back;
    //public KeyCode Left;
    //public KeyCode Right;
    //public KeyCode Jump;
    //public KeyCode Attack;
    public Dictionary<string, KeyCode> keySetting=new Dictionary<string,KeyCode>();
    private static Dictionary<string, object> keyValue = new Dictionary<string, object>();
    float attack_time = 0;
	// Use this for initialization
	void Start () {
        //Forward = KeyCode.W;
        //Back = KeyCode.S;
        //Left = KeyCode.A;
        //Right = KeyCode.D;
        //Jump = KeyCode.Space;
        //Attack = KeyCode.Mouse0;
        keySetting.Add("Forward", KeyCode.W);
        keySetting.Add("Back", KeyCode.S);
        keySetting.Add("Right", KeyCode.D);
        keySetting.Add("Left", KeyCode.A);
        keySetting.Add("Jump", KeyCode.Space);
        keySetting.Add("Attack", KeyCode.Mouse0);
        keyValue.Add("inputV", new Vector2(0, 0));
    }
	
	// Update is called once per frame
	void Update () {
     foreach(var key in keySetting)
        {
            if(Input.GetKeyDown(key.Value))
            {
                keyValue[key.Key] = true;
            }
            else
            {
                keyValue[key.Key] = false;
            }
        }
     

        Vector2 v2 = new Vector2(0, 0);
        if(Input.GetKey(keySetting["Forward"]))
        {
            v2.y = 1;
        }
        if (Input.GetKey(keySetting["Back"]))
        {
            v2.y = -1;
        }
        if (Input.GetKey(keySetting["Left"]))
        {
            v2.x = -1;
        }
        if (Input.GetKey(keySetting["Right"]))
        {
            v2.x = 1;
        }
        keyValue["inputV"] = v2;
        keyValue["Attack"] = false;
        if(Input.GetKey(keySetting["Attack"]))
        {
            attack_time += time.deltaTime;
        }
        else
        {
            if(attack_time < 0.2f && attack_time > 0)
            {
                keyValue["Attack"] = true;
            }
            attack_time = 0;
        }

        keyValue["PluckAttack"] = (attack_time > 0.2f);
        
        
    }

    public static T GetKey<T>(string keyName)
    {
        
        return (T)keyValue[keyName];
    }

    public KeyCode getKeySetting(string keyname)
    {
        foreach (var key in keySetting)
        {
            if (key.Key==keyname)
            {
                return key.Value;
            }
        }
        return KeyCode.None;
    }
}
