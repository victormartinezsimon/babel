using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{

  public Transform InitialPosition;
  public GameObject[] Pieces;

  public float m_angleToAdd = 90;

  private PiezeManager m_currentPieze;
  private GameObject m_nextPieze;

  private float m_MaxHeight;

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
    GeneratePiece();
    AddCallbacks();
  }

  public void OnCollisionDetection(float maxHeight)
  {
    if(maxHeight >= m_MaxHeight)
    {
      m_MaxHeight = maxHeight;
    }
    Debug.Log("max height = " + m_MaxHeight);
    GeneratePiece();
  }

  private void GeneratePiece()
  {
    GameObject go = Instantiate(m_nextPieze, InitialPosition.position, Quaternion.identity) as GameObject;
    m_currentPieze = go.GetComponent<PiezeManager>();
    m_currentPieze.transform.parent = this.transform;
    m_nextPieze = Pieces[Random.Range(0, Pieces.Length)];
  }

  private void AddCallbacks()
  {
    InputManager instance = InputManager.GetInstance();
    instance.RotateLeft += RotateLeft;
    instance.RotateRight += RotateRight;
  }


  private void RotateLeft()
  {
    Vector3 actualRotation = m_currentPieze.transform.rotation.eulerAngles;
    actualRotation += Vector3.forward * m_angleToAdd;
    m_currentPieze.transform.rotation = Quaternion.Euler(actualRotation);
  }

  private void RotateRight()
  {
    Vector3 actualRotation = m_currentPieze.transform.rotation.eulerAngles;
    actualRotation -= Vector3.forward * m_angleToAdd;
    m_currentPieze.transform.rotation = Quaternion.Euler(actualRotation);
  }
}
