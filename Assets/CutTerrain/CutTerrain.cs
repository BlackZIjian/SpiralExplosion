using UnityEngine;
using System.Collections.Generic;

public class CutTerrain : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public static void CutTerrainMesh(Transform terrain,Vector3 center,float distance,float radius)
    {
        VoroInTerrain voro = terrain.GetComponent<VoroInTerrain>();
        if(voro != null)
        {
            Mesh mesh = terrain.GetComponent<MeshFilter>().mesh;
            Vector3[] vertexes = mesh.vertices;
            for(int i=0;i<vertexes.Length;i++)
            {
                Vector3 pos = terrain.TransformPoint(vertexes[i]);
                float dx = pos.x - center.x;
                float dy = pos.z - center.z;
                float real_dis = dx * dx + dy * dy;
                float dis = distance * distance;
                
                if(real_dis < dis)
                {
                    float downslope = Mathf.Sqrt(dis - real_dis);
                    vertexes[i] = terrain.InverseTransformPoint(pos - new Vector3(0, downslope, 0));
                }
            }
            mesh.vertices = vertexes;
        }
    }
}
