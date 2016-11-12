using UnityEngine;
using System.Collections;

public class CollisionDetection : MonoBehaviour
{
  public PieceManager m_manager;
  void OnCollisionEnter(Collision collision)
  {
    if (m_manager != null)
    {
      m_manager.OnCollisionEnter();
    }
  }

  void OnTriggerEnter(Collider other)
  {
      m_manager.OnCollisionEnter();
  }
}
