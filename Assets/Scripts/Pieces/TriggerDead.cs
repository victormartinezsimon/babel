using UnityEngine;
using System.Collections;

public class TriggerDead : MonoBehaviour
{
  public GameObject DeadParticles;
  void OnCollisionEnter(Collision other)
  {
    PieceManager pm = other.gameObject.GetComponent<PieceManager>();
    if(pm != null)
    {
      pm.OnDestroyPiece();
    }
    else
    {
      Debug.Log(other.transform.name);
      Destroy(other.gameObject);
      GameManager.GetInstance().PieceDeleted();
    }

    GameObject go = Instantiate(DeadParticles) as GameObject;
    go.transform.position = other.transform.position;
    Destroy(go, 1);
  }
}
