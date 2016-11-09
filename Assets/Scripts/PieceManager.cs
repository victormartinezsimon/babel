using UnityEngine;
using System.Collections;

public class PieceManager : MonoBehaviour
{

  public GameObject[] m_piezes;

  private bool m_firstCollision;

  private Rigidbody m_rigidbody;
  private GameManager m_gameManager;
  private PieceMovement m_pieceMovement;

  #region Getters
  public Rigidbody Rigidbody
  {
    get { return m_rigidbody; }
  }

  public GameManager GameManager
  {
    get { return m_gameManager; }
  }

  #endregion

  void Start()
  {
    m_firstCollision = false;
    UpdateCollisionSystem(true);
    AddCollisionDetection();
    m_rigidbody = GetComponent<Rigidbody>();
    m_pieceMovement = GetComponent<PieceMovement>();
    m_gameManager = GameManager.GetInstance();
  }

  public void OnCollisionEnter()
  {

    if (!m_firstCollision)
    {
      m_firstCollision = true;
      GameManager.GetInstance().OnCollisionDetection(CalculateMaxHeight());
      UpdateCollisionSystem(false);
      RemoveCollisionDetectionFromSons();
      AddRigidbodysToSons();
      m_pieceMovement.RemoveCallbacks();
      Destroy(m_rigidbody);
      Destroy(m_pieceMovement);
      Destroy(this);
    }

  }
  private void UpdateCollisionSystem(bool ignore)
  {
    for (int i = 0; i < m_piezes.Length - 1; ++i)
    {
      for (int j = i + 1; j < m_piezes.Length; ++j)
      {
        Physics.IgnoreCollision(m_piezes[i].GetComponent<Collider>(), m_piezes[j].GetComponent<Collider>(), ignore);
      }
    }
  }
  private void AddCollisionDetection()
  {
    for (int i = 0; i < m_piezes.Length; ++i)
    {
      CollisionDetection cd = m_piezes[i].AddComponent<CollisionDetection>();
      cd.m_manager = this;
    }
  }
  private void AddRigidbodysToSons()
  {
    for (int i = 0; i < m_piezes.Length; ++i)
    {
      m_piezes[i].AddComponent<Rigidbody>();
    }
  }
  private float CalculateMaxHeight()
  {
    float maxHeight = float.MinValue;
    for (int i = 0; i < m_piezes.Length; ++i)
    {
      maxHeight = Mathf.Max(maxHeight, m_piezes[i].transform.position.y);
    }
    return maxHeight;
  }
  private void RemoveCollisionDetectionFromSons()
  {
    for (int i = 0; i < m_piezes.Length; ++i)
    {
      Destroy(m_piezes[i].GetComponent<CollisionDetection>());
    }
  }

}
