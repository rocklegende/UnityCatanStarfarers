using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants {
    public const float HEX_RADIUS = 1.0f;
    public const int TOKEN_LAYER = -2;
    public const float paddingRight = 1.0f;
    public const float paddingDown = 1.0f;
}


public class MapScript : MonoBehaviour
{

    public GameObject spacePointButton;
    public GameObject hexPrefab;
    public GameObject tokenPrefab;
    GameObject actualTokenInScene;
    public Camera cam;
    Map map;
    GameObject[] hexagonGameObjects;

    void Start()
    {
        Tile_[,] mapRepresentation = {
            { new NullTile(), new NullTile(), new OreResourceTile(), new OreResourceTile(), new BorderTile() },
            { new NullTile(), new NullTile(), new OreResourceTile(), new OreResourceTile(), new BorderTile() },
            { new NullTile(), new FoodResourceTile(), new FoodResourceTile() , new BorderTile(), new NullTile() },
            { new NullTile(), new FoodResourceTile(), new FoodResourceTile(), new BorderTile(), new NullTile() },
            { new FoodResourceTile(), new FoodResourceTile(), new BorderTile(), new BorderTile(), new NullTile() },
        };

        map = new Map(mapRepresentation, this);
        hexagonGameObjects = CreateMap(map);

        SpacePoint point = new SpacePoint(new HexCoordinates(0, 2), 1);

        CreateButtonAtSpacePoint(point);
        CreateTokenAtSpacePoint(map.getAllSpacePointsInDistance(point, 2)[0]);


        ChipGroup squareChipGroup = new SquareChipGroup();
        ChipGroup circleChipGroup = new CircleChipGroup();
        ChipGroup triangleChipGroup = new TriangleChipGroup();

        AssignDiceChipToHex(new PirateToken(new FreightPodsBeatCondition(3), squareChipGroup), hexagonGameObjects[3]);
        AssignDiceChipToHex(new PirateToken(new FreightPodsBeatCondition(3), circleChipGroup), hexagonGameObjects[4]);
        AssignDiceChipToHex(new DiceChip6(triangleChipGroup), hexagonGameObjects[5]);

        //FlipDiceChipAtHex(new HexCoordinates(0, 0));

        //Tile_[] tiles = { new FoodResourceTile(), new OreResourceTile(), new CarbonResourceTile() };
        //TileGroup startTileGroup = new TileGroup(tiles);
        //map.SetTileGroupAtSpacePoint(startTileGroup, point);

        CenterCamera();

    }

    GameObject FindHexGameobjectAt(HexCoordinates coords)
    {
        foreach (GameObject hexObject in hexagonGameObjects)
        {
            HexScript script = hexObject.GetComponent<HexScript>();
            if (script.hexCoords.Equals(coords))
            {
                return hexObject;
            }
        }

        return null;
    }

    public void AssignDiceChipToHex(DiceChip dc, GameObject hexGameObject)
    {
        hexGameObject.GetComponent<HexScript>().SetDiceChip(dc);
    }

    public void FlipDiceChipAtHex(HexCoordinates coordinates)
    {
        // Option 1:
        // go through every hexagonObject, retrieve the HexScript and look if the hex coordinates match, then perform action

        GameObject hex = FindHexGameobjectAt(coordinates);
        HexScript script = hex.GetComponent<HexScript>();
        script.FlipDiceChip();
    }

    public void RepresentationChanged()
    {

        RedrawMap();
    }

    private void RedrawMap()
    {
        foreach (GameObject obj in this.hexagonGameObjects)
        {
            (int, int) indexes = obj.GetComponent<HexScript>().mapArrayIndexes;
            obj.GetComponent<HexScript>().SetTile(this.map.getRepresentation()[indexes.Item1, indexes.Item2]);
        }
    }

    public void OnSpacePointClicked(GameObject spacePointObject, SpacePoint point)
    {
        actualTokenInScene.GetComponent<Space.TokenScript>().MoveTo(point);
        Object.Destroy(spacePointObject);
        CreateButtonAtSpacePoint(map.getAllSpacePointsInDistance(point, 2)[0]);

        FlipDiceChipAtHex(new HexCoordinates(0, 0));

    }

    void CenterCamera()
    {
        Helper helper = new Helper();
        BoundingBox bbox = helper.GetLowestPoint(this.gameObject.transform);
        Debug.Log("Lowest X:" + bbox.minX);
        Debug.Log("Lowest Y:" + bbox.minY);
        Debug.Log("Highest Y:" + bbox.maxY);
        Debug.Log("Higest X:" + bbox.maxX);

        Vector2 topLeft = new Vector2(bbox.minX, bbox.maxY);
        Vector2 bottomRight = new Vector2(bbox.maxX, bbox.minY);

        Vector2 middle = (topLeft + bottomRight) / 2;

        Camera.main.transform.position = new Vector3(middle.x, middle.y, Camera.main.transform.position.z);
    }

    public void CreateButtonAtSpacePoint(SpacePoint point)
    {
        GameObject btn = (GameObject)Instantiate(spacePointButton, point.ToUnityPosition(), Quaternion.identity);
        btn.GetComponent<Space.SpacePointButtonScript>().spacePoint = point;
        btn.GetComponent<Space.SpacePointButtonScript>().referenceToInstance = btn;
        btn.transform.parent = this.gameObject.transform;

    }

    public void CreateTokenAtSpacePoint(SpacePoint point)
    {
        Vector2 xyInWorldCoords = point.ToUnityPosition();
        actualTokenInScene = Instantiate(tokenPrefab, new Vector3(xyInWorldCoords.x, xyInWorldCoords.y, Constants.TOKEN_LAYER), Quaternion.identity);
        actualTokenInScene.GetComponent<Space.TokenScript>().token = actualTokenInScene;
        actualTokenInScene.transform.parent = this.gameObject.transform;

    }

    public GameObject CreateHexagon(Vector2 position, HexCoordinates coords)
    {
        (int, int) mapArrayIndexes = map.coordsToArrayIndexes(coords);

        GameObject hexagon = Instantiate(hexPrefab, position, Quaternion.identity);
        hexagon.GetComponent<HexScript>().tile = this.map.getRepresentation()[mapArrayIndexes.Item1, mapArrayIndexes.Item2];
        hexagon.GetComponent<HexScript>().hexCoords = coords;
        hexagon.GetComponent<HexScript>().mapArrayIndexes = mapArrayIndexes;
        hexagon.transform.parent = this.gameObject.transform;
        return hexagon;
    }

    GameObject[] CreateMap(Map map)
    {

        HexCoordinates[] allValidHexCoords = map.getAllHexCoordinates(true);
        int len = allValidHexCoords.Length;


        List<GameObject> hexagons = new List<GameObject>();
        foreach (HexCoordinates hexCoords in allValidHexCoords)
        {
            GameObject hexagon = CreateHexagon(hexCoords.PointyToPixel(), hexCoords);
            hexagons.Add(hexagon);
        }

        return hexagons.ToArray();
    }

    public static void DrawCircleAroundGameObject(GameObject container, float radius, float lineWidth)
    {
        var segments = 360;
        var line = container.AddComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.positionCount = segments + 1;

        var pointCount = segments + 1; // add extra point to make startpoint and endpoint the same to close the circle
        var points = new Vector3[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            var rad = Mathf.Deg2Rad * (i * 360f / segments);
            points[i] = new Vector3(Mathf.Cos(rad) * radius, Mathf.Sin(rad) * radius, 0);
        }

        line.SetPositions(points);
    }

    void DrawCircleAtSpacePoints(SpacePoint[] points)
    {
        foreach( SpacePoint point in points)
        {
            DrawCircleAtSpacePoint(point);
        }
    }

    void DrawCircleAtSpacePoint(SpacePoint point)
    {
        Vector2 position = point.ToUnityPosition();
        GameObject spacePointObject = new GameObject("Space Point");
        spacePointObject.transform.position = position;

        DrawCircleAroundGameObject(spacePointObject, 0.2f, 0.05f);
        spacePointObject.transform.parent = this.gameObject.transform;

    }

    void AddCityToPosition()
    {
        //TODO
    }

    void AddShipToPosition()
    {
        //TODO
    }

    void MoveShipToPosition()
    {
        //TODO
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
