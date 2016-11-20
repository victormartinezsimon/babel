using UnityEngine;
using System.Collections;

public class AudioEngine : MonoBehaviour
{
  #region singleton
  private static AudioEngine m_instance;
  public static AudioEngine GetInstance()
  {
    return m_instance;
  }
  #endregion

  public AudioClip m_endSong;
  public AudioClip m_rewind;
  public AudioClip m_normal;
  [Header("FX")]
  public AudioClip m_explosion;
  public AudioClip m_fireworks;


  private AudioSource m_sourceGeneral;
  private AudioSource m_sourceFX;

  void Awake()
  {
    if (m_instance != null && m_instance != this)
    {
      Destroy(this.gameObject);
      return;
    }
    m_instance = this;
  }

  void Start()
  {
    GameObject general = new GameObject("Audio General");
    m_sourceGeneral = general.AddComponent<AudioSource>();
    general.transform.SetParent(this.gameObject.transform);

    GameObject fx = new GameObject("Audio FX");
    m_sourceFX = fx.AddComponent<AudioSource>();
    fx.transform.SetParent(this.gameObject.transform);
  }

  public void PlayFinalSong()
  {
    m_sourceGeneral.Stop();
    m_sourceGeneral.clip = m_endSong;
    m_sourceGeneral.loop = true;
    m_sourceGeneral.Play();
  }

  public void PlayRewind()
  {
    m_sourceGeneral.Stop();
    m_sourceGeneral.clip = m_rewind;
    m_sourceGeneral.loop = true;
    m_sourceGeneral.Play();
  }

  public void PlayNormal()
  {
    m_sourceGeneral.Stop();
    m_sourceGeneral.clip = m_normal;
    m_sourceGeneral.loop = true;
    m_sourceGeneral.Play();
  }
  public void PlayExplosion()
  {
    m_sourceFX.Stop();
    m_sourceFX.clip = m_explosion;
    m_sourceFX.Play();
    m_sourceFX.volume = 0.5f;
  }
  public void PlayFirworks()
  {
    m_sourceFX.Stop();
    m_sourceFX.clip = m_fireworks;
    m_sourceFX.Play();
    m_sourceFX.volume = 0.5f;
  }
}
