using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
  public Transform InitialPosition;
  public GameObject Ground;
  public GameObject[] Pieces;

  private PieceManager m_currentPieze;
  private GameObject m_nextPieze;

  private List<PieceManager> m_listPieces;

  [Header("Next pieze")]
  public Transform m_nextPiezeHolder;
  private GameObject nextPieceInstance;

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
    m_nextPieze = Pieces[Random.Range(0, Pieces.Length)];
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
    Vector3 position = InitialPosition.position;
    float sizeX = Ground.GetComponent<Renderer>().bounds.size.x;
    float x = Random.Range(-sizeX/2.0f, sizeX / 2.0f);
    position.x = Mathf.RoundToInt(x);
       
    GameObject go = Instantiate(m_nextPieze, position, Quaternion.identity) as GameObject;
    m_currentPieze = go.GetComponent<PieceManager>();
    m_currentPieze.transform.parent = this.transform;
    m_nextPieze = Pieces[Random.Range(0, Pieces.Length)];

    if(nextPieceInstance != null)
    {
      Destroy(nextPieceInstance);
    }

    nextPieceInstance = Instantiate(m_nextPieze, m_nextPiezeHolder.position, Quaternion.identity)as GameObject;
    nextPieceInstance.transform.parent = m_nextPiezeHolder;
    nextPieceInstance.transform.localScale = Vector3.one;
    nextPieceInstance.layer = LayerMask.NameToLayer("NextPiece");
    Destroy(nextPieceInstance.GetComponent<PieceManager>());
    Destroy(nextPieceInstance.GetComponent<PieceMovement>());
    Destroy(nextPieceInstance.GetComponent<Rigidbody>());

    Collider[] colliders = nextPieceInstance.GetComponentsInChildren<Collider>();
    for(int i = 0; i < colliders.Length; ++i)
    {
      colliders[i].enabled = false;
    }


  }

  public float MaxHeight()
  {
    float best = float.MinValue;

    for (int i = 0; i < m_listPieces.Count; ++i)
    {
      best = Mathf.Max(best, m_listPieces[i].CalculateMaxHeight());
    }

    return best;
  }
}