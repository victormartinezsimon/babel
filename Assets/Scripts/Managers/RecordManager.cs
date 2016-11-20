using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RecordManager : MonoBehaviour {


  private static RecordManager m_instance;
  private float m_actualValue = 0;
  private static string KEY_PLAYERPREFS = "RECORD";
  private float m_bestValue = 0;

  public GameObject m_bestLine;
  public Text m_actualValueText;
  public GameObject m_fireworks;
  public float m_minRecord = 10;

  public static RecordManager GetInstance()
  {
    return m_instance;
  }

  public float ActualValue
  {
    get { return m_actualValue; }
  }

  public float BestValue
  {
    get { return m_bestValue; }
  }

  void Awake()
  {
    if(m_instance != null && m_instance != this)
    {
      Destroy(this);
    }
    m_instance = this;

    m_bestValue = PlayerPrefs.GetFloat(KEY_PLAYERPREFS);
    m_bestValue = Mathf.Max(m_minRecord, m_bestValue);
    SetText(m_actualValueText, 0.0f);

    if(m_bestLine != null)
    {
      Vector3 pos = m_bestLine.transform.position;
      pos.y = m_bestValue;
      m_bestLine.transform.position = pos;
    }

  }
	
  public void SetActualPuntuaction(float val)
  {
    if(val > m_actualValue)
    {
      m_actualValue = val;
      SetText(m_actualValueText, m_actualValue);
      if (m_actualValue >= m_bestValue)
      {
        m_bestValue = m_actualValue;
        PlayerPrefs.SetFloat(KEY_PLAYERPREFS, m_bestValue);
        if(m_fireworks != null)
        {
          m_fireworks.SetActive(true);
          AudioEngine.GetInstance().PlayFirworks();
        }
      }
    }
  }

  private void SetText(Text text, float value)
  {
    if(text != null)
    {
      string valueTxt = value.ToString("0.00");
      text.text = valueTxt + " m";
    }
  }

}
