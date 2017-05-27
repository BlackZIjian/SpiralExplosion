using UnityEngine;
using System.Collections;

public class ElementProperty : MonoBehaviour {

    public enum Elements { None = 0, Fire, Water, Electric, Wind, Soil };

    public Elements currentProperty;
    // Use this for initialization
    void Start()
    {
        currentProperty = Elements.None;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Power()
    {
        switch(currentProperty)
        {
            case Elements.Fire:break;
            case Elements.Water:break;
            case Elements.Electric:break;
            case Elements.Wind:break;
            case Elements.None:break;
            default:currentProperty = Elements.None;break;
        }
    }

    void Elec
}
