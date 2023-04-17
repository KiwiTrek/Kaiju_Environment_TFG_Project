using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Filter
{
    none = 0,
    player,
    session
}
public class HeatmapGenerator : MonoBehaviour
{
    //Range of IDs to take from
    [HideInInspector]
    public int maxIds;
    //Amount of IDs to store
    public bool getAllEvents;
    public int totalIdsToStore;
    int iterator = 0;

    // Text UI
    [Header("Scene Elements")]
    public TMPro.TMP_Text textInfo;
    public TMPro.TMP_Text textSelected;

    // Lists needed
    [HideInInspector] public List<int> numbersChosen = new List<int>();
    public List<HeatmapData> heatmapDatas = new List<HeatmapData>();
    public List<HeatmapData> pathData = new List<HeatmapData>();
    [HideInInspector] public List<GameObject> arrows = new List<GameObject>();
    public List<CubeClass> cubesList = new List<CubeClass>();

    public string url = "https://citmalumnes.upc.es/~guillemab1/";
    [HideInInspector] public string sUrl = "GetEvent.php";
    [HideInInspector] public string nUrl = "GetNumberEvents.php";

    public static Action<int> OnGetEvent;
    public GameObject cubePrefab;
    public GameObject arrowPrefab;

    // Map size
    [Header("Map Measurements")]
    public Vector2 topLeft = new Vector2(-60, 70);
    public int bottomLevel = -3;
    public int topLevel = 9;
    public int mapWidth = 190;
    public int mapLength = 140;

    Dictionary<eventType, int> maxEvents = new Dictionary<eventType, int>();

    [Header("Heatmap Settings")]
    [Range(1, 10)]
    public int granularity;

    // To change the color palette of the cubes
    public Gradient cubeColors;
    public Filter filter;
    [Range(0,255)]
    public int playerId;
    public int sessionId;

    public class CubeClass
    {
        public GameObject classCubePrefab;
        public Vector3 originPosition = new Vector3();
        public int size;
        public Vector3 colorValues;
        public Color color;
        public float value;
        public Dictionary <eventType, int> nEvents = new Dictionary<eventType, int>();

        public GameObject instance = null;

        public CubeClass()
        {  
            nEvents.Add(eventType.movement, 0);
            nEvents.Add(eventType.attack, 0);
            nEvents.Add(eventType.jump, 0);
            nEvents.Add(eventType.hitEnemy,0);
            nEvents.Add(eventType.killEnemy,0);
            nEvents.Add(eventType.recieveDamage,0);
            nEvents.Add(eventType.death,0);
        }
        public void InstantiateCube()
        {
            instance = Instantiate(classCubePrefab, originPosition, Quaternion.identity);
            instance.transform.localScale = new Vector3(size, size, size);
        }

        public void DestroyCube()
        {
            if (instance != null)
            {
                Destroy(instance);
                instance = null;
            }
        }
    }

    // bools
    bool updatePartOne = true;
    bool updatePartTwo = false;
    [HideInInspector]public bool canUpdate = false;

    private void OnEnable()
    {
        OnGetEvent += GetEvent;
    }
    private void OnDisable()
    {
        OnGetEvent -= GetEvent;
    }

    public void GetEvent(int id)
    {
        StartCoroutine(PHP2Event(id));
    }

    IEnumerator PHP2Event(int u)
    {
        string dataUrl = url + sUrl + "?eventId=" + u;
        //Debug.Log(dataUrl);
        WWW www = new WWW(dataUrl);

        yield return www;

        if (www.error == null)
        {
            //Debug.Log(www.text);
            string[] tmp = www.text.Split('>');

            HeatmapData tempHeatMap = new HeatmapData();
            tempHeatMap.dateTime = DateTime.Parse(tmp[0]);
            int tempInt = int.Parse(tmp[1]);
            tempHeatMap.type = (eventType)tempInt;
            tempHeatMap.playerId = uint.Parse(tmp[2]);
            tempHeatMap.sessionId = uint.Parse(tmp[3]);

            float x = float.Parse(tmp[4]);
            float y = float.Parse(tmp[5]);
            float z = float.Parse(tmp[6]);

            tempHeatMap.position.x = x;
            tempHeatMap.position.y = y;
            tempHeatMap.position.z = z;

            heatmapDatas.Add(tempHeatMap);
            iterator++;
            if (iterator != numbersChosen.Count)
            {
                StartCoroutine(PHP2Event(numbersChosen[iterator]));
            }
        }
        else
        {
            Debug.LogError("Error: " + www.error);
        }
    }

    int GetNumberEvents()
    {
        string dataUrl = url + nUrl;
        Debug.Log(dataUrl);
        WWW www = new WWW(dataUrl);

        while (!www.isDone) { }

        if (www.error == null)
        {
            Debug.Log(www.text);
            return int.Parse(www.text);
        }
        else
        {
            Debug.LogError("Error: " + www.error);
        }

        Debug.Log("Session has ended...");
        return 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        maxEvents.Add(eventType.movement, 0);
        maxEvents.Add(eventType.attack, 0);
        maxEvents.Add(eventType.jump, 0);
        maxEvents.Add(eventType.hitEnemy, 0);
        maxEvents.Add(eventType.killEnemy, 0);
        maxEvents.Add(eventType.recieveDamage, 0);
        maxEvents.Add(eventType.death, 0);

        maxIds = GetNumberEvents();

		if (totalIdsToStore > maxIds)
        {
            Debug.LogError("Error! Too many ids to store. Please choose a lower number.");
            textInfo.text = "Error! Too many ids to store. Please choose a lower number.";
        }
        else
        {
            DownloadData();
        }
    }
    public void DownloadData()
    {
        DeleteInstantiatedArrows();
        DeleteInstantiatedCubes();
        textSelected.text = "";
        heatmapDatas.Clear();
        numbersChosen.Clear();
        canUpdate = false;
        updatePartOne = true;
        updatePartTwo = false;
        iterator = 0;
        if (granularity < 1)
        {
            granularity = 1;
        }

        Debug.Log("Getting events...");
        textInfo.text = "Downloading data...";
        if (getAllEvents)
        {
            totalIdsToStore = maxIds - 1;
        }

        while (numbersChosen.Count != totalIdsToStore)
        {
            bool isDupe = false;

            int luckyNumber = UnityEngine.Random.Range(1, maxIds);

            foreach (int number in numbersChosen)
            {
                if (number == luckyNumber) { isDupe = true; }
            }

            if (!isDupe)
            {
                numbersChosen.Add(luckyNumber);
            }
        }
        Debug.Log("Total count: " + numbersChosen.Count);
        OnGetEvent?.Invoke(numbersChosen[0]);
    }

    // Update is called once per frame
    void Update()
    {
        if (heatmapDatas.Count >= totalIdsToStore)
        {
            if (!canUpdate)
			{
                if (updatePartTwo && !updatePartOne)
                {
                    textInfo.text = "Grid created! Generating map...";
                    Debug.Log("Grid created! Generating map...");
                    CreateMap();
                    if (filter == Filter.session)
                    {
                        pathData.Clear();
                        pathData = CreatePath(sessionId);
                    }
                    updatePartTwo = false;
                }

                if (updatePartOne && !updatePartTwo)
                {
                    updatePartOne = false;
                    textInfo.text = "Data downloaded! Creating grid...";
                    Debug.Log("Data downloaded! Creating grid...");
                    GenerateCubesGrid(granularity);
                    updatePartTwo = true;
                }

                if (!updatePartOne && !updatePartTwo)
                {
                    textInfo.text = "Press 1 - 7 to select heatmap!";
                    if (filter == Filter.session)
                    {
                        textInfo.text += " Press 8 to show path from session!";
                    }
                    Debug.Log("Maps generated!");
                    canUpdate = true;
                }
            }
        }

        if (canUpdate)
        {
            //Data Controls     
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ShowMap(eventType.movement);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ShowMap(eventType.attack);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                ShowMap(eventType.jump);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                ShowMap(eventType.hitEnemy);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                ShowMap(eventType.killEnemy);
            }
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                ShowMap(eventType.recieveDamage);
            }
            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                ShowMap(eventType.death);
            }
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                ShowPath();
            }
        }
    }

    private void ShowMap(eventType type)
    {
        canUpdate = false;
        textSelected.text = "Creating map: " + type.ToString();
        DeleteInstantiatedArrows();
        DeleteInstantiatedCubes();

        foreach (CubeClass cube in cubesList)
        {
            if (cube.nEvents[type] > 0) // Only instantiate cubes with meaningful information
            {
                cube.InstantiateCube();
                cube.value = (float)cube.nEvents[type] / (float)maxEvents[type];
                cube.color = cubeColors.Evaluate(cube.value);
                cube.color.a = 0.8f;
                cube.instance.GetComponent<Renderer>().material.color = cube.color;
            }
        }

        textSelected.text = "Heatmap selected: " + type.ToString();
        canUpdate = true;
    }

    private void ShowPath()
    {
        DeleteInstantiatedArrows();
        DeleteInstantiatedCubes();
        textSelected.text = "Showing player path";
        for (int i = 0; i < (pathData.Count - 1); i++)
        {
            GameObject arrow = Instantiate(arrowPrefab, pathData[i].position, Quaternion.identity);
            arrow.transform.LookAt(pathData[i + 1].position);
            float value = (float)i / (float)pathData.Count;
            if (arrow.GetComponentInChildren<Renderer>() != null)
            {
                Color color = cubeColors.Evaluate(value);
                color.a = 0.8f;
                arrow.GetComponentInChildren<Renderer>().material.color = color;
            }
            arrows.Add(arrow);
        }
    }
    private List<HeatmapData> CreatePath(int sessionId)
    {
        List<HeatmapData> orderedData = new List<HeatmapData>();
        foreach(HeatmapData data in heatmapDatas)
        {
            if (data.sessionId == sessionId && data.type == eventType.movement)
            {
                orderedData.Add(data);
            }
        }

        orderedData.Sort((a, b) => DateTime.Compare(a.dateTime, b.dateTime));
        return orderedData;
    }
    private void GenerateCubesGrid(int size) // this fills the cubeList with the correct amount of cubes and sizes/positions. The value of the color will be calculated later
    {

        // The map is 120x80, with the corners being: -33, 40; -33, -40; 94,-40; 94,40 (counter-clockwise)
        // fill an array with all the possible cubes with the given size.

        int topLeftX = (int)topLeft.x;
        int topLeftZ = (int)topLeft.y;

        topLeftX -= (size / 2);
        topLeftZ -= (size / 2);

        int totalSizeX = mapWidth / size;
        int totalSizeZ = mapLength / size;
        int totalSizeY = (-bottomLevel + topLevel) / size;
        if (totalSizeY <= 0) totalSizeY = 1; // So at least we have one cube?

        for (int k = 0; k <= totalSizeY; k++)
        {
            for (int i = 0; i < totalSizeX; i++)
            {
                for (int j = 0; j < totalSizeZ; j++)
                {
                    CubeClass temp = new CubeClass();
                    temp.classCubePrefab = cubePrefab;
                    temp.originPosition.Set(topLeftX + (size * i), bottomLevel + (size * k), topLeftZ - (size * j));
                    temp.size = size;
                    temp.InstantiateCube();
                    cubesList.Add(temp);
                }
            }
        }

        Debug.Log("Grid size: " + cubesList.Count);
    }

    private void CreateMap()
    {
        List<CubeClass> newList = new List<CubeClass>();
        foreach (CubeClass cube in cubesList)
        {
            Dictionary<eventType, int> tempDictionary = new Dictionary<eventType, int>();

            foreach (KeyValuePair<eventType, int> type in cube.nEvents)
            {
                int events = GetEventsInCube(cube, type.Key, filter, playerId, sessionId);

                if (events > 0)
                {
                    if (!newList.Contains(cube))
                    {
                        newList.Add(cube);
                    }
                }

                if (events > maxEvents[type.Key])
                {
                    maxEvents[type.Key] = events;
                }

                tempDictionary.Add(type.Key, events);
            }

            cube.nEvents = tempDictionary;
        }

        DeleteInstantiatedCubes();
        cubesList.Clear();
        cubesList = newList;

        Debug.Log("Data finalized: Count List: " + cubesList.Count);
        foreach (CubeClass cube in cubesList)
        {
            List<int> counts = new List<int>();
            foreach (KeyValuePair<eventType, int> type in cube.nEvents)
            {
                counts.Add(type.Value);
            }

            if (!counts.Any(p => p != 0))
            {
                Debug.Log(counts[0] + " - " + counts[1] + " - " + counts[2] + " - " + counts[3] + " - " + counts[4] + " - " + counts[5] + " - " + counts[6]);
            }
        }
    }

    private int GetEventsInCube(CubeClass cube, eventType type, Filter filter = Filter.none, int playerId = -1, int sessionId = -1)
    {
        int events = 0;

        switch (filter)
        {
            case Filter.none:
                foreach (HeatmapData data in heatmapDatas)
                {
                    if (data.type == type)
                    {
                        if (cube.instance != null)
                        {
                            if (cube.instance.GetComponent<Collider>().bounds.Contains(data.position))
                            {
                                events++;
                            }
                        }
                    }
                }
                break;
            case Filter.player:
                foreach (HeatmapData data in heatmapDatas)
                {
                    if (data.type == type && data.playerId == playerId)
                    {
                        if (cube.instance != null)
                        {
                            if (cube.instance.GetComponent<Collider>().bounds.Contains(data.position))
                            {
                                events++;
                            }
                        }
                    }
                }
                break;
            case Filter.session:
                foreach (HeatmapData data in heatmapDatas)
                {
                    if (data.type == type && data.sessionId == sessionId)
                    {
                        if (cube.instance != null)
                        {
                            if (cube.instance.GetComponent<Collider>().bounds.Contains(data.position))
                            {
                                events++;
                            }
                        }
                    }
                }
                break;
            default:
                foreach (HeatmapData data in heatmapDatas)
                {
                    if (data.type == type)
                    {
                        if (cube.instance != null)
                        {
                            if (cube.instance.GetComponent<Collider>().bounds.Contains(data.position))
                            {
                                events++;
                            }
                        }
                    }
                }
                break;
        }

        return events;
    }

    private void DeleteInstantiatedCubes()
    {
        foreach (CubeClass cube in cubesList)
        {
            cube.DestroyCube();
        }
    }

    private void DeleteInstantiatedArrows()
    {
        if (arrows.Count > 0)
        {
            foreach (GameObject arrow in arrows)
            {
                Destroy(arrow);
            }
        }
        arrows.Clear();
    }
}
