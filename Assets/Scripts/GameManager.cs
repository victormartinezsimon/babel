using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{

  public Transform InitialPosition;
  public GameObject[] Pieces;

  private MovementVariables m_movementVariables;
  private PieceManager m_currentPieze;
  private GameObject m_nextPieze;

  private float m_maxHeight;

  #region singleton
  private static GameManager m_instance;
  public static GameManager GetInstance()
  {
    return m_instance;
  }
  #endregion
  #region Getters and Setters
  public MovementVariables MovementVariables
  {
    get { return m_movementVariables; }
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
    m_movementVariables = GetComponent<MovementVariables>();
    m_nextPieze = Pieces[Random.Range(0, Pieces.Length)];
    GeneratePiece();
  }

  public void OnCollisionDetection(float maxHeight)
  {
    if (maxHeight >= m_maxHeight)
    {
      m_maxHeight = maxHeight;
    }
    Debug.Log("max height = " + m_maxHeight);
    GeneratePiece();
  }

  private void GeneratePiece()
  {
    GameObject go = Instantiate(m_nextPieze, InitialPosition.position, Quaternion.identity) as GameObject;
    m_currentPieze = go.GetComponent<PieceManager>();
    m_currentPieze.transform.parent = this.transform;
    m_nextPieze = Pieces[Random.Range(0, Pieces.Length)];
  }
}