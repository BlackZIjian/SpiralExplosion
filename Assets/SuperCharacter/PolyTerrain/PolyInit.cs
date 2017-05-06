using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


namespace lvlv_voronoi
{
    public class PerlinNoiseGetArgs : EventArgs
    {
        public float x;
        public float y;
        public PerlinNoiseGetArgs(float x,float y)
        {
            this.x = x;
            this.y = y;
        }
    }
    [Serializable]
    public class voronoi
    {
        
        public BiomeType[,] BioDiagram = new BiomeType[4, 6]
        {
            {BiomeType.SUBTROPICAL_DESERT,BiomeType.GRASSLAND,BiomeType.TROPICAL_MONSOON,BiomeType.TROPICAL_MONSOON,BiomeType.RAINFOREST,BiomeType.RAINFOREST },
            {BiomeType.TEMPERATE_DESERT,BiomeType.GRASSLAND,BiomeType.GRASSLAND,BiomeType.TEMPERATE_DECIDUOUS,BiomeType.TEMPERATE_DECIDUOUS,BiomeType.TEMPERATE_RAINFOREST },
            {BiomeType.TEMPERATE_DESERT,BiomeType.TEMPERATE_DESERT,BiomeType.SHRUB,BiomeType.SHRUB,BiomeType.CONIFEROUS,BiomeType.CONIFEROUS },
            {BiomeType.SCORCH,BiomeType.ROCK,BiomeType.TUNDRA,BiomeType.SNOW,BiomeType.SNOW,BiomeType.SNOW }
        };
        System.Random seeder;
        public float nowPerlinNoise = 0;
        public bool initComplete = false;
        public bool isReadByFile = false;
        public string ErrorMessage = null;
        
        public List<Center> voronoiCenterList = new List<Center>();//voroni图所有的中心点
        public Dictionary<Site, Corner> voronoiCornerList = new Dictionary<lvlv_voronoi.Site, Corner>();//voroni图所有的拐角点
        public List<Edge> voronoiEdgeList = new List<Edge>();//vironoi图所有边

        public Dictionary<int,CenterFile> CenterFileList = new Dictionary<int, CenterFile>();
        public Dictionary<int,CornerFile> CornerFileList = new Dictionary<int, CornerFile>();
        public Dictionary<int,EdgeFile> EdgeFileList = new Dictionary<int, EdgeFile>();
        Voronoi voroObject = new Voronoi();
        public event EventHandler<PerlinNoiseGetArgs> OnSetWaterGetPerlin;

        public voronoi()
        {
            
            //初始化随机数对象
            seeder = new System.Random();
            
        }
        public void SaveData(string path, int index)
        {

            CenterFile[] centers = new CenterFile[voronoiCenterList.Count];
            for (int j = 0; j < voronoiCenterList.Count; j++)
            {
                CenterFile cf = new CenterFile(voronoiCenterList[j]);
                centers[j] = cf;
            }
            FileStream fs = new FileStream(path + "terrain" + index + "centerData", FileMode.Create);
            BinaryFormatter b = new BinaryFormatter();
            b.Serialize(fs, centers);
            fs.Close();

            CornerFile[] corners = new CornerFile[voronoiCornerList.Count];
            int i = 0;
            foreach (KeyValuePair<Site, Corner> k in voronoiCornerList)
            {
                CornerFile cf = new CornerFile(k.Value);
                corners[i] = cf;
                i++;
            }
            fs = new FileStream(path + "terrain" + index + "cornerData", FileMode.Create);
            b = new BinaryFormatter();
            b.Serialize(fs, corners);
            fs.Close();

            EdgeFile[] edges = new EdgeFile[voronoiEdgeList.Count];
            for (int j = 0; j < voronoiEdgeList.Count; j++)
            {
                edges[j] = new EdgeFile(voronoiEdgeList[j]);
            }
            fs = new FileStream(path + "terrain" + index + "edgeData", FileMode.Create);
            b = new BinaryFormatter();
            b.Serialize(fs, edges);
            fs.Close();

        }

        public void ReadData(string path, int index)
        {
            try
            {
                CenterFile[] centers;
                CornerFile[] corters;
                EdgeFile[] edges;
                FileStream fs = new FileStream(path + "terrain" + index + "centerData", FileMode.Open, FileAccess.Read, FileShare.Read);
                BinaryFormatter b = new BinaryFormatter();
                centers = b.Deserialize(fs) as CenterFile[];
                fs.Close();

                fs = new FileStream(path + "terrain" + index + "cornerData", FileMode.Open, FileAccess.Read, FileShare.Read);
                b = new BinaryFormatter();
                corters = b.Deserialize(fs) as CornerFile[];
                fs.Close();

                fs = new FileStream(path + "terrain" + index + "edgeData", FileMode.Open, FileAccess.Read, FileShare.Read);
                b = new BinaryFormatter();
                edges = b.Deserialize(fs) as EdgeFile[];
                fs.Close();

                for (int i = 0; i < centers.Count(); i++)
                {

                    CenterFileList.Add(centers[i].index, centers[i]);
                }



                for (int i = 0; i < corters.Count(); i++)
                {
                    CornerFile c = corters[i];
                    CornerFileList.Add(c.index, c);
                }


                for (int i = 0; i < edges.Count(); i++)
                {

                    EdgeFile ef = edges[i];

                    EdgeFileList.Add(ef.index, ef);
                }
                isReadByFile = true;
            }
            catch
            {
                ErrorMessage = "尝试读取位于path" + "terrain" + index + "的地形数据失败";
            }
            
        }

        public void InitVoroni(int siteCount,int width,int height)
        {
            List<DelaunayTriangle> allTriangle = new List<DelaunayTriangle>();//delaunay三角形集合
            List<Site> sitesP = new List<Site>();
            int seed = seeder.Next();
            System.Random rand = new System.Random(seed);
            List<Edge> trianglesEdgeList = new List<Edge>();//Delaunay三角形网所有边
            
            List<Edge> voronoiRayEdgeList = new List<Edge>();//voroni图外围射线边
            //初始设定点数为20
            //初始设定画布大小是500*400
            //超级三角形顶点坐标为（250,0），（0,400），（500,400）
            //点集区域为（125,200），（125,400），（375,200），（375,400），随便设置，只要满足点落在三角形区域中
            for (int i = 0; i < siteCount; i++)
            {

                PointF pf = new PointF((float)(rand.NextDouble() * width), (float)(rand.NextDouble() * height));
                //PointF pf=new PointF((float)(rand.NextDouble() * 250 + 125), (float)(rand.NextDouble() * 200 + 200));
                Site site = new Site(pf.x, pf.y);
                sitesP.Add(site);
                Center c = new Center();
                c.point = site;
                voronoiCenterList.Add(c);
                
            }

            //按点集坐标X值排序
            sitesP.Sort(new SiteSorterXY());
            voronoiCenterList.Sort(new CenterSorterXY());
            for (int i = 0; i < voronoiCenterList.Count; i++)
            {
                voronoiCenterList[i].index = i;
            }


            int relaxNum = 2;
            for (int r = 0; r < relaxNum; r++)
            {
                if (r > 0)
                {
                    allTriangle.Clear();
                    trianglesEdgeList.Clear();
                    voronoiEdgeList.Clear();
                    voronoiRayEdgeList.Clear();
                    voronoiCornerList.Clear();

                    for (int t = 0; t < voronoiCenterList.Count; t++)
                    {
                        bool isBorder = false;
                        double sumx = 0, sumy = 0;
                        double num = voronoiCenterList[t].corners.Count;
                        foreach (KeyValuePair<Site, Corner> k in voronoiCenterList[t].corners)
                        {
                            if (k.Value.point.x <= 20 || k.Value.point.x >= width - 20 || k.Value.point.y <= 20 || k.Value.point.y >= height - 20)
                            {
                                isBorder = true;
                                break;
                            }
                            sumx += k.Value.point.x;
                            sumy += k.Value.point.y;
                        }
                        Center c = new Center();
                        if (!isBorder)
                        {
                            c.point = new Site(sumx / num, sumy / num);
                            voronoiCenterList[t] = c;
                            sitesP[t] = new Site(sumx / num, sumy / num);
                        }
                        else
                        {
                            c.point = voronoiCenterList[t].point;
                            c.border = true;
                            voronoiCenterList[t] = c;
                            sitesP[t] = c.point;
                        }
                    }


                }

                //将超级三角形的三点添加到三角形网中
                Site A = new Site(0, 0);
                Site B = new Site(width, 0);
                Site C = new Site(0, height);
                Site D = new Site(width, height);
                Center CA = new Center();
                CA.point = A;
                CA.index = -1;
                Center CB = new Center();
                CB.point = B;
                CB.index = -1;
                Center CC = new Center();
                CC.point = C;
                CC.index = -1;
                Center CD = new Center();
                CD.point = D;
                CD.index = -1;
                sitesP.Add(A);
                sitesP.Add(B);
                sitesP.Add(C);
                sitesP.Add(D);

                voronoiCenterList.Add(CA);
                voronoiCenterList.Add(CB);
                voronoiCenterList.Add(CC);
                voronoiCenterList.Add(CD);

                DelaunayTriangle dt = new DelaunayTriangle(A, B, C, CA, CB, CC);
                DelaunayTriangle dt2 = new DelaunayTriangle(D, B, C, CD, CB, CC);
                allTriangle.Add(dt);
                allTriangle.Add(dt2);
                //构造Delaunay三角形网
                voroObject.setDelaunayTriangle(allTriangle, sitesP, voronoiCenterList);

                sitesP.Sort(new SiteSorterXY());
                voronoiCenterList.Sort(new CenterSorterXY());
                for (int i = 0; i < voronoiCenterList.Count; i++)
                {
                    voronoiCenterList[i].index = i;
                }
                //
                //不要移除，这样就不用画Delaunay三角形网外围边的射线
                //移除超级三角形
                //voroObject.remmoveTrianglesByOnePoint(allTriangle, A);
                //voroObject.remmoveTrianglesByOnePoint(allTriangle, B);
                //voroObject.remmoveTrianglesByOnePoint(allTriangle, C);

                //返回Delaunay三角形网所有边
                trianglesEdgeList = voroObject.returnEdgesofTriangleList(allTriangle);



                //填充neighbor
                for (int i = 0; i < allTriangle.Count; i++)
                {
                    DelaunayTriangle t = allTriangle[i];
                    try
                    {
                        t.center1.neighbors.Add(t.center2.index, t.center2);
                    }
                    catch (ArgumentException)
                    {

                    }
                    try
                    {
                        t.center1.neighbors.Add(t.center3.index, t.center2);
                    }
                    catch (ArgumentException)
                    {

                    }
                    try
                    {
                        t.center2.neighbors.Add(t.center1.index, t.center1);
                    }
                    catch (ArgumentException)
                    {

                    }
                    try
                    {
                        t.center2.neighbors.Add(t.center3.index, t.center3);
                    }
                    catch (ArgumentException)
                    {

                    }
                    try
                    {
                        t.center3.neighbors.Add(t.center1.index, t.center1);

                    }
                    catch (ArgumentException)
                    {

                    }
                    try
                    {
                        t.center3.neighbors.Add(t.center2.index, t.center2);
                    }
                    catch (ArgumentException)
                    {

                    }


                }
                //获取所有Voronoi边
                voronoiEdgeList = voroObject.returnVoronoiEdgesFromDelaunayTriangles(allTriangle, voronoiRayEdgeList, voronoiCenterList, voronoiCornerList);

                foreach (KeyValuePair<Site, Corner> k in voronoiCornerList)
                {
                    if (k.Value.point.x <= 20 || k.Value.point.x >= width - 20 || k.Value.point.y <= 20 || k.Value.point.y >= height - 20)
                    {
                        k.Value.border = true;

                    }
                }

            }
            for (int i = 0; i < voronoiCenterList.Count; i++)
            {
                voronoiCenterList[i].index = i;
            }

            for (int i = 0; i < voronoiCornerList.Count; i++)
            {
                voronoiCornerList.Values.ElementAt(i).index = i;
            }

            for (int i = 0; i < voronoiEdgeList.Count; i++)
            {
                voronoiEdgeList[i].index = i;
            }


        }
        

       
       public void SetWater(float waterRate,int width,int height)
        {
            //PerlinNoise.PerlinNoise PerlinNoise = new PerlinNoise.PerlinNoise(width,height,6);
            foreach (KeyValuePair<Site,Corner> k in voronoiCornerList)
            {
                OnSetWaterGetPerlin(this, new PerlinNoiseGetArgs((float)k.Key.x, (float)k.Key.y));
                if (k.Value.border || nowPerlinNoise < waterRate + 0.0008f * PointF.Distance(new PointF((float)k.Key.x, (float)k.Key.y), new PointF(width / 2, height / 2)))
                {
                    k.Value.water = true;
                    foreach(KeyValuePair<int,Center> kc in k.Value.touches)
                    {
                        kc.Value.water = true;
                    }
                }
            }
        }

        public void SetOcean()
        {
            Queue<Center> queue = new Queue<Center>();
            for(int i=0;i<voronoiCenterList.Count;i++)
            {
                if(voronoiCenterList[i].border)
                {
                    
                    voronoiCenterList[i].ocean = true;
                    foreach (KeyValuePair<Site, Corner> ck in voronoiCenterList[i].corners)
                    {
                        ck.Value.ocean = true;
                        ck.Value.water = true;
                    }
                    queue.Enqueue(voronoiCenterList[i]);
                }
            }
            while(queue.Count > 0)
            {
                Center c = queue.Dequeue();
                foreach(KeyValuePair<int,Center> k in c.neighbors)
                {
                    if(!k.Value.ocean)
                    {
                        if(k.Value.water)
                        {
                            k.Value.ocean = true;
                            foreach(KeyValuePair<Site,Corner> ck in k.Value.corners)
                            {
                                ck.Value.ocean = true;
                                ck.Value.water = true;
                            }
                            queue.Enqueue(k.Value);
                        }
                        else
                        {
                            foreach (KeyValuePair<Site, Corner> ck in k.Value.corners)
                            {
                                ck.Value.coast = true;
                            }
                            k.Value.coast = true;
                        }
                    }
                }
            }
            
        }

        public void SetElevation(float heightScale)
        {
            Queue<Corner> queue = new Queue<Corner>();
            foreach(KeyValuePair<Site,Corner> k in voronoiCornerList)
            {
                if(k.Value.coast)
                {
                    k.Value.elevation = 0.0f;
                    queue.Enqueue(k.Value);
                }
                else
                {
                    k.Value.elevation = -1;
                }
                foreach(KeyValuePair<int,Center> kcen in k.Value.touches)
                {
                    if(kcen.Value.water)
                    {
                        k.Value.water = true;
                        break;
                    }
                }
                if (!k.Value.water && seeder.Next(0, 100) > 80 + 0.2f * PointF.Distance(new PointF((float)k.Key.x, (float)k.Key.y), new PointF(250, 200)))
                {
                    k.Value.plane = true;
                    foreach (KeyValuePair<int, Corner> kc in k.Value.adjacent)
                    {
                        kc.Value.plane = true;
                    }
                }
            }

            while(queue.Count > 0)
            {
                Corner c = queue.Dequeue();
                float newElevation = c.elevation + 0.01f;
                foreach(KeyValuePair<int,Corner> k in c.adjacent)
                {

                    if (!c.water && !k.Value.water && !k.Value.plane)
                    {
                        newElevation += 1;
                        newElevation += seeder.Next(0,10);
                    }
                    if(newElevation < k.Value.elevation || k.Value.elevation < 0 )
                    {
                        k.Value.elevation = newElevation;
                        
                        queue.Enqueue(k.Value);
                    }
                }
                
            }

            float maxElevation = 0;

            foreach(KeyValuePair<Site,Corner> k in voronoiCornerList)
            {
                if(k.Value.elevation > maxElevation)
                {
                    maxElevation = k.Value.elevation;
                }
            }
            foreach (KeyValuePair<Site,Corner> k in voronoiCornerList)
            {
                k.Value.elevation /= maxElevation;
                k.Value.elevation /= heightScale;
                float maxEle = 0;
                Corner temp = null;
                foreach (KeyValuePair<int, Corner> kc in k.Value.adjacent)
                {
                    if((k.Value.elevation - kc.Value.elevation)>maxEle)
                    {
                        maxEle = k.Value.elevation - kc.Value.elevation;
                        temp = kc.Value;
                    }
                }
                k.Value.downslope = temp;
            }
            for(int i=0;i<voronoiCenterList.Count;i++)
            {
                float sumEle = 0;
                foreach (KeyValuePair<Site, Corner> k in voronoiCenterList[i].corners)
                {
                    sumEle += k.Value.elevation;
                }
                voronoiCenterList[i].elevation = sumEle / voronoiCenterList[i].corners.Count;
            }

        }

        public void SetRiver(int RiverCount,float minElevation)
        {
            for (int i = 0; i < RiverCount; i++)
            {
                Corner startCorner = null;
                while (startCorner == null || startCorner.elevation <= minElevation)
                {
                    int pos = seeder.Next(0, voronoiCornerList.Count);
                    startCorner = voronoiCornerList.Values.ElementAt(pos);
                }
                Corner nextCorner = startCorner;
                int lastRiver = 0;
                while(nextCorner!=null && !nextCorner.water)
                {
                    nextCorner.river += 1 + seeder.Next(0,5) + lastRiver;
                    lastRiver = nextCorner.river;
                    nextCorner.elevation /= 1 + (nextCorner.river) / 15;
                    foreach(KeyValuePair<int,Center> k in nextCorner.touches)
                    {
                        k.Value.elevation /= 1+(nextCorner.river) / 15;
                    }
                    nextCorner = nextCorner.downslope;
                }
            }
        }

        public void SetMoisture(float wet,float dryScale)
        {
            Queue<Corner> queue = new Queue<Corner>();
            foreach (KeyValuePair<Site, Corner> k in voronoiCornerList)
            {
                if ((k.Value.water && !k.Value.ocean) || k.Value.coast || k.Value.river > 0)
                {
                    k.Value.water_distance = 0;
                    queue.Enqueue(k.Value);
                }
                else
                {
                    k.Value.water_distance = -1;
                }
            }

            while (queue.Count > 0)
            {
                Corner c = queue.Dequeue();
                int newWaterdistance = c.water_distance + 1;
                foreach (KeyValuePair<int, Corner> k in c.adjacent)
                {

                    if (!c.water && !k.Value.water)
                    {
                        newWaterdistance += 1;
                        newWaterdistance += seeder.Next(0, 30);
                    }
                    if (newWaterdistance < k.Value.elevation || k.Value.elevation < 0)
                    {
                        k.Value.water_distance = newWaterdistance;
                        queue.Enqueue(k.Value);
                    }
                }

            }

            foreach (KeyValuePair<Site, Corner> k in voronoiCornerList)
            {
                k.Value.moisture = (float)Math.Pow((seeder.NextDouble()* (1 - 0.95f) + 0.95f) * wet, k.Value.water_distance);
            }
            float maxMoisture = 0;
            foreach (KeyValuePair<Site, Corner> k in voronoiCornerList)
            {
                if(k.Value.moisture > maxMoisture)
                {
                    maxMoisture = k.Value.moisture;
                }
            }
            foreach (KeyValuePair<Site, Corner> k in voronoiCornerList)
            {
                
                    k.Value.moisture /= maxMoisture;
                k.Value.moisture /= dryScale;
                
            }

            for (int i = 0; i < voronoiCenterList.Count; i++)
            {
                float sumMoi = 0;
                foreach (KeyValuePair<Site, Corner> k in voronoiCenterList[i].corners)
                {
                    sumMoi += k.Value.moisture;
                }
                voronoiCenterList[i].moisture = sumMoi / voronoiCenterList[i].corners.Count;
            }
        }

        public void SetBiome(BiomeType[,] BiomeDiagram)
        {
            for(int i=0;i<voronoiCenterList.Count;i++)
            {
                Center c = voronoiCenterList[i];
                if(!c.water && !c.coast)
                {
                    int x = (int)(c.elevation * 4);
                    int y = (int)(c.moisture * 6);
                    x = Clamp(x, 0, 3);
                    y = Clamp(y, 0, 5);
                    c.biome = BiomeDiagram[x, y];
                }
            }
        }

        public int Clamp(int value,int min,int max)
        {
            
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }

       
    }
}
