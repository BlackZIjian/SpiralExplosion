using UnityEngine;
using System.Collections;
using lvlv_voronoi;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class VoroInTerrain : MonoBehaviour {
    private Dictionary<int, CenterFile> CenterFileList = new Dictionary<int, CenterFile>();
    private Dictionary<int, CornerFile> CornerFileList = new Dictionary<int, CornerFile>();
    private Dictionary<int, EdgeFile> EdgeFileList = new Dictionary<int, EdgeFile>();
    private Dictionary<Vector3, int> CenterIndexList = new Dictionary<Vector3, int>();
    private Dictionary<Vector3, int> CornerIndexList = new Dictionary<Vector3, int>();
    private Dictionary<int, int> vertexIndexListByCenter = new Dictionary<int, int>();
    private Dictionary<int, int> vertexIndexListByCorner = new Dictionary<int, int>();
    private int terrainIndex;
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void SetCenterInfo(int index, Dictionary<int, int> vertexIndexListByCenter, Dictionary<int, int> vertexIndexListByCorner,Dictionary<int, CenterFile> centers,Dictionary<int,CornerFile> corners,Dictionary<int,EdgeFile> edges, Dictionary<Vector3, int> CenterIndexList, Dictionary<Vector3, int> CornerIndexList)
    {
        CenterFileList = centers;
        CornerFileList = corners;
        EdgeFileList = edges;
        this.CenterIndexList = CenterIndexList;
        this.CornerIndexList = CornerIndexList;
        this.vertexIndexListByCenter = vertexIndexListByCenter;
        this.vertexIndexListByCorner = vertexIndexListByCorner;
        terrainIndex = index;
    }

    public CenterFile GetCenter(int index)
    {
        
            return CenterFileList[index];
        
    }

    public CornerFile GetCorner(int index)
    {

        return CornerFileList[index];

    }

    public int GetCenterVertexIndex(int centerIndex)
    {
        return vertexIndexListByCenter[centerIndex];
    }

    public int GetCornerVertexIndex(int cornerIndex)
    {
        return vertexIndexListByCorner[cornerIndex];
    }

    public void SaveCenterFile(string path)
    {
        CenterFile[] centers = new CenterFile[CenterFileList.Count];
        int i = 0;
        foreach (KeyValuePair<int, CenterFile> k in CenterFileList)
        {
            centers[i] = k.Value;
            i++;
        }

        FileStream fs = new FileStream(path + "terrain" + terrainIndex + "centerData", FileMode.Create);
        BinaryFormatter b = new BinaryFormatter();
        b.Serialize(fs, centers);
        fs.Close();

    }


    public void SaveCornerFile(string path)
    {
        CornerFile[] corners = new CornerFile[CornerFileList.Count];
        int i = 0;
        foreach (KeyValuePair<int, CornerFile> k in CornerFileList)
        {
            corners[i] = k.Value;
            i++;
        }

        FileStream fs = new FileStream(path + "terrain" + terrainIndex + "cornerData", FileMode.Create);
        BinaryFormatter b = new BinaryFormatter();
        b.Serialize(fs, corners);
        fs.Close();

    }



    /// <summary>  
    /// 获取二维圆内所有Center点 
    /// </summary>  
    public List<int> GetCenterIndex(Vector3 center,float distance)
    {
        List<int> result = new List<int>();
        foreach(KeyValuePair<Vector3,int> k in CenterIndexList)
        {
            Vector2 v1 = new Vector2(center.x,center.z);
            Vector2 v2 = new Vector2(k.Key.x, k.Key.z);
            if((v1.x-v2.x)*(v1.x-v2.x) + (v1.y - v2.y)*(v1.y-v2.y) <= distance*distance)
            {
                result.Add(k.Value);
            }
        }
        return result;
    }

    /// <summary>  
    /// 获取二维圆内所有Corner点 
    /// </summary>  
   public  List<int> GetCornerIndex(Vector3 center, float distance)
    {
        List<int> result = new List<int>();
        foreach (KeyValuePair<Vector3, int> k in CornerIndexList)
        {
            Vector2 v1 = new Vector2(center.x, center.z);
            Vector2 v2 = new Vector2(k.Key.x, k.Key.z);
            if ((v1.x - v2.x) * (v1.x - v2.x) + (v1.y - v2.y) * (v1.y - v2.y) <= distance * distance)
            {
                result.Add(k.Value);
            }
        }
        return result;
    }

}
