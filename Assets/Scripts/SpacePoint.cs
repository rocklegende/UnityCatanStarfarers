using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpacePointComparer : IComparer
{
    public int Compare(object x, object y)
    {
        var _x = (SpacePoint)x;
        var _y = (SpacePoint)y;
        if (_x.coordinates == _y.coordinates)
        {
            return _x.vertexNumber - _y.vertexNumber;
        }
        else
        {
            HexComparer hexComparer = new HexComparer();
            return hexComparer.Compare(_x.coordinates, _y.coordinates);
        }
    }
}

public class SpacePoint
{
    public int vertexNumber;
    public HexCoordinates coordinates;
    public bool visited = false;
    public SpacePoint prev = null;

    public override bool Equals(System.Object obj)
    {
        var cityPos = obj as SpacePoint;

        if (cityPos == null)
        {
            // If it is null then it is not equal to this instance.
            return false;
        }

        return this.coordinates.Equals(cityPos.coordinates) && this.vertexNumber == cityPos.vertexNumber;
    }

    public void print()
    {
        Logger.log("(CityPosition) q:" + this.coordinates.q + ", r:" + this.coordinates.r + "vertex: " + vertexNumber);
    }

    public SpacePoint[] GetNeighbors()
    {
        if (vertexNumber == 0)
        {
            return new SpacePoint[] {
                new SpacePoint(new HexCoordinates(coordinates.q + 1, coordinates.r), 1 ),
                new SpacePoint(new HexCoordinates(coordinates.q + 1, coordinates.r - 1), 1 ),
                new SpacePoint(new HexCoordinates(coordinates.q, coordinates.r), 1 )
            };
        } else if (vertexNumber == 1)
        {
            return new SpacePoint[] {
                new SpacePoint(new HexCoordinates(coordinates.q - 1, coordinates.r + 1), 0 ),
                new SpacePoint(new HexCoordinates(coordinates.q - 1, coordinates.r), 0 ),
                new SpacePoint(new HexCoordinates(coordinates.q, coordinates.r), 0 )
            };
        } else
        {
            throw new ArgumentException("Vertexnumber has to be between 0 and 1");
        }

    }

    public int DistanceTo(SpacePoint pos)
    {
        SpacePoint[] MinMax = new SpacePoint[] { this, pos };
        Array.Sort(MinMax, new SpacePointComparer());

        Helper helper = new Helper();

        //Special case 1
        if (this.coordinates.Equals(pos.coordinates))
        {
            return Mathf.Abs(this.vertexNumber - pos.vertexNumber);
        }

        //Special case 2
        if (this.coordinates.q == pos.coordinates.q && this.vertexNumber != pos.vertexNumber) 
        {
            return this.coordinates.DistanceTo(pos.coordinates) * 2 + Mathf.Abs(this.vertexNumber - pos.vertexNumber);
        }

        //default case
        return this.coordinates.DistanceTo(pos.coordinates) * 2 + (MinMax[0].vertexNumber - MinMax[1].vertexNumber);
    }

    public SpacePoint(HexCoordinates coordinates, int vertexNumber)
    {
        this.coordinates = coordinates;
        this.vertexNumber = vertexNumber;

        if (this.vertexNumber < 0 || this.vertexNumber > 1)
        {
            throw new ArgumentOutOfRangeException("vertexNumber has to be 0 or 1");
        }
    }

    public Vector2 ToUnityPosition()
    {
        Helper h = new Helper();
        float radians = Mathf.PI / 2 + vertexNumber * Mathf.PI / 3;
        return coordinates.PointyToPixel() + Constants.HEX_RADIUS * h.getVectorFromAngle(radians);
    }
}
