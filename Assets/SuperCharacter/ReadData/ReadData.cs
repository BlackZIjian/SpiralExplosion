using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using lvlv_voronoi;
using System.Text;
using System;

public class ReadData : MonoBehaviour {
    public GameObject terrain;
    public Material mat;
    public Color[] colors;
	// Use this for initialization
	void Start () {
        //InitPerlinMesh(ReadHeightMap(5000), 0, terrain); ;
        InitPolyMesh(ReadPolyData(30), 30,terrain, 100);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
   
    public float[,] ReadHeightMap(int index)
    {
        float[,] m = new float[50, 40];
        FileStream fs = new FileStream("e:\\TerrainData\\PerlinData\\perlinData"+index, FileMode.Open, FileAccess.Read, FileShare.Read);
        BinaryFormatter b = new BinaryFormatter();
        float[,] f = b.Deserialize(fs) as float[,];
        fs.Close();
        return f;
    }

    public void InitPerlinMesh(float[,] heightMap,float baseHeight,GameObject gameObject)
    {
        List<Vector3> vertexList = new List<Vector3>();
        for(int i=0;i<49;i++)
        {
            for(int j=0;j<39;j++)
            {
                vertexList.Add(new Vector3(i*10, (heightMap[i, j] + baseHeight) * 150, j*10));
                vertexList.Add(new Vector3(i*10, (heightMap[i, j+1] + baseHeight) * 150, (j+1)*10));
                vertexList.Add(new Vector3((i+1)*10, (heightMap[i+1, j] + baseHeight) * 150, j*10));
                vertexList.Add(new Vector3(i*10, (heightMap[i, j+1] + baseHeight) * 150, (j+1)*10));
                vertexList.Add(new Vector3((i+1)*10, (heightMap[i+1, j+1] + baseHeight) * 150, (j+1)*10));
                vertexList.Add(new Vector3((i+1)*10, (heightMap[i+1, j] + baseHeight) * 150, j*10));
            }
        }
        Vector3[] vertexs = new Vector3[vertexList.Count];
        int[] triangles = new int[vertexList.Count];
        for(int i=0;i<vertexList.Count;i++)
        {
            vertexs[i] = vertexList[i];
            triangles[i] = i;
        }

        Mesh mesh = gameObject.AddComponent<MeshFilter>().mesh;


        gameObject.AddComponent<MeshRenderer>().material = mat;

        mesh.Clear();//更新  
        mesh.vertices = vertexs;
       
        mesh.triangles = triangles;
        
      
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        gameObject.transform.localScale *= 10;
    }

    public voronoi ReadPolyData(int index)
    {
        voronoi voro = new voronoi();
        voro.ReadData("e:\\TerrainData\\", index);
        return voro;
    }

    public void InitPolyMesh(voronoi voro, int index,GameObject gameObject, float maxHeight)
    {
        if (gameObject.GetComponent<MeshFilter>() != null)
        {
            return;
        }
        List<int> triangles = new List<int>();
        List<Vector3> vertexes = new List<Vector3>();
        Dictionary<Vector2, float> biomeValue = new Dictionary<Vector2, float>();
        Dictionary<Vector2, int> biomeDis = new Dictionary<Vector2, int>();
        Dictionary<int, int> vertexIndexListByCenter = new Dictionary<int, int>();
     Dictionary<int, int> vertexIndexListByCorner = new Dictionary<int, int>();
    Dictionary<Vector3, int> CenterIndexList = new Dictionary<Vector3, int>();
     Dictionary<Vector3, int> CornerIndexList = new Dictionary<Vector3, int>();
    Texture2D biomeTexture = new Texture2D(500, 400);
        Color[,] textureColor = new Color[600, 500];
        Queue<KeyValuePair<Vector2, float>> queue = new Queue<KeyValuePair<Vector2, float>>();

        Dictionary<int, int> biomeDic = new Dictionary<int, int>();

        List<int>[] trianglesList = new List<int>[13];
        Mesh mesh = gameObject.AddComponent<MeshFilter>().mesh;


        gameObject.AddComponent<MeshRenderer>().material = mat;


        //gameObject.GetComponent<Renderer>().materials = biomeMat;
        //for (int i = 0; i < 13; i++)
        //{
        //    gameObject.GetComponent<Renderer>().materials[i].color = biomeColor[i];
        //}

        //}
        if (voro.isReadByFile)
        {
            foreach (KeyValuePair<int, CenterFile> cf in voro.CenterFileList)
            {


                CenterFile c = cf.Value;

                if (!c.water)
                {
                    foreach (int k in c.borders)
                    {

                        EdgeFile ef = voro.EdgeFileList[k];
                       
                        vertexes.Add(new Vector3((float)c.x, c.elevation * maxHeight, (float)c.y));
                        CenterIndexList[gameObject.transform.TransformPoint(new Vector3((float)c.x, c.elevation * maxHeight, (float)c.y))] = c.index;
                        triangles.Add(vertexes.Count - 1);
                        vertexIndexListByCenter[c.index] = triangles.Count - 1;
                        biomeDic.Add(triangles.Count - 1, (int)c.biome);
                        double x1, x2, y1, y2;
                        x1 = ef.ax - c.x;
                        x2 = ef.bx - c.x;
                        y1 = ef.ay - c.y;
                        y2 = ef.by - c.y;
                        float j = (float)(x1 * y2 - x2 * y1);
                        if (j < 0)
                        {
                            vertexes.Add(new Vector3((float)ef.ax, voro.CornerFileList[ef.cora].elevation * maxHeight, (float)ef.ay));
                            CornerIndexList[gameObject.transform.TransformPoint(new Vector3((float)ef.ax, voro.CornerFileList[ef.cora].elevation * maxHeight, (float)ef.ay))] = ef.cora;
                            triangles.Add(vertexes.Count - 1);
                            vertexIndexListByCorner[ef.cora] = vertexes.Count - 1;
                            biomeDic.Add(triangles.Count - 1, (int)c.biome);

                            vertexes.Add(new Vector3((float)ef.bx, voro.CornerFileList[ef.corb].elevation * maxHeight, (float)ef.by));
                            CornerIndexList[gameObject.transform.TransformPoint(new Vector3((float)ef.bx, voro.CornerFileList[ef.corb].elevation * maxHeight, (float)ef.by))] = ef.corb;
                            vertexIndexListByCorner[ef.corb] = vertexes.Count - 1;
                            triangles.Add(vertexes.Count - 1);
                            biomeDic.Add(triangles.Count - 1, (int)c.biome);
                        }
                        else
                        {
                            vertexes.Add(new Vector3((float)ef.bx, voro.CornerFileList[ef.corb].elevation * maxHeight, (float)ef.by));
                            CornerIndexList[gameObject.transform.TransformPoint(new Vector3((float)ef.bx, voro.CornerFileList[ef.corb].elevation * maxHeight, (float)ef.by))] = ef.corb;
                            vertexIndexListByCorner[ef.corb] = vertexes.Count - 1;
                            triangles.Add(vertexes.Count - 1);
                            biomeDic.Add(triangles.Count - 1, (int)c.biome);

                            vertexes.Add(new Vector3((float)ef.ax, voro.CornerFileList[ef.cora].elevation * maxHeight, (float)ef.ay));
                            CornerIndexList[gameObject.transform.TransformPoint(new Vector3((float)ef.ax, voro.CornerFileList[ef.cora].elevation * maxHeight, (float)ef.ay))] = ef.cora;
                            vertexIndexListByCorner[ef.cora] = vertexes.Count - 1;
                            triangles.Add(vertexes.Count - 1);
                            biomeDic.Add(triangles.Count - 1, (int)c.biome);
                        }
                    }
                }
            }
        }

        else
        {
            for (int i = 0; i < voro.voronoiCenterList.Count; i++)
            {
                Center c = voro.voronoiCenterList[i];

                if (!c.water)
                {
                    foreach (KeyValuePair<Edge, Edge> k in c.borders)
                    {

                        vertexes.Add(new Vector3((float)c.point.x, c.elevation * maxHeight, (float)c.point.y));
                        triangles.Add(vertexes.Count - 1);
                        biomeDic.Add(triangles.Count - 1, (int)c.biome);
                        double x1, x2, y1, y2;
                        x1 = k.Value.a.x - c.point.x;
                        x2 = k.Value.b.x - c.point.x;
                        y1 = k.Value.a.y - c.point.y;
                        y2 = k.Value.b.y - c.point.y;
                        float j = (float)(x1 * y2 - x2 * y1);
                        if (j < 0)
                        {
                            vertexes.Add(new Vector3((float)k.Value.a.x, k.Value.cora.elevation * maxHeight, (float)k.Value.a.y));
                            triangles.Add(vertexes.Count - 1);
                            biomeDic.Add(triangles.Count - 1, (int)c.biome);

                            vertexes.Add(new Vector3((float)k.Value.b.x, k.Value.corb.elevation * maxHeight, (float)k.Value.b.y));
                            triangles.Add(vertexes.Count - 1);
                            biomeDic.Add(triangles.Count - 1, (int)c.biome);
                        }
                        else
                        {
                            vertexes.Add(new Vector3((float)k.Value.b.x, k.Value.corb.elevation * maxHeight, (float)k.Value.b.y));
                            triangles.Add(vertexes.Count - 1);
                            biomeDic.Add(triangles.Count - 1, (int)c.biome);

                            vertexes.Add(new Vector3((float)k.Value.a.x, k.Value.cora.elevation * maxHeight, (float)k.Value.a.y));
                            triangles.Add(vertexes.Count - 1);
                            biomeDic.Add(triangles.Count - 1, (int)c.biome);
                        }
                    }
                }
            }
        }
        Vector3[] vertexesArray = new Vector3[vertexes.Count];
        int[] trianglesArray = new int[triangles.Count];
        Vector2[] uvArray = new Vector2[vertexes.Count];
        for (int i = 0; i < 13; i++)
        {
            trianglesList[i] = new List<int>();
        }
        for (int i = 0; i < vertexes.Count; i++)
        {
            vertexesArray[i] = vertexes[i];
            uvArray[i] = new Vector2(vertexes[i].x / 500, vertexes[i].y / 400);
        }


        for (int i = 0; i < triangles.Count; i++)
        {
            trianglesArray[i] = triangles[i];
            trianglesList[biomeDic[i]].Add(triangles[i]);
        }



        /*设置mesh*/
        mesh.Clear();//更新  
        mesh.vertices = vertexesArray;
        mesh.uv = uvArray;
        mesh.triangles = trianglesArray;
        mesh.subMeshCount = 13;
        gameObject.GetComponent<Renderer>().materials = new Material[13];
        for (int i = 0; i < 13; i++)
        {
            gameObject.GetComponent<Renderer>().materials[i] = mat;
            gameObject.GetComponent<Renderer>().materials[i].color = colors[i];
        }
        for (int i = 0; i < 13; i++)
        {
            mesh.SetTriangles(trianglesList[i], i);
        }

        for (int i = 0; i < 13; i++)
        {
            trianglesList[i] = new List<int>();
        }

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
       
        MeshCollider mc = gameObject.AddComponent<MeshCollider>();
        mc.sharedMesh = mesh;
        gameObject.AddComponent<BSPTree>();

        terrain.AddComponent<VoroInTerrain>().SetCenterInfo(index,vertexIndexListByCenter,vertexIndexListByCorner,voro.CenterFileList,voro.CornerFileList,voro.EdgeFileList,CenterIndexList,CornerIndexList);
        return;
    }
}
