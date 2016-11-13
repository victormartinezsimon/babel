using UnityEngine;
using System.Collections;

public class PieceManager : MonoBehaviour
{

  public Rigidbody[] m_pieces;

  private bool m_firstCollision;

  private GameManager m_gameManager;
  private PieceMovement m_pieceMovement;

  private Rigidbody m_rigidbody;

  private Vector3 sizePiece = Vector3.zero;

  #region Getters
 
  public GameManager GameManager
  {
    get { return m_gameManager; }
  }

  public Vector3 GetPieceSize()
  {
    if(sizePiece == Vector3.zero)
    {
      sizePiece = m_pieces[0].gameObject.GetComponent<Renderer>().bounds.size;
    }
    return sizePiece;
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
    for (int i = 0; i < m_pieces.Length - 1; ++i)
    {
      for (int j = i + 1; j < m_pieces.Length; ++j)
      {
        Physics.IgnoreCollision(m_pieces[i].gameObject.GetComponent<Collider>(), m_pieces[j].gameObject.GetComponent<Collider>(), ignore);
      }
    }
  }
  private void AddCollisionDetection()
  {
    for (int i = 0; i < m_pieces.Length; ++i)
    {
      CollisionDetection cd = m_pieces[i].gameObject.AddComponent<CollisionDetection>();
      cd.m_manager = this;
    }
  }
  private void UpdateRigidbodyInSons()
  {
    for (int i = 0; i < m_pieces.Length; ++i)
    {
      m_pieces[i].useGravity = true;
    }
  }

  public float CalculateMaxHeight()
  {
    float maxHeight = float.MinValue;
    for (int i = 0; i < m_pieces.Length; ++i)
    {
      if(m_pieces[i] == null)
      {
        continue;
      }
      maxHeight = Mathf.Max(maxHeight, m_pieces[i].gameObject.transform.position.y);
    }
    return maxHeight;
  }
  private void RemoveCollisionDetectionFromSons()
  {
    for (int i = 0; i < m_pieces.Length; ++i)
    {
      Destroy(m_pieces[i].gameObject.GetComponent<CollisionDetection>());
    }
  }

  public void MoveToPosition(Vector3 add)
  {
    for(int i = 0; i < m_pieces.Length; ++i)
    {
      m_pieces[i].MovePosition(m_pieces[i].gameObject.transform.position + add);
    }
    m_rigidbody.MovePosition(this.transform.position + add);
  }

  public void SetVelocitiyLateral(float newVel)
  {
    for (int i = 0; i < m_pieces.Length; ++i)
    {
      m_pieces[i].velocity = new Vector3(newVel, m_pieces[i].velocity.y, 0);
    }
    m_rigidbody.velocity = new Vector3(newVel,m_rigidbody.velocity.y, 0);
  }
  public void SetVelocityDown(float newVel)
  {
    for (int i = 0; i < m_pieces.Length; ++i)
    {
      m_pieces[i].velocity = Vector3.down * newVel;
    }
    m_rigidbody.velocity = Vector3.down * newVel;
  }
}
