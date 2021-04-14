using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum Direction
{
    W,
    E,
    NW,
    NE,
    SW,
    SE
}

public class HexComparer : IComparer
{
    public int Compare(object x, object y)
    {
        var _x = (HexCoordinates)x;
        var _y = (HexCoordinates)y;
        if (_x.q == _y.q)
        {
            return _x.r - _y.r;
        }
        else
        {
            return _x.q - _y.q;
        }
    }
}

[Serializable]
public class HexCoordinates
{
    public int q;
    public int r;

    public override bool Equals(System.Object obj)
    {
        var hex = obj as HexCoordinates;

        if (hex == null)
        {
            // If it is null then it is not equal to this instance.
            return false;
        }

        return this.q == hex.q && this.r == hex.r;
    }

    public HexCoordinates(int q, int r)
    {
        this.q = q;
        this.r = r;
    }

    public HexCoordinates NW()
    {
        return new HexCoordinates(q, r - 1);
    }

    public HexCoordinates NE()
    {
        return new HexCoordinates(q + 1, r - 1);
    }

    public HexCoordinates E()
    {
        return new HexCoordinates(q + 1, r);
    }

    public HexCoordinates W()
    {
        return new HexCoordinates(q - 1, r);
    }

    public HexCoordinates SW()
    {
        return new HexCoordinates(q - 1, r + 1);
    }

    public HexCoordinates SE()
    {
        return new HexCoordinates(q, r + 1);
    }

    public int DistanceTo(HexCoordinates hex)
    {
        Vector3 hex1InCube = this.getCubeRepresentation();
        Vector3 hex2InCube = hex.getCubeRepresentation();
        return new Helper().cubeDistance(hex1InCube, hex2InCube);
    }

    public void print()
    {
        Logger.log("(HEX) q: " + q + "r: " + r);
    }

    public float getRadius()
    {
        return Constants.HEX_RADIUS;
    }

    public Vector2 FlatToPixel()
    {
        var x = getRadius() * (3.0f / 2.0f * q);
        var y = -getRadius() * (Mathf.Sqrt(3) / 2 * q + Mathf.Sqrt(3) * r);
        return new Vector2(x, y);
    }

    public Vector2 PointyToPixel()
    {
        var x = Constants.paddingRight * getRadius() * (Mathf.Sqrt(3) * q + Mathf.Sqrt(3) / 2 * r);
        var y = Constants.paddingDown * -getRadius() * (3.0f / 2.0f * r);
        return new Vector2(x, y);
    }

    public HexCoordinates getNeighborCoordinates(Direction direction)
    {
        switch (direction)
        {
            case Direction.E:
                return new HexCoordinates(q + 1, r);
            case Direction.W:
                return new HexCoordinates(q - 1, r);
            case Direction.NE:
                return new HexCoordinates(q + 1, r - 1);
            case Direction.NW:
                return new HexCoordinates(q, r - 1);
            case Direction.SW:
                return new HexCoordinates(q - 1, r + 1);
            case Direction.SE:
                return new HexCoordinates(q, r + 1);
        }
        return null;
    }

    public Vector3 getCubeRepresentation()
    {
        return new Vector3(q, -q - r, r);
    }
}
