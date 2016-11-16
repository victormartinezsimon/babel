using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RememberManager : MonoBehaviour
{
  public int MaxSteps;
  public float TimeBetweenStep;
  public enum PlayingMode { RECORDING, PLAY, REWIND };
  private PlayingMode m_state;

  public struct Actions
  {
    public enum Action { CREATE, DESTROY, POSITION }
    public Transform m_transform;
    public Action m_action;
    public int ID;
  }

  public Dictionary<float, List<Actions>> m_allActions;
  private Dictionary<int, GameObject> m_gameobjects;
  private int m_lastId;
  private float m_totalTime;
  private float m_timeSnapShot;

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
    m_allActions = new Dictionary<float, List<Actions>>();
    m_gameobjects = new Dictionary<int, GameObject>();
    m_lastId = 0;
    m_state = PlayingMode.RECORDING;
  }
  void Update()
  {
    switch (m_state)
    {
      case PlayingMode.RECORDING: Record(); break;
    }
  }

  public int Register(GameObject go)
  {
    CreateSnapShot();

    Actions a = new Actions();
    a.ID = m_lastId;
    a.m_action = Actions.Action.CREATE;
    a.m_transform = go.transform;

    m_allActions[m_totalTime].Add(a);
    m_gameobjects.Add(m_lastId, go);

    ++m_lastId;
    return m_lastId;
  }
  public void Destroy(int id)
  {
    GameObject go = m_gameobjects[id];
    m_gameobjects.Remove(id);
    CreateSnapShot();

    Actions aDestroy = new Actions();
    aDestroy.ID = id;
    aDestroy.m_action = Actions.Action.DESTROY;
    aDestroy.m_transform = go.transform;

    m_allActions[m_totalTime].Add(aDestroy);
  }
  private void CreateSnapShot()
  {
    RemoveFirstSnapShot();

    List<Actions> list = new List<Actions>();
    foreach (KeyValuePair<int, GameObject> par in m_gameobjects)
    {
      Actions a = new Actions();
      a.m_action = Actions.Action.POSITION;
      a.ID = par.Key;
      a.m_transform = par.Value.transform;
      list.Add(a);
    }
    m_allActions.Add(m_totalTime, list);
  }
  private void RemoveFirstSnapShot()
  {
    if (m_allActions.Count < MaxSteps)
    {
      return;
    }

    float min = float.MaxValue;
    foreach (float value in m_allActions.Keys)
    {
      min = Mathf.Min(min, value);
    }
    m_allActions.Remove(min);
  }

  public void Record()
  {
    m_totalTime += Time.deltaTime;
    m_timeSnapShot += Time.deltaTime;

    if (m_timeSnapShot >= TimeBetweenStep)
    {
      m_timeSnapShot = 0;
      CreateSnapShot();
    }
  }

}
