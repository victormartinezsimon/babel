using UnityEngine;
using System.Collections;

public class MovementTest : MonoBehaviour {

  public float times = 10;
  private float m_frecuence;
  private float timeAcum = 0;
	// Use this for initialization
	void Start () {
    GetComponent<Rigidbody>().velocity = (Vector3.down * times);
  }
	
  
	// Update is called once per frame
	void Update () {
    GetComponent<Rigidbody>().velocity = (Vector3.down * times);
    /*
    m_frecuence = 1 / times;
    timeAcum += Time.deltaTime;
    if(timeAcum >= m_frecuence)
    {
      timeAcum = 0;
      this.transform.position += Vector3.down;
    }
    */
  }
  
}
