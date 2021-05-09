using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using com.onebuckgames.UnityStarFarers;

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

    public static List<int> GetIndexesOfPlayersExceptPlayer(List<Player> allPlayers, int excludedPlayerIndex)
    {
        var indexesOfPlayersOtherThanMain = new List<int>();
        for (int i = 0; i < allPlayers.Count; i++)
        {
            if (i != excludedPlayerIndex)
            {
                indexesOfPlayersOtherThanMain.Add(i);
            }
        }

        return indexesOfPlayersOtherThanMain;
    }

    public static GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation, byte group = 0, object[] data = null)
    {
        return SFEnvironment.Instance.instantiationStrategy.Instantiate(prefab, position, rotation, group, data);
        //return GameObject.Instantiate(prefab, position, rotation);
    }

    /// <summary>
    /// Returns clockwise order of player beginning with the current player at turn.
    /// Use this if players need to take some action one after another.<br></br>
    /// <b>NOTE:</b> Zero indexing!
    /// </summary>
    /// <returns>List(int)</returns>
    public static List<int> NextPlayersClockwise(int currentPlayer, int numberOfPlayers)
    {
        if (currentPlayer >= numberOfPlayers)
        {
            throw new ArgumentException("currentPlayer cant be higher or same than numberOfPlayers");
        }
        var list = new List<int>();
        int current = currentPlayer;
        for (int i = 0; i < numberOfPlayers; i++)
        {
            list.Add(current);
            current = NextPlayerClockwise(current, numberOfPlayers);
        }
        return list;
    }

    /// <summary>
    /// Returns index of the next player in clockwise direction.
    /// </summary>
    /// <param name="currentPlayer"></param>
    /// <param name="numberOfPlayers"></param>
    /// <returns></returns>
    public static int NextPlayerClockwise(int currentPlayer, int numberOfPlayers)
    {
        if (currentPlayer >= numberOfPlayers)
        {
            throw new ArgumentException("currentPlayer cant be higher or same than numberOfPlayers");
        }
        return NextPlayerClockwiseStepsAway(currentPlayer, numberOfPlayers, 1);
    }

    public static int NextPlayerClockwiseStepsAway(int currentPlayer, int numberOfPlayers, int steps)
    {
        if (currentPlayer >= numberOfPlayers)
        {
            throw new ArgumentException("currentPlayer cant be higher or same than numberOfPlayers");
        }
        currentPlayer += steps;
        return currentPlayer % numberOfPlayers;
    }

    /// <summary>
    /// Returns index of the previous player in clockwise direction.
    /// </summary>
    /// <param name="currentPlayer"></param>
    /// <param name="numberOfPlayers"></param>
    /// <returns></returns>
    public static int PreviousPlayerClockwise(int currentPlayer, int numberOfPlayers)
    {
        if (currentPlayer >= numberOfPlayers)
        {
            throw new ArgumentException("currentPlayer cant be higher or same than numberOfPlayers");
        }

        var prevPlayerIndex = PreviousPlayerClockwiseStepsAway(currentPlayer, numberOfPlayers, 1);
        return prevPlayerIndex;
    }

    /// <summary>
    /// Returns index of the previous player in clockwise direction stesp away.
    /// </summary>
    /// <param name="currentPlayer"></param>
    /// <param name="numberOfPlayers"></param>
    /// <returns></returns>
    public static int PreviousPlayerClockwiseStepsAway(int currentPlayer, int numberOfPlayers, int steps)
    {
        if (currentPlayer >= numberOfPlayers)
        {
            throw new ArgumentException("currentPlayer cant be higher or same than numberOfPlayers");
        }

        var stepsRight = numberOfPlayers - steps % numberOfPlayers;

        var nextPlayerClockwiseIndex = NextPlayerClockwiseStepsAway(currentPlayer, numberOfPlayers, stepsRight);
        return nextPlayerClockwiseIndex;
    }

    public ResourceCard[] GetAllResourceCardTypes()
    {
        return new ResourceCard[]
        {
            new OreCard(),
            new FuelCard(),
            new FoodCard(),
            new CarbonCard(),
            new GoodsCard()
        };
    }

    public void DestroyAllChildGameObjects(GameObject gobj)
    {
        for (int i = 0; i < gobj.transform.childCount; i++)
        {
            GameObject.Destroy(gobj.transform.GetChild(i));
        }
    }

    public static List<T> CreateCopyOfList<T>(List<T> listToCopy)
    {
        var newList = new List<T>();
        newList.AddRange(listToCopy);
        return newList;
    }

    public List<Token> GetAllTokenOfPlayers(Player[] players)
    {
        List<Token> tokens = new List<Token>();
        foreach (Player player in players)
        {
            foreach (Token token in player.tokens)
            {
                tokens.Add(token);
            }
        }
        return tokens;
    }

    public Hand GetHandWithResources(int food = 0, int goods = 0, int fuel = 0, int ore = 0, int carbon = 0)
    {
        var hand = new Hand();
        for (int i = 0; i < food; i++)
        {
            hand.AddCard(new FoodCard());
        }

        for (int i = 0; i < goods; i++)
        {
            hand.AddCard(new GoodsCard());
        }

        for (int i = 0; i < ore; i++)
        {
            hand.AddCard(new OreCard());
        }

        for (int i = 0; i < fuel; i++)
        {
            hand.AddCard(new FuelCard());
        }

        for (int i = 0; i < carbon; i++)
        {
            hand.AddCard(new CarbonCard());
        }
        return hand;

    }

    public ResourceCard CreateResourceCardFromResource(Resource resource)
    {
        if (resource is CarbonResource)
        {
            return new CarbonCard();
        } else if (resource is FuelResource)
        {
            return new FuelCard();
        }
        else if (resource is FoodResource)
        {
            return new FoodCard();
        }
        else if (resource is GoodsResource)
        {
            return new GoodsCard();
        }
        else if (resource is OreResource)
        {
            return new OreCard();
        } else
        {
            return null;
        }
    }

    public Sprite CreateSpriteFromImageName(string imageName) 
    {
        Texture2D texture = Resources.Load(imageName) as Texture2D;
        Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.0f, 0.0f));
        return sprite;
    }

    public List<SpacePoint> SpacePointArrayFromShortRep(string[] shortRepresentation)
    {
        List<SpacePoint> points = new List<SpacePoint>();
        char[] charSeparators = new char[] { ',' };
        foreach (string str in shortRepresentation) {
            string[] result = str.Split(charSeparators);
            SpacePoint newPoint = new SpacePoint(new HexCoordinates(Int32.Parse(result[0]), Int32.Parse(result[1])), Int32.Parse(result[2]));
            points.Add(newPoint);
        }

        return points;
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
    /// <summary>
    /// returns HexCoordinates at given SpacePoint in clock-wise manner (can return 1,2 or 3 pairs of coordinates)
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public HexCoordinates[] getCoordinatesOfHexesAtPoint(SpacePoint point)
    {
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
