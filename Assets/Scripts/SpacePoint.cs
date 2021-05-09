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

[Serializable]
public class SpacePoint
{
    public int vertexNumber;
    public HexCoordinates coordinates;
    public bool visited = false;
    public SpacePoint prev = null;

    /// <summary>
    /// Point to use if the token position is not relevant anymore, like after settling on a trade station.
    /// </summary>
    public static SpacePoint GarbagePoint = new SpacePoint(100, 100, 0);

    public bool IsEqualTo(SpacePoint other)
    {
        if (other == null)
        {
            // If it is null then it is not equal to this instance.
            return false;
        }

        return this.coordinates.Equals(other.coordinates) && this.vertexNumber == other.vertexNumber;
    }

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

    public override int GetHashCode()
    {
        return string.Format("(CityPosition) q:{0}, r:{1}, vertex:{2}", this.coordinates.q, this.coordinates.r, vertexNumber).GetHashCode();
    }

    public override string ToString()
    {
        return "(SpacePoint) q:" + this.coordinates.q + ", r:" + this.coordinates.r + "vertex: " + vertexNumber;
    }

    public void print()
    {
        Logger.log(ToString());
    }

    public List<SpacePoint> GetNeighbors()
    {
        if (vertexNumber == 0)
        {
            return new List<SpacePoint>() {
                new SpacePoint(new HexCoordinates(coordinates.q + 1, coordinates.r), 1 ),
                new SpacePoint(new HexCoordinates(coordinates.q + 1, coordinates.r - 1), 1 ),
                new SpacePoint(new HexCoordinates(coordinates.q, coordinates.r), 1 )
            };
        } else if (vertexNumber == 1)
        {
            return new List<SpacePoint>() {
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

    public static SpacePoint Zero =  new SpacePoint(0, 0, 0); 

    public SpacePoint(int q, int r, int vertexNumber)
    {
        this.coordinates = new HexCoordinates(q, r);
        this.vertexNumber = vertexNumber;

        if (this.vertexNumber < 0 || this.vertexNumber > 1)
        {
            throw new ArgumentOutOfRangeException("vertexNumber has to be 0 or 1");
        }
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
