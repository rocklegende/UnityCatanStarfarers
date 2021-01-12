using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper
{

    public Vector3 unitsToPixel(Vector3 unityUnitVector)
    {
        return Vector3.zero;
        
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
        int q = point.coordinates.q;
        int r = point.coordinates.r;
        if (point.vertexNumber == 0)
        {
            HexCoordinates[] coords = { new HexCoordinates(q, r), new HexCoordinates(q + 1, r - 1), new HexCoordinates(q, r - 1) };
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
