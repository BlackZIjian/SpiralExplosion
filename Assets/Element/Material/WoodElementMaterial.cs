using UnityEngine;
using System.Collections;

public class WoodElementMaterial : ElementMaterial {
	// Use this for initialization
	void Start () {
        AddState(new WoodNormal(this));
        AddState(new WoodBurn(this));
        AddState(new WoodWet(this));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

public class WoodBurn : ElementMaterialState
{
    public WoodBurn(ElementMaterial f)
    {
        fsm = f;
        stateName = "Burn";
        AddTransition(new StateTransition("Burn", "Normal"));
    }

    public override void Reason()
    {
        ElementMaterial f = (ElementMaterial)fsm;
        if(f.Temperature<f.FlamePoint)
        {
            f.PerformTransition(findTransition("Normal"));
        }
    }

    public override void Act()
    {
        ElementMaterial f = (ElementMaterial)fsm;
        f.ElementDurability--;
    }

    public override void DoBeforeEntering()
    {
       
    }

    public override void DoBeforeLeaving()
    {

    }
}

public class WoodWet : ElementMaterialState
{
    public WoodWet(ElementMaterial f)
    {
        fsm = f;
        stateName = "Wet";
        AddTransition(new StateTransition("Wet", "Normal"));
    }

    public override void Reason()
    {
        ElementMaterial f = (ElementMaterial)fsm;
        if (f.Wetness  <= 1f)
        {
            f.PerformTransition(findTransition("Normal"));
        }
    }

    public override void Act()
    {
        ElementMaterial f = (ElementMaterial)fsm;
        f.Wetness--;
    }

    public override void DoBeforeEntering()
    {

    }

    public override void DoBeforeLeaving()
    {

    }
}

public class WoodNormal : ElementMaterialState
{
    public WoodNormal(ElementMaterial f)
    {
        fsm = f;
        stateName = "Normal";
        AddTransition(new StateTransition("Normal", "Burn"));
        AddTransition(new StateTransition("Normal", "Wet"));
    }

    public override void Reason()
    {
        ElementMaterial f = (ElementMaterial)fsm;
        if (f.Temperature >= f.FlamePoint && f.Wetness-f.WetPoint<float.Epsilon)
        {
            f.PerformTransition(findTransition("Burn"));
        }
        if(f.Wetness - f.WetPoint>float.Epsilon)
        {
            f.PerformTransition(findTransition("Wet"));
        }
    }

    public override void Act()
    {
        ElementMaterial f = (ElementMaterial)fsm;
        if (f.Wetness>float.Epsilon)
            f.Wetness--;

    }

    public override void DoBeforeEntering()
    {

    }

    public override void DoBeforeLeaving()
    {

    }
}

