using UnityEngine;
using System.Collections;

public class ElementMaterial : ElementMaterialFSM {
    public bool Conductibility;//导电性
    public float FlamePoint;//燃点
    public float WetPoint;

    public float Wetness;//湿度
    public float Temperature;//温度
    public int ElementDurability;//耐久度

    public virtual void InitMaterialState() { }

    // Use this for initialization
    void Start () {
        InitMaterialState();
	}
	
	// Update is called once per frame
	void Update () {
	    
	}
}

