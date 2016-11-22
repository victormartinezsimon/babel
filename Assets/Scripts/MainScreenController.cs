using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainScreenController : MonoBehaviour {

  void Update()
  {
    if(Input.GetKeyDown(KeyCode.Escape))
    {
      Application.Quit();
    }
  }

  public void ChangeToGame()
  {
    SceneManager.LoadScene("Game");
  }
}
