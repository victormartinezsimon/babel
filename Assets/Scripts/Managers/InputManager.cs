using UnityEngine;
using System.Collections;
using System;

public class InputManager : MonoBehaviour
{

  public float m_timeBetweenInput = 0.2f;
  private float m_timeBetweenInptuAcum = float.MaxValue;

  public KeyCode m_KeyMoveDown;
  public KeyCode m_KeyMoveLeft;
  public KeyCode m_KeyMoveRight;
  public KeyCode m_KeyRotateLeft;
  public KeyCode m_KeyRotateRight;

  public event Action Exit;
  public event Action<bool> MoveDown;
  public event Action<bool> Rotate;
  public event Action<bool> MoveLeft;
  public event Action<bool> MoveRight;

  #region singleton
  private static InputManager m_instance;
  public static InputManager GetInstance()
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

  // Update is called once per frame
  void Update()
  {
    m_timeBetweenInptuAcum += Time.deltaTime;
    if (m_timeBetweenInptuAcum >= m_timeBetweenInput)
    {
      m_timeBetweenInptuAcum = 0;
      ManageSecondaryInput();
    }
    ManageImportantInput();
  }

  private void ManageImportantInput()
  {
    if(Input.GetKeyDown(KeyCode.Escape))
    {
      Exit();
    }
    if (DownManagement())
    {
      return;
    }

    if (OnLeftMovement())
    {
      return;
    }

    if (OnRightMovement())
    {
      return;
    }
  }

  private void ManageSecondaryInput()
  {
    if (RotationManagement())
    {
      return;
    }
  }

  private bool DownManagement()
  {
    if (Input.GetKeyDown(m_KeyMoveDown))
    {
      if (MoveDown != null)
      {
        MoveDown(true);
        return true;
      }
    }

    if (Input.GetKeyUp(m_KeyMoveDown))
    {
      if (MoveDown != null)
      {
        MoveDown(false);
        return true;
      }
    }
    return false;
  }

  private bool OnLeftMovement()
  {
    if (Input.GetKeyDown(m_KeyMoveLeft))
    {
      if (MoveLeft != null)
      {
        MoveLeft(true);
        return true;
      }
    }
    if (Input.GetKeyUp(m_KeyMoveLeft))
    {
      if (MoveLeft != null)
      {
        MoveLeft(false);
        return true;
      }
    }
    return false;
  }
  private bool OnRightMovement()
  {
    if (Input.GetKeyDown(m_KeyMoveRight))
    {
      if (MoveRight != null)
      {
        MoveRight(true);
        return true;
      }
    }

    if (Input.GetKeyUp(m_KeyMoveRight))
    {
      if (MoveRight != null)
      {
        MoveRight(false);
        return true;
      }
    }
    return false;
  }

  private bool RotationManagement()
  {
    if (Input.GetKey(m_KeyRotateLeft))
    {
      if (Rotate != null)
      {
        Rotate(true);
        return true;
      }
    }
    if (Input.GetKey(m_KeyRotateRight))
    {
      if (Rotate != null)
      {
        Rotate(false);
        return true;
      }
    }
    return false;
  }

}
