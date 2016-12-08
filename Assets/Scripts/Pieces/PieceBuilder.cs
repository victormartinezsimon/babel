using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PieceBuilder : MonoBehaviour {

  public Transform[] m_positions;
  public List<GameObject> m_pieces;
  public GameObject m_cube;

  // Use this for initialization
  void Awake () {
    InstantiatePiece();
  }

  private void InstantiatePiece()
  {
    m_pieces = new List<GameObject>();
    for (int i = 0; i < m_positions.Length; ++i)
    {
      GameObject go = Instantiate(m_cube, m_positions[i].position, Quaternion.identity) as GameObject;
      m_pieces.Add(go);
      go.transform.parent = this.transform;
      go.name = this.gameObject.name + "-" + i;
    }
  }
}
