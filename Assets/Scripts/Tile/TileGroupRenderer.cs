using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.onebuckgames.UnityStarFarers;

public class TileGroupRenderer : SFController, Observer
{
    public GameObject mapObject;
    public GameObject dockStationPrefab;

    private Map map { get { return globalGamecontroller.mapModel; } }

    List<GameObject> dockButtons = new List<GameObject>();

    
    void Start()
    {
        // get all the tilegroups from map and render them according to their state
        // attach this script somewhere inside the map, it will automatically update the tilegroups
    }

    public void Initialize()
    {
        DrawTileGroups();
    }

    public void OnMapDataChanged()
    {
        Debug.Log("Changing tile group look!");
        DestroyAllDockButtons();
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
                Debug.Log(string.Format("HexGameObjectEmpty: {0}", hexGameObject == null));
                if (hexGameObject == null)
                {
                    continue;
                }
                var btns = CreateTradeStationDocks(tradeStation.GetCapacity(), hexGameObject.transform);
                dockButtons.AddRange(btns);

                for (int i = 0; i < tradeStation.dockedSpaceships.Count; i++)
                {
                    var token = tradeStation.dockedSpaceships[i];
                    var newPos = btns[i].transform.position;
                    token.SetPosition(SpacePoint.GarbagePoint);
                    token.useOwnPositioningSystem = false;
                    token.SetUnityPosition(newPos);
                }
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
        if (map != null)
        {
            if (map.tileGroups != null)
            {
                //DestroyAllDockButtons();
                //DrawTileGroups();
            }
        }
    }
}
