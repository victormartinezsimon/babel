using UnityEngine;
using System.Collections;
using System;
using DG.Tweening;

public class CameraManager : MonoBehaviour
{
  [Serializable]
  public struct CameraMovementVariables
  {
    public float detectionPoint;
    public float destinationPoint;
  }
  public CameraMovementVariables m_cameraUp;
  public CameraMovementVariables m_cameraDown;

  public GameManager m_manager;
  public float m_timesToCheckPerSecond = 2;

  private float m_minValue;
  private Camera m_camera;


  private struct CameraRealPoints
  {
    public float DetectionPoint;
    public float Increase;
  }
  private CameraRealPoints m_heightUp;
  private CameraRealPoints m_heightDown;

  public float animationDuration = 0.1f;
  private bool InAnimation = false;
  private bool m_cameraChanged = false;

  private float m_timeAcum = 0;
  void Start()
  {
    m_camera = GetComponent<Camera>();
    m_minValue = m_camera.transform.position.y;

    float diffUp = Screen.height * (m_cameraUp.detectionPoint - m_cameraUp.destinationPoint) / 100.0f;

    m_heightUp.Increase = m_camera.ScreenToWorldPoint(new Vector3(0, diffUp, m_camera.nearClipPlane)).y;
    m_heightDown.Increase = m_camera.ScreenToWorldPoint(new Vector3(0, Screen.height * (m_cameraDown.destinationPoint - m_cameraDown.detectionPoint), m_camera.nearClipPlane)).y;

    CalculateDetectionPoints();
  }

  void Update()
  {
    if (InAnimation || m_cameraChanged)
    {
      return;
    }

    float totalTime = 1 / m_timesToCheckPerSecond;
    m_timeAcum += Time.deltaTime;

    if (m_timeAcum >= totalTime)
    {
      m_timeAcum = 0;
      float maxHeight = m_manager.ActualHeight;
      if (maxHeight >= m_heightUp.DetectionPoint)
      {
        MoveCameraUp();
      }

      if (maxHeight <= m_heightDown.DetectionPoint)
      {
        MoveCameraDown();
      }
    }
  }

  private void CalculateDetectionPoints()
  {
    m_heightUp.DetectionPoint = m_camera.ScreenToWorldPoint(new Vector3(0, Screen.height * m_cameraUp.detectionPoint / 100.0f, m_camera.nearClipPlane)).y;
    m_heightDown.DetectionPoint = m_camera.ScreenToWorldPoint(new Vector3(0, Screen.height * m_cameraDown.detectionPoint / 100.0f, m_camera.nearClipPlane)).y;

  }

  [ContextMenu("Move Up")]
  private void MoveCameraUp()
  {
    InAnimation = true;
    m_camera.transform.DOMoveY(transform.position.y + m_heightUp.Increase, animationDuration).SetEase(Ease.OutQuart).OnComplete(() =>
    {
      CalculateDetectionPoints();
      InAnimation = false;
    });
  }

  [ContextMenu("Move Down")]
  private void MoveCameraDown()
  {
    InAnimation = true;
    float destination = Mathf.Max(m_minValue, transform.position.y - m_heightDown.Increase);
    m_camera.transform.DOMoveY(destination, animationDuration).SetEase(Ease.OutQuart).OnComplete(() =>
    {
      CalculateDetectionPoints();
      InAnimation = false;
    });
  }


  public void RecalculateCamera(float limitLeft, float limitRight, float height)
  {
    float actualSize = m_camera.orthographicSize;
    float position = m_camera.transform.position.y;
    Vector3 bottomLeft = m_camera.ScreenToWorldPoint(new Vector3(0, 0, m_camera.nearClipPlane));
    Vector3 topRight = m_camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, m_camera.nearClipPlane));

    float actualWidht = topRight.x - bottomLeft.x;
    float actualHeight = topRight.y - bottomLeft.y;

    float desiredHeight = height - bottomLeft.y;

    float newDesiredSize = desiredHeight * actualSize / actualHeight;

    float maxwidth = limitRight - limitLeft;
    float maxNewSize = maxwidth * actualSize / actualWidht;

    float newSize = Mathf.Max(actualSize, Mathf.Min(newDesiredSize, maxNewSize));
    m_camera.orthographicSize = newSize;
    float newY = newSize * position / actualSize;

    Vector3 pos = m_camera.transform.position;
    pos.y = newY;
    m_camera.transform.position = pos;
    m_cameraChanged = true;
  }

  [ContextMenu("fake")]
  public void FakeCamera()
  {
    RecalculateCamera(-50, 50, 5000);
  }
}
