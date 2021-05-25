using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.onebuckgames.UnityStarFarers;

public class TileGroupRenderer : SFController, Observer
{
    public GameObject mapObject;
    public GameObject dockStationPrefab;
    
    public Dictionary<SpacePoint, List<GameObject>> TradeStationDocks { get { return _tradeStationDocks; } }

    private Map map { get { return globalGamecontroller.mapModel; } }
    private Dictionary<SpacePoint, List<GameObject>> _tradeStationDocks;
    List<GameObject> dockButtons = new List<GameObject>();

    
    void Start()
    {
        _tradeStationDocks = new Dictionary<SpacePoint, List<GameObject>>();
        // get all the tilegroups from map and render them according to their state
        // attach this script somewhere inside the map, it will automatically update the tilegroups
    }

    public void Initialize()
    {
        DrawTileGroups();
    }

    public List<GameObject> CreateTradeStationDocks(TradeStation station, Transform transform)
    {
        List<GameObject> buttons = new List<GameObject>();
        var numDocks = station.GetCapacity();
        for (int i = 0; i < numDocks; i++)
        {
            //TODO: use width of tile for calculation
            var rad = Mathf.Deg2Rad * (i * 360f / numDocks);
            var position = new Vector3(Mathf.Cos(rad) * 0.5f, Mathf.Sin(rad) * 0.5f, 0) + transform.position;
            GameObject btn = (GameObject)Instantiate(dockStationPrefab, position, Quaternion.identity);
            btn.transform.parent = transform;
            buttons.Add(btn);
        }
        _tradeStationDocks.Add(station.GetCenter(), buttons);
        return buttons;

    }

    void DrawTileGroups()
    {
        var mapScript = mapObject.GetComponent<MapScript>();
        foreach (var group in map.tileGroups)
        {
            if (group is TradeStation)
            {
                var tradeStation = (TradeStation)group;
                var hexGameObject = mapScript.FindHexGameObjectWithTile(tradeStation.GetTiles()[0]);
                Debug.Log(string.Format("HexGameObjectEmpty: {0}", hexGameObject == null));
                if (hexGameObject == null)
                {
                    continue;
                }
                var btns = CreateTradeStationDocks(tradeStation, hexGameObject.transform);
                dockButtons.AddRange(btns);

                
            }
        }
    }

    void Update()
    {
        
    }

    public override void OnNotification(string p_event_path, Object p_target, params object[] p_data)
    {
        
    }

    public void SubjectDataChanged(Subject subject, object[] data)
    {
        
    }
}
