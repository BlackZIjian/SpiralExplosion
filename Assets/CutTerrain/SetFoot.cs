using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SetFoot : MonoBehaviour {
    Transform foot;
    // Use this for initialization
    void Start()
    {
        GetFoot();
    }

    void GetFoot()
    {
        foot = transform.FindChild("__mfoot");
        if (foot == null)
        {
            foot = new GameObject().transform;
            foot.name = "__mfoot";
            foot.parent = transform;
            foot.transform.position = transform.position;
        }
    }
}
