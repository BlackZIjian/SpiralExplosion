using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using lvlv_voronoi;

public class Test : MonoBehaviour {
    VoroInTerrain voro;
    public Material mat;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
       
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);
            foreach(RaycastHit hit in hits)
            {
                Transform terrain = hit.transform;
                VoroInTerrain voro = terrain.GetComponent<VoroInTerrain>();
                if(voro != null)
                {
                    CutTerrain.CutTerrainMesh(terrain, hit.point, 10, 0.5f);
                }
            }
        }
	}
}
