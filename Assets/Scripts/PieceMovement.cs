using UnityEngine;
using System.Collections;

public class PieceMovement : MonoBehaviour {

  private PieceManager m_pieceManager;
  private bool m_fast;
  private bool m_moveLeft;
  private bool m_moveRight;


  void Start()
  {
    m_pieceManager = GetComponent<PieceManager>();
    CreateCallbacks();
  }

  void Update()
  {
    MovementDown();
    LateralMovement();
  }

  public void CreateCallbacks()
  {
    InputManager instance = InputManager.GetInstance();
    instance.Rotate += Rotate;
    instance.MoveDown += MoveDown;
    instance.MoveLeft += MoveLeft;
    instance.MoveRight += MoveRight;
  }

  public void RemoveCallbacks()
  {
    InputManager instance = InputManager.GetInstance();
    instance.Rotate -= Rotate;
    instance.MoveDown -= MoveDown;
    instance.MoveLeft -= MoveLeft;
    instance.MoveRight -= MoveRight;
  }

  #region CallBacks
  private void Rotate(bool left)
  {
    Vector3 actualRotation = transform.rotation.eulerAngles;
    Vector3 vector = left ? Vector3.forward : Vector3.back;
    actualRotation += vector * m_pieceManager.GameManager.MovementVariables.Rotation;
    transform.rotation = Quaternion.Euler(actualRotation);
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
  #endregion

  #region Movements
  private void MovementDown()
  {
    if(m_fast)
    {
      m_pieceManager.Rigidbody.velocity =  Vector3.down * m_pieceManager.GameManager.MovementVariables.ForceFast;
    }
    else
    {
      m_pieceManager.Rigidbody.velocity = Vector3.down * m_pieceManager.GameManager.MovementVariables.ForceNormal;
    }
  }
  private void LateralMovement()
  {
    if(m_moveLeft)
    {
      m_pieceManager.Rigidbody.velocity = new Vector3(-m_pieceManager.GameManager.MovementVariables.ForceNormal, m_pieceManager.Rigidbody.velocity.y, 0);
    }
    if (m_moveRight)
    {
      m_pieceManager.Rigidbody.velocity = new Vector3(m_pieceManager.GameManager.MovementVariables.ForceNormal, m_pieceManager.Rigidbody.velocity.y, 0);
    }
    if (!m_moveLeft && !m_moveRight)
    {
      m_pieceManager.Rigidbody.velocity = new Vector3(0, m_pieceManager.Rigidbody.velocity.y, 0);
    }
  }
  #endregion

}
