using UnityEngine;
using System.Collections;

public class RememberMe : MonoBehaviour {

  private int ID;
  public int GameObjectID;
  private RememberManager m_manager;

	// Use this for initialization
	void Start () {
    m_manager = RememberManager.GetInstance();
    if(m_manager != null)
    {
      ID = m_manager.Register(this.gameObject, GameObjectID);
    }
  }
	
	// Update is called once per frame
	void OnDestroy () {
    if(m_manager != null)
    {
      m_manager.Destroy(ID);
    }
  }
}
