using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class PieceManager : MonoBehaviour
{
  private bool m_firstCollision;
  private bool m_firstDestroy;

  private GameManager m_gameManager;
  private PieceMovement m_pieceMovement;

  private Rigidbody m_rigidbody;

  private Vector3 sizePiece = Vector3.zero;

  private List<GameObject> m_pieces;

  public float[] m_gameLimits;

  private Mutex m_mutex;
  private int m_piecesAlive;
  public float m_timeWait = 0.1f;

  #region Getters

  public GameManager GameManager
  {
    get { return m_gameManager; }
  }

  public Vector3 GetPieceSize()
  {
    if (sizePiece == Vector3.zero)
    {
      sizePiece = m_pieces[0].gameObject.GetComponent<Renderer>().bounds.size;
    }
    return sizePiece;
  }

  #endregion

  void Start()
  {
    m_firstCollision = false;
    InstantiatePiece();
    AddCollisionDetection();
    m_pieceMovement = GetComponent<PieceMovement>();
    m_gameManager = GameManager.GetInstance();
    m_rigidbody = GetComponent<Rigidbody>();

    m_mutex = new Mutex(true);
    m_mutex.ReleaseMutex();
    m_piecesAlive = GetComponent<PieceBuilder>().m_pieces.Count;
  }

  private void InstantiatePiece()
  {
    m_pieces = GetComponent<PieceBuilder>().m_pieces;
  }

  public void MyCollisionEnter(GameObject piece)
  {
    m_mutex.WaitOne();
    if(!m_firstCollision)
    {
      m_firstCollision = true;
      StartCoroutine(EndMovement());
    }
    Destroy(piece);
    --m_piecesAlive;
    if(m_piecesAlive <= 0)
    {
      Destroy(this.gameObject);
    }
    m_mutex.ReleaseMutex();
  }

  private IEnumerator EndMovement()
  {
    yield return new WaitForSeconds(m_timeWait);
    SetVelocityDown(0);
    m_pieceMovement.RemoveCallbacks();
    Destroy(m_pieceMovement);//?
    m_rigidbody.useGravity = true;
    if (GameManager.GetInstance() != null)
    {
      GameManager.GetInstance().OnCollisionDetection();
    }
  }

  private void AddCollisionDetection()
  {
    for (int i = 0; i < m_pieces.Count; ++i)
    {
      GroundDetection cd = m_pieces[i].AddComponent<GroundDetection>();
      cd.m_manager = this;
    }
  }

  public float CalculateMaxHeight()
  {
    float maxHeight = float.MinValue;
    for (int i = 0; i < m_pieces.Count; ++i)
    {
      if (m_pieces[i] == null)
      {
        continue;
      }
      maxHeight = Mathf.Max(maxHeight, m_pieces[i].transform.position.y);
    }
    return maxHeight;
  }

  public void SetVelocitiyLateral(float newVel)
  {
    if (m_pieces != null)
    {
      for (int i = 0; i < m_pieces.Count; ++i)
      {
        Rigidbody r = m_pieces[i].GetComponent<Rigidbody>();
        if (r != null)
        {
          r.velocity = new Vector3(newVel, r.velocity.y, 0);
        }
      }
      m_rigidbody.velocity = new Vector3(newVel, m_rigidbody.velocity.y, 0);

      if(m_rigidbody.transform.position.x < m_gameLimits[0])
      {
        m_rigidbody.MovePosition(new Vector3(m_gameLimits[0], m_rigidbody.transform.position.y, m_rigidbody.transform.position.z));
      }

      if (m_rigidbody.transform.position.x > m_gameLimits[1])
      {
        m_rigidbody.MovePosition(new Vector3(m_gameLimits[1], m_rigidbody.transform.position.y, m_rigidbody.transform.position.z));
      }

    }
  }
  public void SetVelocityDown(float newVel)
  {
    if(m_pieces != null)
    {
      for (int i = 0; i < m_pieces.Count; ++i)
      {
        Rigidbody r = m_pieces[i].GetComponent<Rigidbody>();
        if (r != null)
        {
          r.velocity = Vector3.down * newVel;
        }
      }
      m_rigidbody.velocity = Vector3.down * newVel;
    }
  }

  public void OnDestroyPiece()
  {
    if(!m_firstDestroy)
    {
      m_firstDestroy = true;
      Destroy(this.gameObject);
      GameManager.GetInstance().PieceDeleted();
    }
  }
}
