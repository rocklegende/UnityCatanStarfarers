using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants {
    public const float HEX_RADIUS = 1.0f;
    public const float paddingRight = 1.0f;
    public const float paddingDown = 1.0f;
}




public class MapScript : MonoBehaviour
{

    public GameObject spacePointButton;
    public GameObject hexPrefab;
    public Camera cam;

    void Start()
    {
        Tile[,] mapRepresentation = {
            { Tile.NULL, Tile.NULL, Tile.VALID, Tile.VALID, Tile.BORDER },
            { Tile.NULL, Tile.NULL, Tile.VALID, Tile.VALID, Tile.BORDER },
            { Tile.NULL, Tile.VALID, Tile.VALID, Tile.BORDER, Tile.NULL },
            { Tile.NULL, Tile.VALID, Tile.VALID, Tile.BORDER, Tile.NULL },
            { Tile.VALID, Tile.VALID, Tile.BORDER, Tile.BORDER, Tile.NULL },
        };

        Map map = new Map(mapRepresentation);
        DrawMap(map);

        SpacePoint point = new SpacePoint(new HexCoordinates(0, 1), 0);
        SpacePoint[] pointsOneStepAway = map.getAllSpacePointsInDistance(point, 0);

        CreateButtonAtSpacePoint(pointsOneStepAway[0]);

        DrawCircleAtSpacePoints(pointsOneStepAway);


        // test getAllAvailableCityPositions
        //SpacePoint[] allPositions = map.getAllAvailableSpacePoints();
        //foreach (SpacePoint pos in allPositions)
        //{
        //    Debug.Log("q: " + pos.coordinates.q + ", r: " + pos.coordinates.r + ", v: " + pos.vertexNumber);
        //}

        CenterCamera();

    }

    void CenterCamera()
    {
        Helper helper = new Helper();
        //Debug.Log("Lowest Y:" + helper.GetLowestPoint(this.gameObject.transform).minY);
        BoundingBox bbox = helper.GetLowestPoint(this.gameObject.transform);
        Debug.Log("Lowest X:" + bbox.minX);
        Debug.Log("Lowest Y:" + bbox.minY);
        Debug.Log("Highest Y:" + bbox.maxY);
        Debug.Log("Higest X:" + bbox.maxX);

        Vector2 topLeft = new Vector2(bbox.minX, bbox.maxY);
        Vector2 bottomRight = new Vector2(bbox.maxX, bbox.minY);

        Vector2 middle = (topLeft + bottomRight) / 2;
        Debug.Log(middle);

        Camera.main.transform.position = new Vector3(middle.x, middle.y, Camera.main.transform.position.z);
    }

    public void CreateButtonAtSpacePoint(SpacePoint point)
    {
        GameObject canvas = GameObject.Find("Canvas");
        GameObject btn = Instantiate(spacePointButton, new Vector3(0,0,0), Quaternion.identity);
        btn.transform.parent = canvas.transform;
        RectTransform rt = btn.GetComponent<RectTransform>();

        rt.transform.position = point.ToUnityPosition();
        Vector3 bla = rt.transform.position;

        Debug.Log(cam.WorldToScreenPoint(point.ToUnityPosition()));
        
    }

    public void DrawHexagon(Vector2 position)
    {
        GameObject hexagon = Instantiate(hexPrefab, position, Quaternion.identity);




        //Helper helper = new Helper();
        //GameObject hexagon = new GameObject("Hex");
        //helper.AddHexagonRenderer(hexagon, position);
        hexagon.transform.parent = this.gameObject.transform;
    }

    void DrawMap(Map map)
    {

        HexCoordinates[] allValidHexCoords = map.getAllHexCoordinates(true);


        foreach (HexCoordinates hexCoords in allValidHexCoords)
        {
            Debug.Log("Drawing Hex with q: " + hexCoords.q + ", r: " + hexCoords.r);
            Debug.Log(hexCoords.PointyToPixel());
            DrawHexagon(hexCoords.PointyToPixel());
        }

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

    void AddResourceTileAtPosition(Vector2 position)
    {
        GameObject resourceTile = new GameObject("Resource Tile");

        SpriteRenderer renderer = resourceTile.AddComponent<SpriteRenderer>();

        Texture2D texture = Resources.Load("resource") as Texture2D;
        Debug.Log(texture);
        Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, 150.0f, 150.0f), new Vector2(0.0f, 0.0f));
        renderer.sprite = sprite;
        resourceTile.transform.position = position;
        resourceTile.transform.parent = this.gameObject.transform;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
