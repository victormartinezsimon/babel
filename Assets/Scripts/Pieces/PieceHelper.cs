using UnityEngine;
using System.Collections;

public class PieceHelper : MonoBehaviour {

  public float MinX = -6.4f;
  public float MaxX = 6.4f;
  private Renderer m_renderer;

  void Start()
  {
    m_renderer = GetComponent<Renderer>();
  }
	
	// Update is called once per frame
	void Update () {
    float x = transform.position.x;
	  if( x < MinX || x > MaxX)
    {
      m_renderer.material.color = Color.red;
    }
    else
    {
      m_renderer.material.color = Color.white;
    }
	}
}
