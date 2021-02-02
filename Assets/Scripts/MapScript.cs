using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants {
    public const float HEX_RADIUS = 1.0f;
    public const int TOKEN_LAYER = -2;
    public const float paddingRight = 1.0f;
    public const float paddingDown = 1.0f;
}

public abstract class TokenFilter
{
    public abstract bool fullfillsFilter(Token token);
    
}

public class IsSettledColonyFilter : TokenFilter
{
    public override bool fullfillsFilter(Token token)
    {
        if (token is ColonyBaseToken)
        {
            ColonyBaseToken cb = (ColonyBaseToken)token;
            return cb.isSettled;
        }
        return false;
    }
}

class TradeshipSpacePointFilter : SpacePointFilter
{
    public override bool pointFulfillsFilter(SpacePoint point, Map map, Player[] players)
    {
        Tile_[] tiles = map.getTilesAtPoint(point);
        var t = map.GetTilesOfType<ResourceTile>(tiles);
        return t.Length == 2;
    }
}

class IsSpacePointFreeFilter : SpacePointFilter
{
    public override bool pointFulfillsFilter(SpacePoint point, Map map, Player[] players)
    {
        foreach(Token token in GetAllTokenOfPlayers(players))
        {
            if (token.position.Equals(point))
            {
                return false;
            }
        }
        return true;
    }
}

class IsValidSpacePointFilter : SpacePointFilter
{
    public override bool pointFulfillsFilter(SpacePoint point, Map map, Player[] players)
    {
        var tiles = map.getTilesAtPoint(point);
        ResourceTile[] resourceTiles = map.GetTilesOfType<ResourceTile>(tiles);
        if (resourceTiles.Length == 3)
        {
            return false;
        } else
        {
            return true;
        }

    }
}

class IsNeighborOwnSpacePortFilter : SpacePointFilter
{
    public override bool pointFulfillsFilter(SpacePoint point, Map map, Player[] players)
    {
        SpacePoint[] neighborPoints = map.getAllSpacePointsInDistance(point, 1); //all neighbors
        foreach (SpacePoint neighbor in neighborPoints)
        {
            foreach (Token token in GetAllTokenOfPlayers(players))
            {
                if (token.position.Equals(neighbor))
                {
                    if (token.attachedToken is SpacePortToken)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}


public class MapScript : SFController
{

    public GameObject spacePointButton;
    public GameObject hexPrefab;
    public GameObject tokenPrefab;
    public GameObject tokenRendererPrefab;
    
    public Camera cam;
    public Player[] players;
    GameObject selectedTokenRenderer;
    Map map;
    List<GameObject> currentlyShownSpacePointButtons = new List<GameObject>();
    GameObject[] hexagonGameObjects;

    void Start()
    {
        

    }

    public void SetPlayers(Player[] players)
    {
        this.players = players;
        DisplayPlayerTokens();
    }

    public void SetMap(Map map)
    {
        this.map = map;
        hexagonGameObjects = CreateMap(map);
        CenterCamera();
    }

    void DisplayPlayerTokens()
    {
        var allTokens = new Helper().GetAllTokenOfPlayers(players);
        foreach(Token token in allTokens)
        {
            DisplayToken(token);
        }
    }

    

    public void DisplayToken(Token token) {
        GameObject prefab = tokenRendererPrefab;
        GameObject tokenInstance = Instantiate(prefab, new Vector3(0,0,0), Quaternion.identity);
        tokenInstance.GetComponent<Space.TokenScript>().tokenGameObject = tokenInstance;
        tokenInstance.GetComponent<Space.TokenScript>().tokenModel = token;
        tokenInstance.GetComponent<Space.TokenScript>().Draw();

        tokenInstance.transform.parent = this.gameObject.transform;
    }

    

    public void ShowAllAvailableSpacePoints()
    {
        SpacePoint[] allSpacePoints = map.getAllAvailableSpacePoints();
        CreateButtonsAtSpacePoints(allSpacePoints);
    }

    void CreateButtonsAtSpacePoints(SpacePoint[] points)
    {
        foreach (SpacePoint point in points)
        {
            CreateButtonAtSpacePoint(point);
        }
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

        //RedrawMap();
    }

    public void RemoveAllSpacePointButtons()
    {
        foreach (GameObject go in currentlyShownSpacePointButtons)
        {
            GameObject.Destroy(go);
        }

        currentlyShownSpacePointButtons.Clear();
    }

    private void RedrawMap()
    {
        foreach (GameObject obj in this.hexagonGameObjects)
        {
            (int, int) indexes = obj.GetComponent<HexScript>().mapArrayIndexes;
            obj.GetComponent<HexScript>().SetTile(this.map.getRepresentation()[indexes.Item1, indexes.Item2]);
        }
    }

    //public void OnSpacePointClicked(GameObject spacePointObject, SpacePoint point)
    //{
    //    //actualTokenInScene.GetComponent<Space.TokenScript>().MoveTo(point);
    //    //Object.Destroy(spacePointObject);
    //    //CreateButtonAtSpacePoint(map.getAllSpacePointsInDistance(point, 2)[0]);

    //    //FlipDiceChipAtHex(new HexCoordinates(0, 0));
    //    RemoveAllSpacePointButtons();

    //    selectedTokenRenderer.GetComponent<Space.TokenScript>().MoveTo(point);

    //}

    void CenterCamera()
    {
        Helper helper = new Helper();
        BoundingBox bbox = helper.GetLowestPoint(this.gameObject.transform);
        Logger.log("Lowest X:" + bbox.minX);
        Logger.log("Lowest Y:" + bbox.minY);
        Logger.log("Highest Y:" + bbox.maxY);
        Logger.log("Higest X:" + bbox.maxX);

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

        btn.GetComponentInChildren<bla>().point = point;
        currentlyShownSpacePointButtons.Add(btn);

        btn.transform.parent = this.gameObject.transform;

    }

    public GameObject CreateHexagon(Vector2 position, HexCoordinates coords)
    {
        (int, int) mapArrayIndexes = map.coordsToArrayIndexes(coords);

        GameObject hexagon = Instantiate(hexPrefab, position, Quaternion.identity);
        hexagon.GetComponent<HexScript>().tile = this.map.getRepresentation()[mapArrayIndexes.Item1, mapArrayIndexes.Item2];
        hexagon.GetComponent<HexScript>().hexCoords = coords;
        hexagon.GetComponent<HexScript>().mapArrayIndexes = mapArrayIndexes;
        hexagon.transform.parent = this.gameObject.transform;
        hexagon.GetComponent<HexScript>().Draw();
        return hexagon;
    }

    GameObject[] CreateMap(Map map)
    {

        HexCoordinates[] allValidHexCoords = map.getAllHexCoordinates(true);

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

    public void HighlightToken(Token token)
    {
        DrawCircleAtSpacePoint(token.position);
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

        DrawCircleAroundGameObject(spacePointObject, 1.0f, 0.05f);
        spacePointObject.transform.parent = this.gameObject.transform;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HighlightTokensFullfillingFilters(TokenFilter[] filters)
    {
        Token[] tokens = GetTokensFullfillingFilters(filters);
        foreach(Token tok in tokens)
        {
            DrawCircleAtSpacePoint(tok.position);
        }
    }

    public Token[] GetTokensFullfillingFilters(TokenFilter[] filters)
    {
        Token[] tokens = GetAllTokenOnBoard();
        foreach (var filter in filters)
        {
            tokens = FilterToken(tokens, filter);
        }
        return tokens;

    }

    Token[] FilterToken(Token[] inputToken, TokenFilter filter)
    {
        List<Token> valid = new List<Token>();
        foreach (Token token in inputToken)
        {
            if (filter.fullfillsFilter(token))
            {
                valid.Add(token);
            }
        }
        return valid.ToArray();
    }

    Token[] GetAllTokenOnBoard()
    {
        List<Token> allTokens = new List<Token>();
        foreach (Player player in players)
        {
            foreach(Token token in player.tokens)
            {
                allTokens.Add(token);
            }
        }

        return allTokens.ToArray();
    }

    public void ShowSpacePointsFulfillingFilters(SpacePointFilter[] filters)
    {
        SpacePoint[] points = map.getAllAvailableSpacePoints();
        foreach(var filter in filters)
        {
            points = map.applyFilter(points, filter, players);
        }
        CreateButtonsAtSpacePoints(points);
    }

    void ShowSpacePointsForColonyship()
    {
        ShowAllAvailableSpacePoints();
    }

    void ShowSpacePointsForSpaceport()
    {
        ShowAllAvailableSpacePoints();
    }

    public override void OnNotification(string p_event_path, Object p_target, params object[] p_data)
    {
        switch (p_event_path)
        {
            //case SFNotification.HUD_build_colonyship_button_clicked:
            //    ShowSpacePointsForColonyship();
            //    break;
            //case SFNotification.HUD_build_tradeship_button_clicked:
            //    ShowSpacePointsForTradeship();
            //    break;
            //case SFNotification.HUD_build_spaceport_button_clicked:
            //    ShowSpacePointsForSpaceport();
            //    break;

            //case SFNotification.token_was_selected:
            //    selectedTokenRenderer = (GameObject)p_target;
            //    ShowAllAvailableSpacePoints();
            //    break;
        }
    }
}
