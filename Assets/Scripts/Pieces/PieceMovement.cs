using UnityEngine;
using System.Collections;

public class PieceMovement : MonoBehaviour {

  private PieceManager m_pieceManager;
  private bool m_fast;

  void Start()
  {
    m_pieceManager = GetComponent<PieceManager>();
    CreateCallbacks();
  }

  void Update()
  {
    MovementDown();
    //LateralMovement();
  }

  public void CreateCallbacks()
  {
    InputManager instance = InputManager.GetInstance();
    instance.Rotate += Rotate;
    instance.MoveDown += MoveDown;
    instance.MoveLeft += LeftMovement;
    instance.MoveRight += RightMovement;
  }

  public void RemoveCallbacks()
  {
    InputManager instance = InputManager.GetInstance();
    instance.Rotate -= Rotate;
    instance.MoveDown -= MoveDown;
    instance.MoveLeft -= LeftMovement;
    instance.MoveRight -= RightMovement;
  }

  #region CallBacks
  private void Rotate(bool left)
  {
    Vector3 actualRotation = transform.localRotation.eulerAngles;
    Vector3 vector = left ? Vector3.forward : Vector3.back;
    actualRotation += vector * MovementVariables.GetInstance().Rotation;
    transform.localRotation = Quaternion.Euler(actualRotation);
  }

  private void LeftMovement()
  {
    m_pieceManager.MoveToPosition(new Vector3(-m_pieceManager.GetPiezeSize().x, 0, 0));
  }

  private void RightMovement()
  {
    m_pieceManager.MoveToPosition(new Vector3(m_pieceManager.GetPiezeSize().x, 0, 0));
  }

  private void MoveDown(bool start)
  {
    m_fast = start;
  }
  #endregion

  #region Movements
  private void MovementDown()
  {
    if(m_fast)
    {
      m_pieceManager.SetVelocityDown(MovementVariables.GetInstance().ForceFast);
    }
    else
    {
      m_pieceManager.SetVelocityDown(MovementVariables.GetInstance().ForceNormal);
    }
  }
  #endregion

}
