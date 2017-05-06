
using System;
using System.Collections.Generic;
namespace lvlv_voronoi
{


    [Serializable]
    public class CenterFile
    {
        public int index;
        public bool plane;
        public double x;
        public double y;
        public bool water;  // lake or ocean
        public bool ocean;  // ocean
        public bool coast;  // land polygon touching an ocean
        public bool border;  // at the edge of the map
        public int biome;
        public float elevation = 0;  // 0.0-1.0
        public float moisture = 0;  // 0.0-1.0
        public int[] neighbors;
        public int[] borders;
        public int[] corners;

        public CenterFile(Center c)
        {
            index = c.index;
            x = c.point.x;
            y = c.point.y;
            water = c.water;
            ocean = c.ocean;
            coast = c.coast;
            border = c.border;
            biome = (int)c.biome;
            elevation = c.elevation;
            moisture = c.moisture;
            plane = c.plane;
            neighbors = new int[c.neighbors.Count];
            int i = 0;
            foreach (KeyValuePair<int, Center> k in c.neighbors)
            {
                neighbors[i] = k.Value.index;
                i++;
            }
            corners = new int[c.corners.Count];
            i = 0;
            foreach (KeyValuePair<Site, Corner> k in c.corners)
            {
                corners[i] = k.Value.index;
                i++;
            }

            borders = new int[c.borders.Count];
            i = 0;
            foreach (KeyValuePair<Edge, Edge> k in c.borders)
            {
                borders[i] = k.Value.index;
                i++;
            }
        }
    }

    [Serializable]
    public class CornerFile
    {
        public int index;
        public double x;
        public bool plane;
        public double y;
        public bool water;  // lake or ocean
        public bool ocean;  // ocean
        public bool coast;  // land polygon touching an ocean
        public bool border;  // at the edge of the map
        public float elevation = 0;  // 0.0-1.0
        public float moisture = 0;  // 0.0-1.0
        public int[] touches;
        public int[] protrudes;
        public int[] adjacent;
        public int river;  // 0 if no river, or volume of water in river
        public int downslope;  // pointer to adjacent corner most downhill
        public int watershed;  // pointer to coastal corner, or null
        public int watershed_size;
        public int water_distance;

        public CornerFile(Corner c)
        {
            index = c.index;
            x = c.point.x;
            y = c.point.y;
            water = c.water;
            ocean = c.ocean;
            coast = c.coast;
            plane = c.plane;
            border = c.border;
            elevation = c.elevation;
            moisture = c.moisture;
            river = c.river;
            if (c.downslope == null)
                downslope = -1;
            else
                downslope = c.downslope.index;
            if (c.watershed == null)
                watershed = -1;
            else
                watershed = c.watershed.index;
            watershed_size = c.watershed_size;
            water_distance = c.water_distance;
            int i = 0;
            touches = new int[c.touches.Count];
            foreach (KeyValuePair<int, Center> k in c.touches)
            {
                touches[i] = k.Value.index;
                i++;
            }
            adjacent = new int[c.adjacent.Count];
            i = 0;
            foreach (KeyValuePair<int, Corner> k in c.adjacent)
            {
                adjacent[i] = k.Value.index;
                i++;
            }

            protrudes = new int[c.protrudes.Count];
            i = 0;
            foreach (KeyValuePair<Edge, Edge> k in c.protrudes)
            {
                protrudes[i] = k.Value.index;
                i++;
            }
        }
    }

    [Serializable]
    public class EdgeFile
    {
        public int index;
        public double ax;
        public double ay;
        public double bx;
        public double by;
        public int ca;
        public int cb;
        public int cora;
        public int corb;
        public int river;

        public EdgeFile(Edge e)
        {
            index = e.index;
            ax = e.a.x;
            ay = e.a.y;
            bx = e.b.x;
            by = e.b.y;
            if (e.ca == null)
            {
                ca = -1;
            }
            else
            {
                ca = e.ca.index;
            }

            if (e.cb == null)
            {
                cb = -1;
            }
            else
            {
                cb = e.cb.index;
            }

            if (e.cora == null)
            {
                cora = -1;
            }
            else
            {
                cora = e.cora.index;
            }

            if (e.corb == null)
            {
                corb = -1;
            }
            else
            {
                corb = e.corb.index;
            }

            river = e.river;
        }
    }

    //点
    [Serializable]
	public class Site
	{
		public double x, y;
 
        public Site()
        { }
        public Site(double x, double y)
		{
            this.x = x;
            this.y = y;
		}
	}
	
    [Serializable]
    public class PointF
    {
        public float x;
        public float y;

        public PointF(float x,float y)
        {
            this.x = x;
            this.y = y;
        }

        public static float Distance(PointF a,PointF b)
        {
            float x = b.x - a.x;
            float y = b.y - a.y;
            return (float)Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
        }
    }

    //三角形的边
    [Serializable]
	public class CenterEdge
	{
        
        public Center a, b;
        public CenterEdge(Center a, Center b)
        {
            this.a = a;
            this.b = b;
            
        }
	}

    [Serializable]
    public class CornerEdge
    {

        public Corner a, b;
        public CornerEdge(Corner a, Corner b)
        {
            this.a = a;
            this.b = b;

        }
    }
    [Serializable]
    public class Edge
    {
        public int index;
        public Site a, b;
        public Center ca, cb;
        public Corner cora, corb;
        public int river;
        public Edge()
        {

        }
        public Edge(Site a, Site b,Center ca,Center cb)
        {
            this.a = a;
            this.b = b;
            this.ca = ca;
            this.cb = cb;

        }

        public Edge(Site a, Site b, Corner ca, Corner cb)
        {
            this.a = a;
            this.b = b;
            this.cora = ca;
            this.corb = cb;

        }
    }

    //自定义排序规则
    [Serializable]
    public class SiteSorterXY : IComparer<Site>
	{
        public int Compare(Site p1, Site p2)
		{
			if ( p1.x > p2.x ) return 1;
            if (p1.x < p2.x) return -1;
			return 0;
		}
	}
    [Serializable]
    public class CenterSorterXY : IComparer<Center>
    {
        public int Compare(Center p1, Center p2)
        {
            if (p1.point.x > p2.point.x) return 1;
            if (p1.point.x < p2.point.x) return -1;
            return 0;
        }
    }
    [Serializable]
    public class DelaunayTriangle
    {
        Voronoi voronoi = new Voronoi();
        public Site site1, site2, site3;//三角形三点
        public Center center1, center2, center3;
        public Site centerPoint;//外界圆圆心
        public double radius;//外接圆半径
        public List<DelaunayTriangle> adjoinTriangle;//邻接三角形 

        

        public DelaunayTriangle(Site site1, Site site2, Site site3,Center center1, Center center2, Center center3)
        {
            centerPoint = new Site();
            this.site1 = site1;
            this.site2 = site2;
            this.site3 = site3;
            this.center1 = center1;

            this.center2 = center2;

            this.center3 = center3;

            //构造外接圆圆心以及半径
            voronoi.circle_center(centerPoint, site1, site2, site3, ref radius);
        }
    }
    [Serializable]
    public enum BiomeType
    {
        SNOW,
        TUNDRA,
        ROCK,
        SCORCH,
        CONIFEROUS,
        SHRUB,
        TEMPERATE_DESERT,
        TEMPERATE_RAINFOREST,
        TEMPERATE_DECIDUOUS,
        GRASSLAND,
        RAINFOREST,
        TROPICAL_MONSOON,
        SUBTROPICAL_DESERT
    }
    [Serializable]
    public class Center
    {
        public int index;

        public Site point;  // location
        public bool water;  // lake or ocean
        public bool ocean;  // ocean
        public bool coast;  // land polygon touching an ocean
        public bool border;  // at the edge of the map
        public bool plane;
        public BiomeType biome;  // biome type (see article)
        public float elevation = 0;  // 0.0-1.0
        public float moisture = 0;  // 0.0-1.0

        public Dictionary<int,Center> neighbors;
        public Dictionary<Edge,Edge> borders;
        public Dictionary<Site,Corner> corners;

        public Center()
        {
            neighbors = new Dictionary<int, Center>();
            borders = new Dictionary<Edge, Edge>();
            corners = new Dictionary<Site, Corner>();
        }

     
    }
    [Serializable]
    public class Corner
    {
        public int index;

        public Site point;  // location
        public bool water;  // lake or ocean
        public bool ocean;  // ocean
        public bool coast;  // land polygon touching an ocean
        public bool border;  // at the edge of the map
        public bool plane;
        public float elevation = 0;  // 0.0-1.0
        public float moisture = 0;  // 0.0-1.0

        public Dictionary<int,Center> touches;
        public Dictionary<Edge,Edge> protrudes;
        public Dictionary<int,Corner> adjacent;

        public int river;  // 0 if no river, or volume of water in river
        public Corner downslope;  // pointer to adjacent corner most downhill
        public Corner watershed;  // pointer to coastal corner, or null
        public int watershed_size;
        public int water_distance;

        public Corner()
        {
            touches = new Dictionary<int, Center>();
            protrudes = new Dictionary<Edge, Edge>();
            adjacent = new Dictionary<int, Corner>();
        }
    }

}
