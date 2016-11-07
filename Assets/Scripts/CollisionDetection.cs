using UnityEngine;
using System.Collections;

public class CollisionDetection : MonoBehaviour
{

  public PiezeManager m_manager;

  void OnCollisionEnter(Collision collision)
  {
    m_manager.OnCollisionEnter();
  }

  void Update()
  {
    if(Input.GetKeyDown(KeyCode.F1))
    {
      m_manager.OnCollisionEnter();
    }
  }
}
