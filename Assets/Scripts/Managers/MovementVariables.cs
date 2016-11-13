using UnityEngine;
using System.Collections;
using System;

public class MovementVariables : MonoBehaviour
{
  public float ForceNormal = 10;
  public float ForceFast = 20;
  public float Rotation = 90;
  public float ForceLateral = 40;

  private static MovementVariables m_instance = null;

  public static MovementVariables GetInstance()
  {
    return m_instance;
  }

  void Awake()
  {
    if(m_instance != null && m_instance != this)
    {
      Destroy(this.gameObject);
    }
    m_instance = this;
  }

}
