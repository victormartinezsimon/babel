using UnityEngine;
using System.Collections;

public class CreateSomething : MonoBehaviour {

  public float Time;
  public GameObject clone;

	// Use this for initialization
	void Start () {
    StartCoroutine(Create());
	}
	
	IEnumerator Create() {
    yield return new WaitForSeconds(Time);
    GameObject go = Instantiate(clone) as GameObject;
    go.transform.position = this.transform.position;
	}
}
