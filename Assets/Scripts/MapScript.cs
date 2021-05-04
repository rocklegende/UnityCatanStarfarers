﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using com.onebuckgames.UnityStarFarers;

public class Constants
{
    public const float HEX_RADIUS = 1.0f;
    public const int TOKEN_LAYER = -2;
    public const float paddingRight = 1.0f;
    public const float paddingDown = 1.0f;
}

public enum MapMode
{
    NORMAL,
    SELECT
}

public class MapScript : SFController, Observer
{

    public GameObject spacePointButton;
    public GameObject hexPrefab;
    public GameObject tokenPrefab;
    public GameObject tokenRendererPrefab;
    public GameObject TileGroupRenderer;

    public Camera cam;
    public bool isReceivingNotifications = false;
    public Map map;
    private MapMode mode = MapMode.NORMAL;
    private System.Action<Token> tokenSelectCallback;
    private System.Action<SpacePoint> didSelectSpacePointCallback;

    List<GameObject> currentlyShownSpacePointButtons = new List<GameObject>();
    GameObject[] hexagonGameObjects;
    List<GameObject> currentlyDisplayedPlayerTokens;
    List<Token> highlightedTokens = new List<Token>();
    List<GameObject> highlightCircles = new List<GameObject>();

    void Start()
    {

        
    }

    public List<GameObject> GetAllTokenObjects()
    {
        return currentlyDisplayedPlayerTokens;
    }

    public List<GameObject> GetAllShownSpacePointButtons()
    {
        return currentlyShownSpacePointButtons;
    }

    public void SetMap(Map map)
    {
        this.map = map;
        map.RegisterObserver(this);
        hexagonGameObjects = CreateMap(map);
        TileGroupRenderer.GetComponent<TileGroupRenderer>().Initialize(map);
        currentlyDisplayedPlayerTokens = DisplayPlayerTokens();
        CenterCamera();
    }

    void RedrawTokens()
    {
        if (currentlyDisplayedPlayerTokens != null)
        {
            foreach (GameObject go in currentlyDisplayedPlayerTokens)
            {
                Destroy(go);
            }
        }

        currentlyDisplayedPlayerTokens = DisplayPlayerTokens();
    }

    List<GameObject> DisplayPlayerTokens()
    {
        List<GameObject> tokens = new List<GameObject>();
        foreach (Token token in map.tokensOnMap)
        {
            var go = DisplayToken(token);
            tokens.Add(go);
        }

        return tokens;
    }



    public GameObject DisplayToken(Token token)
    {
        GameObject prefab = tokenRendererPrefab;

        //GameObject tokenInstance = Helper.Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
        GameObject tokenInstance = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
        DontDestroyOnLoad(tokenInstance);
        tokenInstance.GetComponent<Space.TokenScript>().tokenGameObject = tokenInstance;
        tokenInstance.GetComponent<Space.TokenScript>().tokenModel = token;
        tokenInstance.GetComponent<Space.TokenScript>().Draw();

        tokenInstance.transform.parent = this.gameObject.transform;
        return tokenInstance;
    }



    public void ShowAllAvailableSpacePoints()
    {
        var allSpacePoints = map.getAllAvailableSpacePoints();
        CreateButtonsAtSpacePoints(allSpacePoints);
    }

    void CreateButtonsAtSpacePoints(List<SpacePoint> points)
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

    public void HideAllSpacePointButtons()
    {
        foreach (GameObject go in currentlyShownSpacePointButtons)
        {
            GameObject.Destroy(go);
        }

        currentlyShownSpacePointButtons.Clear();
    }

    public void RedrawMap()
    {
        RedrawTokens();
        foreach (GameObject obj in this.hexagonGameObjects)
        {
            (int, int) indexes = obj.GetComponent<HexScript>().mapArrayIndexes;
            obj.GetComponent<HexScript>().SetTile(this.map.getRepresentation()[indexes.Item1, indexes.Item2]);
        }
    }

    public GameObject FindHexGameObjectWithTile(Tile_ tile)
    {
        foreach (var gameObject in hexagonGameObjects)
        {
            if (gameObject.GetComponent<HexScript>().tile == tile)
            {
                return gameObject;
            }
        }
        return null;
    }

    void CenterCamera()
    {
        Helper helper = new Helper();
        BoundingBox bbox = helper.GetLowestPoint(this.gameObject.transform);

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

    public void OpenTokenSelection(List<Token> selectableToken, System.Action<Token> didSelectTokenCallback)
    {
        mode = MapMode.SELECT;
        HighlightTokens(selectableToken);

        tokenSelectCallback = didSelectTokenCallback;
        
    }

    public void CloseTokenSelection()
    {
        mode = MapMode.NORMAL;
        RemoveAllHighlights();
    }

    public void RemoveAllHighlights()
    {
        highlightedTokens = new List<Token>();
        foreach(var obj in highlightCircles)
        {
            Destroy(obj);
        }
    }

    public void HighlightTokens(List<Token> tokens)
    {
        foreach(var tok in tokens)
        {
            HighlightToken(tok);
        }
    }

    void HighlightToken(Token token)
    {
        highlightedTokens.Add(token);
        var circleHighlight = DrawCircleAtSpacePoint(token.position);
        highlightCircles.Add(circleHighlight);
    }

    void DrawCircleAtSpacePoints(List<SpacePoint> points)
    {
        foreach (SpacePoint point in points)
        {
            DrawCircleAtSpacePoint(point);
        }
    }

    GameObject DrawCircleAtSpacePoint(SpacePoint point)
    {
        Vector3 position = new Vector3(point.ToUnityPosition().x, point.ToUnityPosition().y, -1);
        GameObject spacePointObject = new GameObject("Space Point Highlight Circle");
        spacePointObject.transform.position = position;

        DrawCircleAroundGameObject(spacePointObject, 1.0f, 0.05f);
        spacePointObject.transform.parent = this.gameObject.transform;

        return spacePointObject;

    }

    public void HighlightTokensFullfillingFilters(List<TokenFilter> filters)
    {
        List<Token> tokens = GetTokensFullfillingFilters(filters);
        HighlightTokens(tokens);
    }

    public List<Token> GetTokensFullfillingFilters(List<TokenFilter> filters)
    {
        var tokens = Helper.CreateCopyOfList(map.tokensOnMap);
        foreach (var filter in filters)
        {
            tokens = FilterToken(tokens, filter);
        }
        return tokens;

    }

    List<Token> FilterToken(List<Token> inputTokens, TokenFilter filter)
    {
        return inputTokens.Where(tok => filter.fullfillsFilter(tok)).ToList();
    }

    public void HandleClickOfToken(Token token)
    {
        if (mode == MapMode.SELECT)
        {
            if (highlightedTokens.Contains(token))
            {
                this.tokenSelectCallback(token);
                RemoveAllHighlights();
                mode = MapMode.NORMAL;
            }
        }
    }

    public void SettleToken(Token token)
    {
        map.SettleToken(token);
        
    }

    public void OpenSpacePointSelection(List<SpacePoint> spacePointsToChooseFrom, System.Action<SpacePoint> didSelectSpacePoint)
    {
        if (spacePointsToChooseFrom.Count < 1)
        {
            //immediately make callback call if we have no points to choose from
            didSelectSpacePoint(null);
        }
        ShowSpacePoints(spacePointsToChooseFrom);
        this.didSelectSpacePointCallback = didSelectSpacePoint;
    }

    public void ShowSpacePointsFulfillingFilters(List<SpacePointFilter> filters, List<Player> players)
    {
        List<SpacePoint> points = map.GetSpacePointsFullfillingFilters(filters, players.ToArray());
        CreateButtonsAtSpacePoints(points);
    }

    public void ShowSpacePoints(List<SpacePoint> spacePoints)
    {
        CreateButtonsAtSpacePoints(spacePoints);
    }

    public override void OnNotification(string p_event_path, Object p_target, params object[] p_data)
    {
        if (isReceivingNotifications)
        {
            switch (p_event_path)
            {                
                case SFNotification.token_was_selected:
                    HandleClickOfToken((Token)p_data[0]);
                    break;

                case SFNotification.spacepoint_selected:
                    didSelectSpacePointCallback?.Invoke((SpacePoint)p_data[0]);
                    HideAllSpacePointButtons();
                    break;
            }
        }

    }

    public void SubjectDataChanged(object[] data)
    {
        //map data changed
        Debug.Log("MapScript reacting to map data change");
        RedrawMap();
    }
}
