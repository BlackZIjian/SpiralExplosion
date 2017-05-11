using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Sphere
{
    public Vector3 center;
    public float radius;
}
public class SightCone : MonoBehaviour
{
    public Vector3 startPoint;
    public Vector3 direction;
    public float nearDis;
    public float farDis;
    public float angle;
    public Vector3 characterUp;
    bool isRonating = false;
    float remainAngle = 0;
    float ronateDelta = 0;
    Quaternion finalQua = Quaternion.AngleAxis(0, new Vector3(1, 0, 0));
    List<Sphere> spheres = new List<Sphere>();

    public void Start()
    {
        if (Vector3.Dot(direction, characterUp) != 0)
        {
            Debug.Log("up与direction不垂直");
        }
        float lastRadius = 0;
        float lastX = nearDis;
        float sin = Mathf.Sin(angle / 2 * Mathf.PI / 180);
        float remainDis = farDis;
        direction = direction.normalized;
        while (remainDis > 0)
        {
            lastRadius = lastX * (1-sin) / sin;
            lastX = lastX + lastRadius;
            Sphere sph = new Sphere();
            sph.center = startPoint + direction * lastX;
            sph.radius = lastRadius;
            spheres.Add(sph);
            remainDis -= lastX + lastRadius;
        }
        
    }

    //public void RonateTo(float xAngle, float delta)
    //{
    //    isRonating = true;
    //    remainAngle = xAngle;
    //    ronateDelta = delta;
    //    if (remainAngle < 0)
    //    {
    //        delta = -delta;
    //    }
    //}
    //public void RonateTo(Vector3 targetDirection, float delta)
    //{
    //    isRonating = true;
    //    Vector3 plane = Math3d.ProjectVectorOnPlane(characterUp, targetDirection);
    //    Quaternion qua = Quaternion.FromToRotation(direction, plane);
    //    Vector3 v;
    //    qua.ToAngleAxis(out remainAngle, out v);
    //    ronateDelta = delta;
    //    if (remainAngle < 0)
    //    {
    //        delta = -delta;
    //    }
    //}
    public Collider[] GetCastColliders(int layermask)
    {
        return GetConeCastColliders(startPoint, direction, nearDis, farDis, angle, layermask);
    }
    public static Collider[] GetConeCastColliders(Vector3 startPoint, Vector3 direction, float nearDis, float farDis, float angle, int layermask)
    {
        float lastRadius = 0;
        float lastX = nearDis;
        float sin = Mathf.Sin(angle / 2);
        direction = direction.normalized;
        Dictionary<Collider, Collider> colliders = new Dictionary<Collider, Collider>();
        while (true)
        {
            lastRadius = lastX * (1 - sin) / sin;
            lastX = lastX + lastRadius;
            Collider[] cs = Physics.OverlapSphere(startPoint + direction * lastX, lastRadius, layermask);
            for (int i = 0; i < cs.Length; i++)
            {

                Bounds bs = cs[i].bounds;
                bool isCanSee = false;
                Vector3[] vers = new Vector3[9];
                vers[0] = bs.center;
                vers[1] = bs.center + new Vector3(bs.extents.x, bs.extents.y, bs.extents.z);
                vers[2] = bs.center + new Vector3(-bs.extents.x, bs.extents.y, bs.extents.z);
                vers[3] = bs.center + new Vector3(bs.extents.x, -bs.extents.y, bs.extents.z);
                vers[4] = bs.center + new Vector3(bs.extents.x, bs.extents.y, -bs.extents.z);
                vers[5] = bs.center + new Vector3(-bs.extents.x, -bs.extents.y, bs.extents.z);
                vers[6] = bs.center + new Vector3(-bs.extents.x, bs.extents.y, -bs.extents.z);
                vers[7] = bs.center + new Vector3(bs.extents.x, -bs.extents.y, -bs.extents.z);
                vers[8] = bs.center + new Vector3(-bs.extents.x, -bs.extents.y, -bs.extents.z);
                for (int k = 0; k < 9; k++)
                {
                    Vector3 tar = vers[k];
                    Ray ray = new Ray(startPoint, tar - startPoint);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {
                        if (hit.collider == cs[i])
                        {
                            isCanSee = true;
                            break;
                        }
                    }
                }
                if (isCanSee)
                {
                    colliders[cs[i]] = cs[i];
                }
            }
            if ((lastX + 2 * lastRadius) >= farDis)
            {
                break;
            }
        }
        Collider[] cols = new Collider[colliders.Count];
        int j = 0;
        foreach (KeyValuePair<Collider, Collider> k in colliders)
        {
            cols[j] = k.Key;
            j++;
        }
        return cols;
    }


    public void Update()
    {
        
    }
}
