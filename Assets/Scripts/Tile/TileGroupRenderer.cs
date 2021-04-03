using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileGroupRenderer : SFController
{
    public GameObject mapObject;
    private Map map;
    public GameObject dockStationPrefab;
    bool alreadyDrawn = false;
    List<GameObject> dockButtons = new List<GameObject>();

    // Use this for initialization
    void Start()
    {
        // get all the tilegroups from map and render them according to their state
        // attach this script somewhere inside the map, it will automatically update the tilegroups
        
    }

    public void Initialize(Map map)
    {
        // register itself as observer of mapModel
        // mapModel.registerObserver(this)
        this.map = map;
        DrawTileGroups();
    }

    GameObject[] CreateTradeStationDocks(int numDocks, Transform transform)
    {
        List<GameObject> buttons = new List<GameObject>();
        for (int i = 0; i < numDocks; i++)
        {
            //TODO: use width of tile for calculation
            var rad = Mathf.Deg2Rad * (i * 360f / numDocks);
            Vector3 position = new Vector3(Mathf.Cos(rad) * 0.5f, Mathf.Sin(rad) * 0.5f, 0) + transform.position;
            GameObject btn = (GameObject)Instantiate(dockStationPrefab, position, Quaternion.identity);
            btn.transform.parent = transform;
            buttons.Add(btn);
        }
        return buttons.ToArray();

    }

    void DestroyAllDockButtons()
    {
        foreach (var dockBtn in dockButtons)
        {
            GameObject.Destroy(dockBtn);
        }
        this.dockButtons.Clear();
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
                var btns = CreateTradeStationDocks(tradeStation.GetCapacity(), hexGameObject.transform);
                dockButtons.AddRange(btns);

                for (int i = 0; i < tradeStation.dockedSpaceships.Count; i++)
                {
                    var token = tradeStation.dockedSpaceships[i];
                    var newPos = btns[i].transform.position;
                    token.SetPosition(new SpacePoint(new HexCoordinates(100, 100), 0)); //just point to really random point, garbage point
                    token.useOwnPositioningSystem = false;
                    token.SetUnityPosition(newPos);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //var mapScript = mapObject.GetComponent<MapScript>();
        //map = mapScript.map;
        //if (map.tileGroups != null && !alreadyDrawn)
        //{
        //    DrawTileGroups();
        //    alreadyDrawn = true;
        //}
    }

    public override void OnNotification(string p_event_path, Object p_target, params object[] p_data)
    {
        switch(p_event_path)
        {
            case SFNotification.map_data_changed:
                if (map != null)
                {
                    if (map.tileGroups != null)
                    {
                        //TODO: this is failing because we do some stuff to the tilegroups in the map creation which triggers the notification
                        // but we don't have the map created yet. Better way is to let the mapscript activate this class and register it as an
                        // observer of the mapModel
                        DestroyAllDockButtons();
                        DrawTileGroups();
                    }
                }
                break;
        }
    }
}
