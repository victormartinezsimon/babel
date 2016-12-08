using UnityEngine;
using System.Collections;

public class GroundDetection : MonoBehaviour
{
  public PieceManager m_manager;
  /*
  void OnCollisionEnter(Collision collision)
    {
    if (m_manager != null)
    {
      m_manager.OnCollisionEnter(this.gameObject);
    }
  }
  */

  void OnCollisionEnter(Collision collision)
  {
    Debug.Log("collision");
  }

  void OnTriggerEnter(Collider other)
  {
    Debug.Log("tigger");
  }
}
