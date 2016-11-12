using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{

  public Transform InitialPosition;
  public GameObject[] Pieces;

  private PieceManager m_currentPieze;
  private GameObject m_nextPieze;

  private List<PieceManager> m_listPieces; 

  #region singleton
  private static GameManager m_instance;
  public static GameManager GetInstance()
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
  }
  // Use this for initialization
  void Start()
  {
    m_nextPieze = Pieces[0];//Pieces[Random.Range(0, Pieces.Length)];
    m_listPieces = new List<PieceManager>();
    GeneratePiece();
  }

  public void OnCollisionDetection()
  {
    m_listPieces.Add(m_currentPieze);
    GeneratePiece();
  }

  private void GeneratePiece()
  {
    GameObject go = Instantiate(m_nextPieze, InitialPosition.position, Quaternion.identity) as GameObject;
    m_currentPieze = go.GetComponent<PieceManager>();
    m_currentPieze.transform.parent = this.transform;
    m_nextPieze = Pieces[0];//Pieces[Random.Range(0, Pieces.Length)];
  }

  public float MaxHeight()
  {
    float best = float.MinValue;

    for(int i = 0; i < m_listPieces.Count; ++i)
    {
      best = Mathf.Max(best, m_listPieces[i].CalculateMaxHeight());
    }

    return best;
  }
}