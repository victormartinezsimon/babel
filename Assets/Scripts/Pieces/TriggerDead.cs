using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TriggerDead : MonoBehaviour
{
  public GameObject DeadParticles;
  void OnCollisionEnter(Collision other)
  {
    List<string> goEliminated = new List<string>();
    ContactPoint[] points = other.contacts;
    for(int i = 0; i < points.Length; ++i)
    {
      //Debug.Log(points[i].otherCollider.gameObject.name);
      GameObject go = points[i].otherCollider.gameObject;

      if(goEliminated.Contains(go.name))
      {
        continue;
      }
      goEliminated.Add(go.name);

      PieceManager pm = go.GetComponentInParent<PieceManager>();

      if(pm != null)
      {
        pm.DestroyPiece(go);
      }
      else
      {
        Destroy(go);
      }

      GameObject particles = Instantiate(DeadParticles) as GameObject;
      particles.transform.position = other.transform.position;
      Destroy(particles, 1);
    }
    
    /*
    GameObject go = other.gameObject;
    PieceManager pm = go.GetComponentInParent<PieceManager>();

    if (pm != null)
    {
      pm.DestroyPiece(go);
    }
    else
    {
      Destroy(go);
      Debug.Log("this piece will be deleted withoug manager => " + go.name);
    }

    GameObject particles = Instantiate(DeadParticles) as GameObject;
    particles.transform.position = other.transform.position;
    Destroy(particles, 1);
    */
  }
}
