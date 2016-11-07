using UnityEngine;
using System.Collections;

public class PiezeManager : MonoBehaviour {

  public GameObject[] m_piezes;

  private bool m_firstCollision;
	// Use this for initialization
	void Start ()
  {
    m_firstCollision = false;
    UpdateCollisionSystem(true);
    AddCollisionDetection();

  }
	
  public void OnCollisionEnter()
  {
    if(!m_firstCollision)
    {
      m_firstCollision = true;
      AddRigidBodys();
      UpdateCollisionSystem(false);

      float maxHeight = float.MinValue;
      for(int i = 0; i < m_piezes.Length; ++i)
      {
        maxHeight = Mathf.Max(maxHeight, m_piezes[i].transform.position.y);
        Destroy(m_piezes[i].GetComponent<CollisionDetection>());
      }
      GameManager.GetInstance().OnCollisionDetection(maxHeight);
    }
  }

  private void AddRigidBodys()
  {
    for(int i = 0; i < m_piezes.Length; ++i)
    {
      Rigidbody r = m_piezes[i].AddComponent<Rigidbody>();
      r.useGravity = true;
    }
  }
  private void AddCollisionDetection()
  {
    for (int i = 0; i < m_piezes.Length; ++i)
    {
      CollisionDetection cd = m_piezes[i].AddComponent<CollisionDetection>();
      cd.m_manager = this;
    }
  }
  private void UpdateCollisionSystem(bool ignore)
  {
    for(int i = 0; i < m_piezes.Length -1 ; ++i)
    {
      for(int j = i+1; j < m_piezes.Length; ++j)
      {
        Physics.IgnoreCollision(m_piezes[i].GetComponent<Collider>(), m_piezes[j].GetComponent<Collider>(), ignore);
      }
    }
  }
}
