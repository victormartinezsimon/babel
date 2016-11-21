using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
  public Transform m_initialPosition;
  public GameObject m_ground;
  public GameObject[] m_pieces;

  private PieceManager m_currentPiece;
  private GameObject m_nextPiece;

  private List<PieceManager> m_listPieces;

  [Header("Next piece")]
  public Transform m_nextPieceHolder;
  private GameObject m_nextPieceInstance;
  public GameObject m_nextPieceCamera;
  public float LimitLeft;
  public float LimitRight;

  private float m_actualPuntuaction;

  public float m_TotalLives = 10;
  private bool m_rewind;
  private bool m_endGame;
  public GameObject m_endGameText;
  public Text m_actualLivesText;

  [Header("Rewind")]
  public GameObject m_rewindGeneral;
  public GameObject m_rewindLogo;
  public GameObject m_rewindLines;

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
    m_nextPiece = m_pieces[Random.Range(0, m_pieces.Length)];
    m_listPieces = new List<PieceManager>();
    EnableTextGameOver(false);
    AudioEngine.GetInstance().PlayNormal();
    SubscribeEvents();
    GeneratePiece();
    UpdateActualLivesText();
    m_rewindGeneral.SetActive(false);
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
    Vector3 position = m_initialPosition.position;
    float sizeX = m_ground.GetComponent<Renderer>().bounds.size.x;
    float x = Random.Range(-sizeX/2.0f, sizeX / 2.0f);
    position.x = x;
       
    GameObject go = Instantiate(m_nextPiece, position, Quaternion.identity) as GameObject;
    m_currentPiece = go.GetComponent<PieceManager>();
    m_currentPiece.transform.parent = this.transform;
    m_currentPiece.m_gameLimits = new float[] { LimitLeft, LimitRight };
    m_nextPiece = m_pieces[Random.Range(0, m_pieces.Length)];

    if(m_nextPieceInstance != null)
    {
      Destroy(m_nextPieceInstance);
    }

    m_nextPieceInstance = Instantiate(m_nextPiece, m_nextPieceHolder.position, Quaternion.identity)as GameObject;
    m_nextPieceInstance.transform.parent = m_nextPieceHolder;
    m_nextPieceInstance.transform.localScale = Vector3.one;
    m_nextPieceInstance.layer = LayerMask.NameToLayer("NextPiece");
    Destroy(m_nextPieceInstance.GetComponent<PieceManager>());
    Destroy(m_nextPieceInstance.GetComponent<PieceMovement>());
    Destroy(m_nextPieceInstance.GetComponent<Rigidbody>());

    Collider[] colliders = m_nextPieceInstance.GetComponentsInChildren<Collider>();
    for(int i = 0; i < colliders.Length; ++i)
    {
      colliders[i].enabled = false;
    }

    for(int i = 0; i < m_nextPieceInstance.transform.childCount; ++i)
    {
      m_nextPieceInstance.transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer("NextPiece");
      Destroy(m_nextPieceInstance.transform.GetChild(i).gameObject.GetComponent<PieceHelper>());
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
      m_rewind = true;
      m_nextPieceCamera.SetActive(false);
      Destroy(m_currentPiece);
      FinishGame();
    }
  }
  private void FinishGame()
  {
    AudioEngine.GetInstance().PlayRewind();
    m_rewindGeneral.SetActive(true);
    StartCoroutine(AnimateRewindLogo());
    StartCoroutine(AnimateRewindLines());
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
    m_rewind = false;
    m_rewindGeneral.SetActive(false);
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

  private IEnumerator AnimateRewindLogo()
  {
    while(m_rewind)
    {
      m_rewindLogo.SetActive(true);
      yield return new WaitForSeconds(0.5f);
      m_rewindLogo.SetActive(false);
      yield return new WaitForSeconds(0.5f);
    }
  }

  private IEnumerator AnimateRewindLines()
  {
    while (m_rewind)
    {
      m_rewindLines.SetActive(true);
      yield return new WaitForSeconds(Random.Range(0.2f, 0.8f));
      m_rewindLines.SetActive(false);
      yield return new WaitForSeconds(Random.Range(0.2f, 0.8f));
    }
  }
}