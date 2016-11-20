using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class RememberManager : MonoBehaviour
{
  public int MaxSteps;
  public float TimeBetweenStep;
  public enum PlayingMode { RECORDING, FORWARD, REWIND, NONE };
  public PlayingMode m_state;
  private Mutex m_mutex;
  private Dictionary<int, GameObject> m_gameobjectsForClone;

  public struct Action
  {
    public Vector3 m_position;
    public Quaternion m_rotation;
    public int m_gameobjectID;
    public int ID;
    public GameObject m_GameObject;
  }

  public Dictionary<float, List<Action>> m_allActions;
  //private Dictionary<int, GameObject> m_gameobjectsForSnapshots;
  private int m_gameobjectID;
  private float m_tickInRecord;
  private float m_timeWaitSnapShot;

  //animation stuff
  private float m_timeAcumAnimation;
  private List<float> m_timesSnapShots;
  public float FactorRewind = 1;
  public float FactorForward;
  private Dictionary<int, GameObject> m_gameobjectsForAnimations;

  public event System.Action RewindEnded;
  public event System.Action ForwardEnded;

  #region singleton
  private static RememberManager m_instance;
  public static RememberManager GetInstance()
  {
    return m_instance;
  }
  #endregion

  void Awake()
  {
    if (m_instance != null && m_instance != this)
    {
      Destroy(this.gameObject);
      return;
    }
    m_instance = this;
    m_allActions = new Dictionary<float, List<Action>>();
    m_timesSnapShots = new List<float>();
    m_gameobjectsForAnimations = new Dictionary<int, GameObject>();
    m_gameobjectsForClone = new Dictionary<int, GameObject>();
    m_gameobjectID = 0;
    m_timeWaitSnapShot = 0;
    m_mutex = new Mutex(true);
    m_mutex.ReleaseMutex();
    m_state = PlayingMode.RECORDING;
  }
  void Update()
  {
    switch (m_state)
    {
      case PlayingMode.RECORDING: Record(); break;
      case PlayingMode.REWIND: Rewind(); break;
      case PlayingMode.FORWARD: Forward(); break;
    }
  }

  public int Register(GameObject go, int gameobjectID)
  {
    m_mutex.WaitOne();
    CreateSnapShot();
    Action a = new Action();
    a.ID = m_gameobjectID;
    a.m_position = go.transform.position;
    a.m_rotation = go.transform.rotation;
    a.m_gameobjectID = gameobjectID;
    a.m_GameObject = go;

    m_allActions[m_tickInRecord].Add(a);
    if(!m_gameobjectsForClone.ContainsKey(gameobjectID))
    {
      GameObject forclone = CleanGameObject(Instantiate(go));
      forclone.transform.SetParent(this.gameObject.transform);
      forclone.SetActive(false);
      m_gameobjectsForClone.Add(gameobjectID, forclone);
    }

    ++m_gameobjectID;
    m_mutex.ReleaseMutex();
    return a.ID;
  }
  public void Destroy(int id)
  {
    CreateSnapShot();

    List<Action> list = m_allActions[m_timesSnapShots[m_timesSnapShots.Count - 1]];
    Action a = list.Find((a1) => a1.ID == id);
    list.Remove(a);
    m_allActions[m_timesSnapShots[m_timesSnapShots.Count - 1]] = list;
  }
  private void CreateSnapShot()
  {
    RemoveFirstSnapShot();
    if (m_timesSnapShots.Count > 0)
    {
      List<Action> list = new List<Action>();
      List<Action> lastActions = m_allActions[m_timesSnapShots[m_timesSnapShots.Count - 1]];
      for (int i = 0; i < lastActions.Count; ++i)
      {
        Action action = lastActions[i];
        Action a = new Action();
        a.ID = action.ID;
        a.m_position = action.m_GameObject.transform.position;
        a.m_rotation = action.m_GameObject.transform.rotation;
        a.m_gameobjectID = action.m_gameobjectID;
        a.m_GameObject = action.m_GameObject;
        list.Add(a);
      }
      if(m_allActions.ContainsKey(m_tickInRecord))
      {
        m_allActions[m_tickInRecord] = list;

      }
      else
      {
        m_allActions.Add(m_tickInRecord, list);
      }
    }
    else
    {
      m_allActions.Add(m_tickInRecord, new List<Action>());
    }

    m_timesSnapShots.Add(m_tickInRecord);
  }
  private void RemoveFirstSnapShot()
  {
    if (m_allActions.Count < MaxSteps)
    {
      return;
    }

    float timeToDelete = m_timesSnapShots[0];
    m_allActions.Remove(timeToDelete);
    m_timesSnapShots.RemoveAt(0);
  }

  public void Record()
  {
    m_tickInRecord += Time.deltaTime;
    m_timeWaitSnapShot += Time.deltaTime;

    if (m_timeWaitSnapShot >= TimeBetweenStep)
    {
      m_timeWaitSnapShot = 0;
      CreateSnapShot();
    }
  }

  [ContextMenu("Rewind")]
  public void DoRewind()
  {
    m_timeAcumAnimation = m_timesSnapShots[m_timesSnapShots.Count - 1];
    CloneAllGameObjects();
    ColocateAllPiecesAnimation(m_timesSnapShots.Count - 2, m_timeAcumAnimation);
    HideOriginalGameObjects();
    m_state = PlayingMode.REWIND;
  }
  [ContextMenu("Forward")]
  public void DoForward()
  {
    m_timeAcumAnimation = m_timesSnapShots[0];
    CloneAllGameObjects();
    ColocateAllPiecesAnimation(0, 0);
    HideOriginalGameObjects();
    m_state = PlayingMode.FORWARD;
  }

  private void CloneAllGameObjects()
  {
    foreach (KeyValuePair<int, GameObject> par in m_gameobjectsForAnimations)
    {
      Destroy(par.Value);
    }
    m_gameobjectsForAnimations.Clear();

    List<Action> m_list = m_allActions[m_timesSnapShots[m_timesSnapShots.Count - 1]];
    for (int i = 0; i < m_list.Count; ++i)
    {
      GameObject goClone = Instantiate(m_gameobjectsForClone[m_list[i].m_gameobjectID]) as GameObject;
      goClone.SetActive(true);
      goClone.name = "Animation [" + m_list[i].ID + "]";
      m_gameobjectsForAnimations.Add(m_list[i].ID, goClone);
    }
  }
  private void HideOriginalGameObjects()
  {
    List<Action> m_list = m_allActions[m_timesSnapShots[m_timesSnapShots.Count - 1]];
    for (int i = 0; i < m_list.Count; ++i)
    {
      m_list[i].m_GameObject.SetActive(false);
    }
  }

  public void Rewind()
  {
    if (m_timeAcumAnimation > m_timesSnapShots[0])
    {
      m_timeAcumAnimation -= Time.deltaTime * FactorRewind;
      int index = FindCloseIndex(m_timeAcumAnimation);
      float timeLerp = (m_timeAcumAnimation - m_timesSnapShots[index]) / (m_timesSnapShots[index + 1] - m_timesSnapShots[index]);
      ColocateAllPiecesAnimation(index, timeLerp);
    }
    else
    {
      m_state = PlayingMode.NONE;
      if (RewindEnded != null)
      {
        RewindEnded();
      }
    }
  }

  public void Forward()
  {
    if (m_timeAcumAnimation < m_timesSnapShots[m_timesSnapShots.Count - 1])
    {
      m_timeAcumAnimation += Time.deltaTime * FactorForward;
      int index = FindCloseIndex(m_timeAcumAnimation);
      float timeLerp = (m_timeAcumAnimation - m_timesSnapShots[index]) / (m_timesSnapShots[index + 1] - m_timesSnapShots[index]);
      ColocateAllPiecesAnimation(index, timeLerp);
    }
    else
    {
      m_state = PlayingMode.NONE;
      if (ForwardEnded != null)
      {
        ForwardEnded();
      }
    }
  }

  private int FindCloseIndex(float time)
  {
    for (int i = 1; i < m_timesSnapShots.Count; ++i)
    {
      if (m_timesSnapShots[i] >= time)
      {
        return i - 1;
      }
    }
    return 0;
  }

  private void ColocateAllPiecesAnimation(int index, float timeForLerp)
  {
    float timeFromSnapshot = m_timesSnapShots[index];
    float timeFromSnapshotNext = m_timesSnapShots[index + 1];
    List<Action> actionsInIndex = m_allActions[timeFromSnapshot];
    List<Action> actionsInNextIndex = m_allActions[timeFromSnapshotNext];

    //destroy unnecesary pieces
    List<int> toRemove = new List<int>();
    foreach (int key in m_gameobjectsForAnimations.Keys)
    {
      if (!actionsInIndex.Exists((a) => a.ID == key))
      {
        Destroy(m_gameobjectsForAnimations[key]);
        toRemove.Add(key);
      }
    }

    for (int i = 0; i < toRemove.Count; ++i)
    {
      m_gameobjectsForAnimations.Remove(toRemove[i]);
    }

    for (int i = 0; i < actionsInIndex.Count; ++i)
    {
      Action actual = actionsInIndex[i];
      Action next = actionsInNextIndex.Find((a) => a.ID == actual.ID);
      if (!m_gameobjectsForAnimations.ContainsKey(actual.ID))
      {
        GameObject goClone = Instantiate(m_gameobjectsForClone[actual.m_gameobjectID]) as GameObject;
        goClone.SetActive(true);
        goClone.name = "Animation [" + actual.ID + "]"; ;
        m_gameobjectsForAnimations.Add(actual.ID, goClone);
      }

      GameObject go = m_gameobjectsForAnimations[actual.ID];
      Vector3 position = Vector3.Lerp(actual.m_position, next.m_position, timeForLerp);
      Quaternion rotation = Quaternion.Slerp(actual.m_rotation, next.m_rotation, timeForLerp);
      go.transform.position = position;
      go.transform.rotation = rotation;

    }
  }

  private GameObject CleanGameObject(GameObject go)
  {
    Destroy(go.GetComponent<Rigidbody>());
    Destroy(go.GetComponent<Collider>());
    Destroy(go.GetComponent<RememberMe>());
    Destroy(go.GetComponent<AutoDestroy>());
    return go;
  }
}
