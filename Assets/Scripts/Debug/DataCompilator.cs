using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public enum eventType
{
    _null = -1,
    movement,
    attack,
    jump,
    hitEnemy,
    killEnemy,
    recieveDamage,
    death
}
public class HeatmapData
{
    public DateTime dateTime;
    public eventType type;
    public uint playerId; //Random number (0 - 9)
    public uint sessionId; //Return from primary key when starting
    public Vector3 position = Vector3.zero;
    public HeatmapData() { }

    public HeatmapData(DateTime dateTime, eventType type, uint playerId, uint sessionId, Vector3 position)
    {
        this.dateTime = dateTime;
        this.type = type;
        this.playerId = playerId;
        this.sessionId = sessionId;
        this.position = position;
    }
    public string GetData()
    {
        float x = (float)(Mathf.Round(position.x * 100f) / 100f);
        float y = (float)(Mathf.Round(position.y * 100f) / 100f);
        float z = (float)(Mathf.Round(position.z * 100f) / 100f);

        string data = "dateTime=" + dateTime.ToString("yyyy-MM-dd HH:mm:ss") 
            + "&type=" + (int)type 
            + "&playerId=" + playerId 
            + "&sessionId=" + sessionId 
            + "&positionX=" + x
            + "&positionY=" + y
            + "&positionZ=" + z;
        return data;
    }

    public void PrintInfo()
    {
        Debug.Log("dateTime: " + this.dateTime);
        Debug.Log("type: " + this.type);
        Debug.Log("playerId: " + this.playerId);
        Debug.Log("sessionId: " + this.sessionId);
        Debug.Log("position: " + this.position);
    }
}

public class SessionClass
{
    public DateTime dateTime;
    public uint id;

    public SessionClass(DateTime dateTime, uint id)
    {
        this.dateTime = dateTime;
        this.id = id;
    }

    public string GetData()
    {
        string data = "dateTime=" + dateTime.ToString("yyyy-MM-dd HH:mm:ss") + "&id=" + id;
        return data;
    }
}
public class DataCompilator : MonoBehaviour
{
    uint playerId = 0;
    uint currentSession = 0;
    bool newSessionStarted = false;

    public GameObject character;
    Vector3 lastPosition;
    float registerTimer = 0.5f;
    float currentTimer = 0.0f;

    public string url = "https://citmalumnes.upc.es/~sergicf4/";
    public string sUrl = "AddSessionGameplay.php";
    public string fUrl = "FinishSessionGameplay.php";
    public string eUrl = "AddEvent.php";

    public static Action<DateTime, eventType, uint, uint, Vector3> OnNewEvent;
    public static Action<DateTime> OnNewSession;

    private void OnEnable()
    {
        OnNewEvent += NewEvent;
        OnNewSession += NewSession;
    }
    private void OnDisable()
    {
        OnNewEvent -= NewEvent;
        OnNewSession -= NewSession;
    }
    // Start is called before the first frame update
    void Start()
    {
        playerId = (uint)UnityEngine.Random.Range(0, 256);
        OnNewSession?.Invoke(DateTime.Now);
        lastPosition = character.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (newSessionStarted)
        {
            currentTimer += Time.deltaTime;
            if (currentTimer >= registerTimer)
            {
                currentTimer = 0.0f;
                
                if (lastPosition != character.transform.position)
                {
                    Debug.Log("Registered: Character movement!");
                    OnNewEvent?.Invoke(
                        DateTime.Now,
                        eventType.movement,
                        playerId,
                        currentSession,
                        character.transform.position);
                }

                lastPosition = character.transform.position;
            }
        }
    }
    public void RegisterCharacterJump()
    {
        if (newSessionStarted)
        {
            Debug.Log("Registered: Jump!");
            OnNewEvent?.Invoke(
                DateTime.Now,
                eventType.jump,
                playerId,
                currentSession,
                character.transform.position);
        }
    }
    public void RegisterAttack()
    {
        if (newSessionStarted)
        {
            Debug.Log("Registered: Attack!");
            OnNewEvent?.Invoke(
                DateTime.Now,
                eventType.attack,
                playerId,
                currentSession,
                character.transform.position);
        }
    }
    public void RegisterRecieveDamage()
    {
        if (newSessionStarted)
        {
            Debug.Log("Registered: Recieved damage!");
            OnNewEvent?.Invoke(
                DateTime.Now,
                eventType.recieveDamage,
                playerId,
                currentSession,
                character.transform.position);
        }
    }
    public void RegisterDeath()
    {
        if (newSessionStarted)
        {
            Debug.Log("Registered: Character death!");
            OnNewEvent?.Invoke(
                DateTime.Now,
                eventType.death,
                playerId,
                currentSession,
                character.transform.position);
        }
    }
    public void RegisterHitEnemy()
    {
        if (newSessionStarted)
        {
            Debug.Log("Registered: Enemy hit!");
            OnNewEvent?.Invoke(
                DateTime.Now,
                eventType.hitEnemy,
                playerId,
                currentSession,
                character.transform.position);
        }
    }

    private void NewEvent(DateTime dateTime, eventType type, uint playerId, uint sessionId, Vector3 position)
    {
        HeatmapData hmBuffer = new HeatmapData(dateTime, type, playerId, sessionId, position);
        StartCoroutine(Event2PHP(hmBuffer));
    }
    IEnumerator Event2PHP(HeatmapData d)
    {
        string dataUrl = url + eUrl + "?" + d.GetData();
        Debug.Log(dataUrl);
        WWW www = new WWW(dataUrl);

        yield return www;

        if (www.error == null)
        {
            Debug.Log(www.text);
            uint eventId = uint.Parse(www.text);
        }
        else
        {
            Debug.LogError("Error: " + www.error);
        }
    }

    private void NewSession(DateTime dateTime)
    {
        SessionClass sBuffer = new SessionClass(dateTime, playerId);
        StartCoroutine(SessionStart2PHP(sBuffer));
    }

    IEnumerator SessionStart2PHP(SessionClass s)
    {
        string dataUrl = url + sUrl + "?" + s.GetData();
        Debug.Log(dataUrl);
        WWW www = new WWW(dataUrl);

        yield return www;

        if (www.error == null)
        {
            Debug.Log(www.text);
            currentSession = uint.Parse(www.text);
            newSessionStarted = true;
        }
        else
        {
            Debug.LogError("Error: " + www.error);
        }
    }

    public void EndSession(DateTime dateTime)
    {
        SessionClass sBuffer = new SessionClass(dateTime, currentSession);
        SessionEnd2PHP(sBuffer);
    }

    void SessionEnd2PHP(SessionClass s)
    {
        string dataUrl = url + fUrl + "?" + s.GetData();
        Debug.Log(dataUrl);
        WWW www = new WWW(dataUrl);

        while (!www.isDone){ }

        if (www.error == null)
        {
            Debug.Log(www.text);
        }
        else
        {
            Debug.LogError("Error: " + www.error);
        }

        Debug.Log("Session has ended...");
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Ending session...");
        EndSession(DateTime.Now);
    }
}
