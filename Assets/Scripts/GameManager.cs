using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{

  public Transform InitialPosition;
  public GameObject[] Pieces;
  public GameVelocities m_velocities;

  public float m_angleToAdd = 90;

  private PiezeManager m_currentPieze;
  private GameObject m_nextPieze;

  private float m_maxHeight;
  private bool m_fast;
  private bool m_moveLeft;
  private bool m_moveRight;

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
    m_fast = false;
    GeneratePiece();
    AddCallbacks();
  }

  void Update()
  {
    if (m_currentPieze != null)
    {
      MovementDown();
      MovementLeft();
      MovementRight();
    }
  }

  private void MovementDown()
  {
    float velocity = m_fast ? m_velocities.m_velocityDownFaster : m_velocities.m_velocityDown;
    m_currentPieze.transform.position += Vector3.down * Time.deltaTime * velocity;
  }

  private void MovementLeft()
  {
    float velocity = m_moveLeft ? m_velocities.m_velocityLateral : 0;
    m_currentPieze.transform.position -= Vector3.right * Time.deltaTime * velocity;
  }

  private void MovementRight()
  {
    float velocity = m_moveRight ? m_velocities.m_velocityLateral : 0;
    m_currentPieze.transform.position += Vector3.right * Time.deltaTime * velocity;
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
    m_currentPieze = go.GetComponent<PiezeManager>();
    m_currentPieze.transform.parent = this.transform;
    m_nextPieze = Pieces[Random.Range(0, Pieces.Length)];
  }

  private void AddCallbacks()
  {
    InputManager instance = InputManager.GetInstance();
    instance.Rotate += Rotate;
    instance.MoveDown += MoveDown;
    instance.MoveLeft += MoveLeft;
    instance.MoveRight += MoveRight;
  }

  private void Rotate(bool left)
  {
    Vector3 actualRotation = m_currentPieze.transform.rotation.eulerAngles;
    Vector3 vector = left ? Vector3.forward : Vector3.back;
    actualRotation += vector * m_angleToAdd;
    m_currentPieze.transform.rotation = Quaternion.Euler(actualRotation);
  }

  private void MoveDown(bool start)
  {
    m_fast = start;
  }

  private void MoveLeft(bool start)
  {
    m_moveLeft = start;
  }

  private void MoveRight(bool start)
  {
    m_moveRight = start;
  }
}
