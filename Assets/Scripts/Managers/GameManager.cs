using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
  public Transform InitialPosition;
  public GameObject Ground;
  public GameObject[] Pieces;

  private PieceManager m_currentPiece;
  private GameObject m_nextPiece;

  private List<PieceManager> m_listPieces;

  [Header("Next piece")]
  public Transform m_nextPieceHolder;
  private GameObject nextPieceInstance;
  public GameObject m_nextPieceCamera;

  private float m_actualPuntuaction;

  public float m_TotalLives = 10;
  private bool m_endGame;
  public GameObject m_endGameText;
  public Text m_actualLivesText;

  #region singleton
  private static GameManager m_instance;
  public static GameManager GetInstance()
  {
    return m_instance;
  }
  #endregion

  #region Getters
  public float ActualHeight
  {
    get { return m_actualPuntuaction; }
  }
  #endregion

  void Awake()
  {
    if (m_instance != null && m_instance != this)
    {
      Destroy(this.gameObject);
      return;
    }
    m_instance = this;
  }
  // Use this for initialization
  void Start()
  {
    m_nextPiece = Pieces[Random.Range(0, Pieces.Length)];
    m_listPieces = new List<PieceManager>();
    EnableTextGameOver(false);
    AudioEngine.GetInstance().PlayNormal();
    SubscribeEvents();
    GeneratePiece();
    UpdateActualLivesText();
  }

  public void OnCollisionDetection()
  {
    m_listPieces.Add(m_currentPiece);
    GeneratePiece();
    CalculateMaxHeight();
    RecordManager.GetInstance().SetActualPuntuaction(m_actualPuntuaction);
  }

  private void GeneratePiece()
  {
    if(m_endGame)
    {
      return;
    }
    Vector3 position = InitialPosition.position;
    float sizeX = Ground.GetComponent<Renderer>().bounds.size.x;
    float x = Random.Range(-sizeX/2.0f, sizeX / 2.0f);
    position.x = x;
       
    GameObject go = Instantiate(m_nextPiece, position, Quaternion.identity) as GameObject;
    m_currentPiece = go.GetComponent<PieceManager>();
    m_currentPiece.transform.parent = this.transform;
    m_nextPiece = Pieces[Random.Range(0, Pieces.Length)];

    if(nextPieceInstance != null)
    {
      Destroy(nextPieceInstance);
    }

    nextPieceInstance = Instantiate(m_nextPiece, m_nextPieceHolder.position, Quaternion.identity)as GameObject;
    nextPieceInstance.transform.parent = m_nextPieceHolder;
    nextPieceInstance.transform.localScale = Vector3.one;
    nextPieceInstance.layer = LayerMask.NameToLayer("NextPiece");
    Destroy(nextPieceInstance.GetComponent<PieceManager>());
    Destroy(nextPieceInstance.GetComponent<PieceMovement>());
    Destroy(nextPieceInstance.GetComponent<Rigidbody>());

    Collider[] colliders = nextPieceInstance.GetComponentsInChildren<Collider>();
    for(int i = 0; i < colliders.Length; ++i)
    {
      colliders[i].enabled = false;
    }

    for(int i = 0; i < nextPieceInstance.transform.childCount; ++i)
    {
      nextPieceInstance.transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer("NextPiece");
      Destroy(nextPieceInstance.transform.GetChild(i).gameObject.GetComponent<PieceHelper>());
    }

  }

  public void CalculateMaxHeight()
  {
    float best = float.MinValue;

    for (int i = 0; i < m_listPieces.Count; ++i)
    {
      best = Mathf.Max(best, m_listPieces[i].CalculateMaxHeight());
    }
    m_actualPuntuaction = best;
  }

  public void PieceDeleted()
  {
    AudioEngine.GetInstance().PlayExplosion();
    --m_TotalLives;
    m_TotalLives = Mathf.Max(0, m_TotalLives);
    UpdateActualLivesText();
    if (m_TotalLives <= 0)
    {
      m_endGame = true;
      m_nextPieceCamera.SetActive(false);
      Destroy(m_currentPiece);
      FinishGame();
    }
  }
  private void FinishGame()
  {
    AudioEngine.GetInstance().PlayRewind();
    RememberManager.GetInstance().DoRewind();
  }

  private void SubscribeEvents()
  {
    RememberManager.GetInstance().RewindEnded += EndRewind;
    RememberManager.GetInstance().ForwardEnded += EndForward;
  }

  private void UnSubscribeEvents()
  {
    RememberManager.GetInstance().RewindEnded -= EndRewind;
    RememberManager.GetInstance().ForwardEnded -= EndForward;
  }

  private void EndRewind()
  {
    AudioEngine.GetInstance().PlayFinalSong();
    RememberManager.GetInstance().DoForward();
  }

  private void EndForward()
  {
    StartCoroutine(RestarGame());
  }

  private IEnumerator RestarGame()
  {
    EnableTextGameOver(true);
    UnSubscribeEvents();
    yield return new WaitForSeconds(5f);
    SceneManager.LoadScene("Start");
  }

  private void EnableTextGameOver(bool value)
  {
    if(m_endGameText != null)
    {
      m_endGameText.SetActive(value);
    }
  }

  private void UpdateActualLivesText()
  {
    if(m_actualLivesText != null)
    {
      m_actualLivesText.text = m_TotalLives.ToString();
    }
  }
}