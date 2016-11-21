using UnityEngine;
using System.Collections;

public class PieceAutoDestroy : MonoBehaviour {

  public float m_distanceToDead;
	
  // Update is called once per frame
	void Update () {
	  if(transform.position.y < m_distanceToDead)
    {
      Destroy(this.gameObject);
      GameManager.GetInstance().PieceDeleted();
    }
	}
}
