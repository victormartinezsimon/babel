using UnityEngine;
using System.Collections;

public class TriggerDead : MonoBehaviour
{
  public GameObject DeadParticles;
  void OnCollisionEnter(Collision other)
  {
    Destroy(other.gameObject);
    GameObject go = Instantiate(DeadParticles) as GameObject;
    go.transform.position = other.transform.position;
    Destroy(go, 1);
  }
}
