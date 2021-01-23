using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundingBox
{
    public float minY;
    public float maxY;
    public float minX;
    public float maxX;

    public BoundingBox(float minY, float maxY, float minX, float maxX)
    {
        this.minY = minY;
        this.maxY = maxY;
        this.minX = minX;
        this.maxX = maxX;
    }
}

public class Helper
{

    public SpacePoint[] SpacePointArrayFromShortRep(string[] shortRepresentation)
    {
        List<SpacePoint> points = new List<SpacePoint>();
        char[] charSeparators = new char[] { ',' };
        foreach (string str in shortRepresentation) {
            string[] result = str.Split(charSeparators);
            SpacePoint newPoint = new SpacePoint(new HexCoordinates(Int32.Parse(result[0]), Int32.Parse(result[1])), Int32.Parse(result[2]));
            points.Add(newPoint);
        }

        return points.ToArray();
    }

    public Vector3 unitsToPixel(Vector3 unityUnitVector)
    {
        return Vector3.zero;
        
    }

    public BoundingBox GetLowestPoint(Transform parent)
    {
        float minY = 10000.0f;
        float minX = 10000.0f;
        float maxX = -10000.0f;
        float maxY = -10000.0f;
        Transform[] childObjects = parent.GetComponentsInChildren<Transform>();
        foreach (Transform child in childObjects)
        {
            Logger.log(child.gameObject.name);
            Logger.log(child.transform.position.y);
            Renderer renderer = child.GetComponent<Renderer>();
            //if (renderer != null)
            //{
                float y = child.transform.position.y;
                float x = child.transform.position.x;
                //Debug.Log(child.transform.position.y);
                //float lowPoint = child.transform.position.y - renderer.bounds.extents.y;
                if (y < minY)
                {
                    minY = y;
                }
                if (y > maxY)
                {
                    maxY = y;
                }
                if (x < minX)
                {
                    minX = x;
                }
                if (x > maxX)
                {
                    maxX = x;
                }
            //}
            
        }
        return new BoundingBox(minY, maxY, minX, maxX);
    }


    public int cubeDistance(Vector3 a, Vector3 b)
    {
        float[] values = new float[] { Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y), Mathf.Abs(a.z - b.z) };
        return (int)Mathf.Max(values);
    }

    public Vector2 getVectorFromAngle(float radians)
    {
        return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
    }

    public Vector3 ToVector3(Vector2 vec2)
    {
        return new Vector3(vec2.x, vec2.y);
    }

    public bool HexCoordinateGroupsAreEqual(HexCoordinates[] hexCoords1, HexCoordinates[] hexCoords2)
    {
        if (hexCoords1.Length != hexCoords2.Length)
        {
            return false;
        }

        Array.Sort(hexCoords1, new HexComparer());
        Array.Sort(hexCoords2, new HexComparer());

        for (int i = 0; i < hexCoords1.Length; i++)
        {
            if (!hexCoords1[i].Equals(hexCoords2[i]))
            {
                return false;
            }
        }
        return true;
    }

    public HexCoordinates[] getAllHexesDistanceAwayFromHex(HexCoordinates givenHex, int distance)
    {
        HexCoordinates[] allHexesInRange = getAllHexCoordinatesInsideRange(givenHex, distance);
        List<HexCoordinates> result = new List<HexCoordinates>();

        foreach(HexCoordinates hex in allHexesInRange)
        {
            if (givenHex.DistanceTo(hex) == distance)
            {
                result.Add(hex);
            }
        }
        return result.ToArray();

    }

    public HexCoordinates[] getAllHexCoordinatesInsideRange(HexCoordinates origin, int distance)
    {
        int lowest_q = origin.q - distance;
        int highest_q = origin.q + distance;
        int lowest_r = origin.r - distance;
        int highest_r = origin.r + distance;

        List<HexCoordinates> hexes = new List<HexCoordinates>();
        for (int q = lowest_q; q <= highest_q; q++)
        {
            for (int r = lowest_r; r <= highest_r; r++)
            {
                hexes.Add(new HexCoordinates(q, r));
            }
        }
        return hexes.ToArray();
    }

    public HexCoordinates[] getCoordinatesOfHexesAtPoint(SpacePoint point)
    {
        // returns HexCoordinates at given SpacePoint in clock-wise manner (can return 1,2 or 3 pairs of coordinates)

        int q = point.coordinates.q;
        int r = point.coordinates.r;
        if (point.vertexNumber == 0)
        {
            HexCoordinates[] coords = { new HexCoordinates(q, r), new HexCoordinates(q, r - 1), new HexCoordinates(q + 1, r - 1) };
            return coords;
        }
        else
        {
            HexCoordinates[] coords = { new HexCoordinates(q, r), new HexCoordinates(q - 1, r), new HexCoordinates(q, r - 1) };
            return coords;
        }
    }

    public void AddHexagonRenderer(GameObject gobj, Vector2 position)
    {
        LineRenderer lr = gobj.AddComponent<LineRenderer>();
        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;
        lr.positionCount = 7;
        lr.startColor = Color.blue;
        lr.endColor = Color.blue;
        Vector3[] positions = new Vector3[lr.positionCount];
        for (int i = 0; i < lr.positionCount; i++)
        {
            Vector3 newPosition = position + Constants.HEX_RADIUS * getVectorFromAngle(i * Mathf.PI / 3 + Mathf.PI / 6);
            positions[i] = newPosition;
        }

        lr.SetPositions(positions);
    }

    

}
