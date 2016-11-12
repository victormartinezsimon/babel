using UnityEngine;
using System.Collections;

public class PieceManager : MonoBehaviour
{

  public Rigidbody[] m_piezes;

  private bool m_firstCollision;

  private GameManager m_gameManager;
  private PieceMovement m_pieceMovement;

  private Rigidbody m_rigidbody;

  private Vector3 sizePieze = Vector3.zero;

  #region Getters
 
  public GameManager GameManager
  {
    get { return m_gameManager; }
  }

  public Vector3 GetPiezeSize()
  {
    if(sizePieze == Vector3.zero)
    {
      sizePieze = m_piezes[0].gameObject.GetComponent<Renderer>().bounds.size;
    }
    return sizePieze;
  }

  #endregion

  void Start()
  {
    m_firstCollision = false;
    UpdateCollisionSystem(true);
    AddCollisionDetection();
    m_pieceMovement = GetComponent<PieceMovement>();
    m_gameManager = GameManager.GetInstance();
    m_rigidbody = GetComponent<Rigidbody>();
  }

  public void OnCollisionEnter()
  {
    if (!m_firstCollision)
    {
      m_firstCollision = true;
      SetVelocityDown(0);
      UpdateCollisionSystem(false);
      RemoveCollisionDetectionFromSons();
      UpdateRigidbodyInSons();
      m_pieceMovement.RemoveCallbacks();
      Destroy(m_pieceMovement);
      Destroy(m_rigidbody);

      if (GameManager.GetInstance() != null)
      {
        GameManager.GetInstance().OnCollisionDetection();
      }
      Destroy(this);
    }

  }
  private void UpdateCollisionSystem(bool ignore)
  {
    for (int i = 0; i < m_piezes.Length - 1; ++i)
    {
      for (int j = i + 1; j < m_piezes.Length; ++j)
      {
        Physics.IgnoreCollision(m_piezes[i].gameObject.GetComponent<Collider>(), m_piezes[j].gameObject.GetComponent<Collider>(), ignore);
      }
    }
  }
  private void AddCollisionDetection()
  {
    for (int i = 0; i < m_piezes.Length; ++i)
    {
      CollisionDetection cd = m_piezes[i].gameObject.AddComponent<CollisionDetection>();
      cd.m_manager = this;
    }
  }
  private void UpdateRigidbodyInSons()
  {
    for (int i = 0; i < m_piezes.Length; ++i)
    {
      m_piezes[i].useGravity = true;
      m_piezes[i].isKinematic = false;
    }
  }

  public float CalculateMaxHeight()
  {
    float maxHeight = float.MinValue;
    for (int i = 0; i < m_piezes.Length; ++i)
    {
      if(m_piezes[i] == null)
      {
        continue;
      }
      maxHeight = Mathf.Max(maxHeight, m_piezes[i].gameObject.transform.position.y);
    }
    return maxHeight;
  }
  private void RemoveCollisionDetectionFromSons()
  {
    for (int i = 0; i < m_piezes.Length; ++i)
    {
      Destroy(m_piezes[i].gameObject.GetComponent<CollisionDetection>());
    }
  }

  public void MoveToPosition(Vector3 add)
  {
    for(int i = 0; i < m_piezes.Length; ++i)
    {
      m_piezes[i].MovePosition(m_piezes[i].gameObject.transform.position + add);
    }
    m_rigidbody.MovePosition(this.transform.position + add);
  }

  public void SetVelocitiyLateral(float newVel)
  {
    for (int i = 0; i < m_piezes.Length; ++i)
    {
      m_piezes[i].velocity = new Vector3(newVel, m_piezes[i].velocity.y, 0);
    }
    m_rigidbody.velocity = new Vector3(newVel,m_rigidbody.velocity.y, 0);
  }
  public void SetVelocityDown(float newVel)
  {
    Debug.Log("set velocity Down to =>" + newVel);
    for (int i = 0; i < m_piezes.Length; ++i)
    {
      m_piezes[i].velocity = Vector3.down * newVel;
    }
    m_rigidbody.velocity = Vector3.down * newVel;
  }
}
