using UnityEngine;


namespace TMPro.Examples
{
  public class TMP_FrameRateCounter : MonoBehaviour
  {
    public enum FpsCounterAnchorPositions
    {
      TopLeft, BottomLeft, TopRight,

      BottomRight,
    }

    private const string fpsLabel = "{0:2}</color> <#8080ff>FPS \n<#FF8000>{1:2} <#8080ff>MS";

    public float UpdateInterval = 5.0f;

    public FpsCounterAnchorPositions AnchorPosition = FpsCounterAnchorPositions.TopRight;

    private string htmlColorTag;

    private FpsCounterAnchorPositions last_AnchorPosition;

    private Camera m_camera;

    private Transform m_frameCounter_transform;

    private int m_Frames;

    private float m_LastInterval;

    private TextMeshPro m_TextMeshPro;

    private void Awake()
    {
      if (!this.enabled)
      {
        return;
      }

      this.m_camera               = Camera.main;
      Application.targetFrameRate = 9999;

      var frameCounter = new GameObject("Frame Counter");

      this.m_TextMeshPro      = frameCounter.AddComponent<TextMeshPro>();
      this.m_TextMeshPro.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");

      this.m_TextMeshPro.fontSharedMaterial =
              Resources.Load<Material>("Fonts & Materials/LiberationSans SDF - Overlay");

      this.m_frameCounter_transform = frameCounter.transform;
      this.m_frameCounter_transform.SetParent(this.m_camera.transform);
      this.m_frameCounter_transform.localRotation = Quaternion.identity;

      this.m_TextMeshPro.enableWordWrapping = false;
      this.m_TextMeshPro.fontSize           = 24;

      //m_TextMeshPro.FontColor = new Color32(255, 255, 255, 128);
      //m_TextMeshPro.edgeWidth = .15f;
      //m_TextMeshPro.isOverlay = true;

      //m_TextMeshPro.FaceColor = new Color32(255, 128, 0, 0);
      //m_TextMeshPro.EdgeColor = new Color32(0, 255, 0, 255);
      //m_TextMeshPro.FontMaterial.renderQueue = 4000;

      //m_TextMeshPro.CreateSoftShadowClone(new Vector2(1f, -1f));

      this.Set_FrameCounter_Position(this.AnchorPosition);
      this.last_AnchorPosition = this.AnchorPosition;
    }

    private void Start()
    {
      this.m_LastInterval = Time.realtimeSinceStartup;
      this.m_Frames       = 0;
    }

    private void Update()
    {
      if (this.AnchorPosition != this.last_AnchorPosition)
      {
        this.Set_FrameCounter_Position(this.AnchorPosition);
      }

      this.last_AnchorPosition = this.AnchorPosition;

      this.m_Frames += 1;
      var timeNow = Time.realtimeSinceStartup;

      if (timeNow > this.m_LastInterval + this.UpdateInterval)
      {
        // display two fractional digits (f2 format)
        var fps = this.m_Frames / (timeNow - this.m_LastInterval);
        var ms  = 1000.0f       / Mathf.Max(fps, 0.00001f);

        if (fps < 30)
        {
          this.htmlColorTag = "<color=yellow>";
        }
        else if (fps < 10)
        {
          this.htmlColorTag = "<color=red>";
        }
        else
        {
          this.htmlColorTag = "<color=green>";
        }

        //string format = System.String.Format(htmlColorTag + "{0:F2} </color>FPS \n{1:F2} <#8080ff>MS",fps, ms);
        //m_TextMeshPro.text = format;

        this.m_TextMeshPro.SetText(this.htmlColorTag + TMP_FrameRateCounter.fpsLabel, fps, ms);

        this.m_Frames       = 0;
        this.m_LastInterval = timeNow;
      }
    }

    private void Set_FrameCounter_Position(FpsCounterAnchorPositions anchor_position)
    {
      //Debug.Log("Changing frame counter anchor position.");
      this.m_TextMeshPro.margin = new(1f, 1f, 1f, 1f);

      switch (anchor_position)
      {
        case FpsCounterAnchorPositions.TopLeft:
          this.m_TextMeshPro.alignment           = TextAlignmentOptions.TopLeft;
          this.m_TextMeshPro.rectTransform.pivot = new(0, 1);
          this.m_frameCounter_transform.position = this.m_camera.ViewportToWorldPoint(new(0, 1, 100.0f));

          break;

        case FpsCounterAnchorPositions.BottomLeft:
          this.m_TextMeshPro.alignment           = TextAlignmentOptions.BottomLeft;
          this.m_TextMeshPro.rectTransform.pivot = new(0, 0);
          this.m_frameCounter_transform.position = this.m_camera.ViewportToWorldPoint(new(0, 0, 100.0f));

          break;

        case FpsCounterAnchorPositions.TopRight:
          this.m_TextMeshPro.alignment           = TextAlignmentOptions.TopRight;
          this.m_TextMeshPro.rectTransform.pivot = new(1, 1);
          this.m_frameCounter_transform.position = this.m_camera.ViewportToWorldPoint(new(1, 1, 100.0f));

          break;

        case FpsCounterAnchorPositions.BottomRight:
          this.m_TextMeshPro.alignment           = TextAlignmentOptions.BottomRight;
          this.m_TextMeshPro.rectTransform.pivot = new(1, 0);
          this.m_frameCounter_transform.position = this.m_camera.ViewportToWorldPoint(new(1, 0, 100.0f));

          break;
      }
    }
  }
}
