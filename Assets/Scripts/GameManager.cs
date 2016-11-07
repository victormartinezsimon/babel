using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{

  public Transform InitialPosition;

  public GameObject[] Pieces;

  public float TimeAcum = 0;

  // Use this for initialization
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    TimeAcum += Time.deltaTime;
    if (TimeAcum > 3)
    {
      GeneratePiece();
      TimeAcum = 0;
    }
  }

  private void GeneratePiece()
  {
    GameObject piece = Pieces[Random.Range(0, Pieces.Length)];
    GameObject go = Instantiate(piece, InitialPosition.position, Quaternion.identity) as GameObject;
    go.transform.rotation = Quaternion.Euler(0, 0, 90 * Random.Range(0,4));
  }
}
