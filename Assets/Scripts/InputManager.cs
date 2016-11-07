using UnityEngine;
using System.Collections;
using System;

public class InputManager : MonoBehaviour {

  public float m_timeBetweenInput = 0.2f;
  private float m_timeBetweenInptuAcum = float.MaxValue;

  public KeyCode m_KeyMoveDown;
  public KeyCode m_KeyMoveLeft;
  public KeyCode m_KeyMoveRight;
  public KeyCode m_KeyRotateLeft;
  public KeyCode m_KeyRotateRight;

  public event Action MoveDown;
  public event Action MoveLeft;
  public event Action MoveRight;
  public event Action RotateLeft;
  public event Action RotateRight;

  #region singleton
  private static InputManager m_instance;
  public static InputManager GetInstance()
  {
    return m_instance;
  }
  #endregion
  void Awake()
  {
    if(m_instance != null && m_instance != this)
    {
      Destroy(this.gameObject);
      return;
    }
    m_instance = this;
  }
  
	// Update is called once per frame
	void Update () {
    m_timeBetweenInptuAcum += Time.deltaTime;
    if(m_timeBetweenInptuAcum >= m_timeBetweenInput)
    {
      m_timeBetweenInptuAcum = 0;
      ManageInput();
    }
	}

  private void ManageInput()
  {
    if(Input.GetKey(m_KeyMoveDown))
    {
      if(MoveDown != null)
      {
        MoveDown();
        return;
      }
    }
    if (Input.GetKey(m_KeyMoveLeft))
    {
      if(MoveLeft != null)
      {
        MoveLeft();
        return;
      }
    }
    if (Input.GetKey(m_KeyMoveRight))
    {
      if(MoveRight != null)
      {
        MoveRight();
        return;
      }
    }
    if (Input.GetKey(m_KeyRotateLeft))
    {
      if(RotateLeft != null)
      {
        RotateLeft();
        return;
      }
    }
    if (Input.GetKey(m_KeyRotateRight))
    {
      if(RotateRight != null)
      {
        RotateRight();
        return;
      }
    }
  }
}
