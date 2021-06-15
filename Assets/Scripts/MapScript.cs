using System.Collections;
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

public class MapScript : SFController
{

    public GameObject spacePointButton;
    public GameObject hexPrefab;
    public GameObject tokenPrefab;
    public GameObject tokenRendererPrefab;
    public GameObject TileGroupRenderer;

    public Camera cam;
    public Map map { get { return globalGamecontroller.mapModel; } }
    private MapMode mode = MapMode.NORMAL;
    private System.Action<Token> tokenSelectCallback;
    private System.Action<SpacePoint> didSelectSpacePointCallback;
    public bool isInteractive = true;

    List<GameObject> allSpacePointButtons = new List<GameObject>();
    List<GameObject> hexagonGameObjects;
    List<GameObject> currentlyDisplayedPlayerTokens;
    List<string> highlightedTokensGuids = new List<string>();
    List<GameObject> highlightCircles = new List<GameObject>();

    void Start()
    {
        

    }

    public void ActivateAllInteraction(bool isInteractive)
    {
        this.isInteractive = isInteractive;
    }

    public List<string> GetHighlightedTokens()
    {
        return highlightedTokensGuids;
    }

    public List<GameObject> GetAllTokenObjects()
    {
        return currentlyDisplayedPlayerTokens;
    }

    public List<GameObject> GetAllShownSpacePointButtons()
    {
        return allSpacePointButtons.Where(btn => btn.activeInHierarchy).ToList();
    }

    public void Init()
    {
        hexagonGameObjects = CreateMap(map);
        currentlyDisplayedPlayerTokens = DisplayPlayerTokens();
        TileGroupRenderer.GetComponent<TileGroupRenderer>().Initialize();
        CenterCamera();
        CreateAllSpacePointButtons();
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

        GameObject tokenInstance = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
        tokenInstance.GetComponent<Space.TokenScript>().tokenModel = token;
        tokenInstance.GetComponent<Space.TokenScript>().Draw();

        var tuple = GetTradestationWhichThisTokenIsDocked(token);
        if (tuple != null)
        {
            tokenInstance.GetComponent<Space.TokenScript>().tokenInstance.transform.position = GetTradeStationDockPosition(tuple.Item2, tuple.Item1);
        }
        tokenInstance.transform.parent = this.gameObject.transform;
        return tokenInstance;
    }


    Vector3 GetTradeStationDockPosition (TradeStation station, int index)
    {
        var tradeStationDocks = TileGroupRenderer.GetComponent<TileGroupRenderer>().TradeStationDocks;
        if (tradeStationDocks == null)
        {
            throw new System.ArgumentException("bla");
        }

        if (!tradeStationDocks.ContainsKey(station.GetCenter()))
        {
            throw new System.ArgumentException("no tradestation of that kind");
        }

        return tradeStationDocks[station.GetCenter()][index].transform.position;
    }

    System.Tuple<int, TradeStation> GetTradestationWhichThisTokenIsDocked(Token token)
    {
        foreach(var tradestation in map.GetTradeStations())
        {
            var index = tradestation.dockedTokens.FindIndex(tok => tok == token);
            if (index != -1)
            {
                return new System.Tuple<int, TradeStation>(index, tradestation);
            } 
        }
        return null;
    }

    void CreateAllSpacePointButtons()
    {
        var points = map.AllAvailableSpacePoints;
        foreach (SpacePoint point in points)
        {
            GameObject btn = (GameObject)Instantiate(spacePointButton, point.ToUnityPosition(), Quaternion.identity);
            btn.GetComponent<Space.SpacePointButtonScript>().spacePoint = point;
            btn.GetComponent<Space.SpacePointButtonScript>().referenceToInstance = btn;
            btn.GetComponentInChildren<bla>().point = point;
            allSpacePointButtons.Add(btn);
            btn.transform.parent = this.gameObject.transform;
            btn.SetActive(false);
        }
    }

    public void HideAllSpacePointButtons()
    {
        foreach (GameObject go in allSpacePointButtons)
        {
            go.SetActive(false);
        }
    }

    public void RedrawMap()
    {
        Debug.Log("Th1 - Redrawing Map!");
        RedrawTokens();
        foreach (GameObject obj in this.hexagonGameObjects)
        {
            (int, int) indexes = obj.GetComponent<HexScript>().mapArrayIndexes;
            obj.GetComponent<HexScript>().SetTile(this.map.getRepresentation()[indexes.Item1, indexes.Item2]);
        }
    }

    public GameObject FindHexGameObjectWithTile(Tile_ tile)
    {
        return hexagonGameObjects.Find(hex => hex.GetComponent<HexScript>().tile == tile);
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

    List<GameObject> CreateMap(Map map)
    {
        HexCoordinates[] allValidHexCoords = map.getAllHexCoordinates(true);

        List<GameObject> hexagons = new List<GameObject>();
        foreach (HexCoordinates hexCoords in allValidHexCoords)
        {
            GameObject hexagon = CreateHexagon(hexCoords.PointyToPixel(), hexCoords);
            hexagons.Add(hexagon);
        }

        return hexagons;
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
        tokenSelectCallback = null;
        RemoveAllHighlights();
    }

    public void RemoveAllHighlights()
    {
        highlightedTokensGuids = new List<string>();
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
        if (map.tokensOnMap.Contains(token))
        {
            highlightedTokensGuids.Add(token.guid.ToString());
            var circleHighlight = DrawCircleAtSpacePoint(token.position);
            highlightCircles.Add(circleHighlight);
        } else
        {
            throw new System.ArgumentException("trying to highlight token that is not part of the map model!");
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
        Debug.Log("TOken was clicked");
        if (mode == MapMode.SELECT)
        {
            if (highlightedTokensGuids.Contains(token.guid.ToString()))
            {
                this.tokenSelectCallback?.Invoke(token);
                CloseTokenSelection();
            }
            else
            {
                Debug.Log("CLicked token not part of highlightedTokens");
            }
        }
    }

    public void SettleToken(Token token)
    {
        map.SettleToken(token);
        OnMapDataChanged();
        
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

    public void CloseSpacePointSelection()
    {
        HideAllSpacePointButtons();
        this.didSelectSpacePointCallback = null;
    }

    public void ShowSpacePointsFulfillingFilters(List<SpacePointFilter> filters)
    {
        List<SpacePoint> points = map.GetSpacePointsFullfillingFilters(filters);
        ShowSpacePoints(points);
    }

    public void ShowSpacePoints(List<SpacePoint> spacePoints)
    {
        foreach (var point in spacePoints)
        {
            var associatedButton = allSpacePointButtons.Find(btn => btn.GetComponent<Space.SpacePointButtonScript>().spacePoint.Equals(point));
            if (associatedButton != null)
            {
                associatedButton.SetActive(true);
            }
            else
            {
                Logger.LogError("Couldnt find button with spacepoint");
            }
        }
    }

    public override void OnNotification(string p_event_path, Object p_target, params object[] p_data)
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

    public void OnMapDataChanged()
    {
        RedrawMap();
    }
}
