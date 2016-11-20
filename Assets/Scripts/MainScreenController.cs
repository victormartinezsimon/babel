using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainScreenController : MonoBehaviour {

  public void ChangeToGame()
  {
    SceneManager.LoadScene("Game");
  }
}
