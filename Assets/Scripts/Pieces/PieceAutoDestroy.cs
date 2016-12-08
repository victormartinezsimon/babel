using UnityEngine;
using System.Collections;

public class PieceAutoDestroy : MonoBehaviour
{

  public float m_distanceToDead;
  private PieceManager m_manager;

  void Start()
  {
    m_manager = GetComponentInParent<PieceManager>();
  }

  // Update is called once per frame
  void Update()
  {
    if (transform.position.y < m_distanceToDead)
    {
      m_manager.DestroyPiece(this.gameObject);
    }
  }
}
