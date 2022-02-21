using UnityEditor;

using UnityEngine;

using System;


namespace TMPro.Examples
{
  public class TMP_TextInfoDebugTool : MonoBehaviour
  {
    // Since this script is used for debugging, we exclude it from builds.
    // TODO: Rework this script to make it into an editor utility.
  #if UNITY_EDITOR
    public bool ShowCharacters;

    public bool ShowWords;

    public bool ShowLinks;

    public bool ShowLines;

    public bool ShowMeshBounds;

    public bool ShowTextBounds;

    [Space(10)]
    [TextArea(2, 2)]
    public string ObjectStats;

    [SerializeField]
    private TMP_Text m_TextComponent;

    private Transform m_Transform;

    private TMP_TextInfo m_TextInfo;

    private float m_ScaleMultiplier;

    private float m_HandleSize;

    private void OnDrawGizmos()
    {
      if (this.m_TextComponent == null)
      {
        this.m_TextComponent = this.GetComponent<TMP_Text>();

        if (this.m_TextComponent == null)
        {
          return;
        }
      }

      this.m_Transform = this.m_TextComponent.transform;

      // Get a reference to the text object's textInfo
      this.m_TextInfo = this.m_TextComponent.textInfo;

      // Update Text Statistics
      this.ObjectStats =
              "Characters: "                 +
              this.m_TextInfo.characterCount +
              "   Words: "                   +
              this.m_TextInfo.wordCount      +
              "   Spaces: "                  +
              this.m_TextInfo.spaceCount     +
              "   Sprites: "                 +
              this.m_TextInfo.spriteCount    +
              "   Links: "                   +
              this.m_TextInfo.linkCount      +
              "\nLines: "                    +
              this.m_TextInfo.lineCount      +
              "   Pages: "                   +
              this.m_TextInfo.pageCount;

      // Get the handle size for drawing the various
      this.m_ScaleMultiplier = this.m_TextComponent.GetType() == typeof(TextMeshPro) ? 1 : 0.1f;
      this.m_HandleSize = HandleUtility.GetHandleSize(this.m_Transform.position) * this.m_ScaleMultiplier;

      // Draw line metrics

    #region Draw Lines

      if (this.ShowLines)
      {
        this.DrawLineBounds();
      }

    #endregion

      // Draw word metrics

    #region Draw Words

      if (this.ShowWords)
      {
        this.DrawWordBounds();
      }

    #endregion

      // Draw character metrics

    #region Draw Characters

      if (this.ShowCharacters)
      {
        this.DrawCharactersBounds();
      }

    #endregion

      // Draw Quads around each of the words

    #region Draw Links

      if (this.ShowLinks)
      {
        this.DrawLinkBounds();
      }

    #endregion

      // Draw Quad around the bounds of the text

    #region Draw Bounds

      if (this.ShowMeshBounds)
      {
        this.DrawBounds();
      }

    #endregion

      // Draw Quad around the rendered region of the text.

    #region Draw Text Bounds

      if (this.ShowTextBounds)
      {
        this.DrawTextBounds();
      }

    #endregion
    }

    /// <summary>
    ///   Method to draw a rectangle around each character.
    /// </summary>
    /// <param name="text"></param>
    private void DrawCharactersBounds()
    {
      var characterCount = this.m_TextInfo.characterCount;

      for (var i = 0; i < characterCount; i++)
      {
        // Draw visible as well as invisible characters
        var characterInfo = this.m_TextInfo.characterInfo[i];

        var isCharacterVisible = i                        < this.m_TextComponent.maxVisibleCharacters &&
                                 characterInfo.lineNumber < this.m_TextComponent.maxVisibleLines      &&
                                 i                        >= this.m_TextComponent.firstVisibleCharacter;

        if (this.m_TextComponent.overflowMode == TextOverflowModes.Page)
        {
          isCharacterVisible =
                  isCharacterVisible && characterInfo.pageNumber + 1 == this.m_TextComponent.pageToDisplay;
        }

        if (!isCharacterVisible)
        {
          continue;
        }

        float dottedLineSize = 6;

        // Get Bottom Left and Top Right position of the current character
        var bottomLeft = this.m_Transform.TransformPoint(characterInfo.bottomLeft);
        var topLeft = this.m_Transform.TransformPoint(new(characterInfo.topLeft.x, characterInfo.topLeft.y, 0));
        var topRight = this.m_Transform.TransformPoint(characterInfo.topRight);

        var bottomRight =
                this.m_Transform.TransformPoint(new(characterInfo.bottomRight.x, characterInfo.bottomRight.y, 0));

        // Draw character bounds
        if (characterInfo.isVisible)
        {
          var color = Color.green;
          this.DrawDottedRectangle(bottomLeft, topRight, color);
        }
        else
        {
          var color = Color.grey;

          var whiteSpaceAdvance =
                  Math.Abs(characterInfo.origin - characterInfo.xAdvance) > 0.01f
                          ? characterInfo.xAdvance
                          : characterInfo.origin + ((characterInfo.ascender - characterInfo.descender) * 0.03f);

          this.DrawDottedRectangle(
                                   this.m_Transform.TransformPoint(
                                                                   new(characterInfo.origin, characterInfo.descender, 0)
                                                                  ),
                                   this.m_Transform.TransformPoint(new(whiteSpaceAdvance, characterInfo.ascender, 0)),
                                   color,
                                   4
                                  );
        }

        var origin = characterInfo.origin;
        var advance = characterInfo.xAdvance;
        var ascentline = characterInfo.ascender;
        var baseline = characterInfo.baseLine;
        var descentline = characterInfo.descender;

        //Draw Ascent line
        var ascentlineStart = this.m_Transform.TransformPoint(new(origin, ascentline, 0));
        var ascentlineEnd = this.m_Transform.TransformPoint(new(advance, ascentline, 0));

        Handles.color = Color.cyan;
        Handles.DrawDottedLine(ascentlineStart, ascentlineEnd, dottedLineSize);

        // Draw Cap Height & Mean line
        var capline =
                characterInfo.fontAsset == null
                        ? 0
                        : baseline + (characterInfo.fontAsset.faceInfo.capLine * characterInfo.scale);

        var capHeightStart =
                new Vector3(topLeft.x, this.m_Transform.TransformPoint(new(0, capline, 0)).y, 0);

        var capHeightEnd =
                new Vector3(topRight.x, this.m_Transform.TransformPoint(new(0, capline, 0)).y, 0);

        var meanline =
                characterInfo.fontAsset == null
                        ? 0
                        : baseline + (characterInfo.fontAsset.faceInfo.meanLine * characterInfo.scale);

        var meanlineStart =
                new Vector3(topLeft.x, this.m_Transform.TransformPoint(new(0, meanline, 0)).y, 0);

        var meanlineEnd =
                new Vector3(topRight.x, this.m_Transform.TransformPoint(new(0, meanline, 0)).y, 0);

        if (characterInfo.isVisible)
        {
          // Cap line
          Handles.color = Color.cyan;
          Handles.DrawDottedLine(capHeightStart, capHeightEnd, dottedLineSize);

          // Mean line
          Handles.color = Color.cyan;
          Handles.DrawDottedLine(meanlineStart, meanlineEnd, dottedLineSize);
        }

        //Draw Base line
        var baselineStart = this.m_Transform.TransformPoint(new(origin, baseline, 0));
        var baselineEnd = this.m_Transform.TransformPoint(new(advance, baseline, 0));

        Handles.color = Color.cyan;
        Handles.DrawDottedLine(baselineStart, baselineEnd, dottedLineSize);

        //Draw Descent line
        var descentlineStart = this.m_Transform.TransformPoint(new(origin, descentline, 0));
        var descentlineEnd = this.m_Transform.TransformPoint(new(advance, descentline, 0));

        Handles.color = Color.cyan;
        Handles.DrawDottedLine(descentlineStart, descentlineEnd, dottedLineSize);

        // Draw Origin
        var originPosition = this.m_Transform.TransformPoint(new(origin, baseline, 0));
        this.DrawCrosshair(originPosition, 0.05f / this.m_ScaleMultiplier, Color.cyan);

        // Draw Horizontal Advance
        var advancePosition = this.m_Transform.TransformPoint(new(advance, baseline, 0));
        this.DrawSquare(advancePosition, 0.025f     / this.m_ScaleMultiplier, Color.yellow);
        this.DrawCrosshair(advancePosition, 0.0125f / this.m_ScaleMultiplier, Color.yellow);

        // Draw text labels for metrics
        if (this.m_HandleSize < 0.5f)
        {
          var style = new GUIStyle(GUI.skin.GetStyle("Label"));
          style.normal.textColor = new(0.6f, 0.6f, 0.6f, 1.0f);
          style.fontSize = 12;
          style.fixedWidth = 200;
          style.fixedHeight = 20;

          Vector3 labelPosition;
          var     center = (origin + advance) / 2;

          //float baselineMetrics = 0;
          //float ascentlineMetrics = ascentline - baseline;
          //float caplineMetrics = capline - baseline;
          //float meanlineMetrics = meanline - baseline;
          //float descentlineMetrics = descentline - baseline;

          // Ascent Line
          labelPosition = this.m_Transform.TransformPoint(new(center, ascentline, 0));
          style.alignment = TextAnchor.UpperCenter;
          Handles.Label(labelPosition, "Ascent Line", style);

          //Handles.Label(labelPosition, "Ascent Line (" + ascentlineMetrics.ToString("f3") + ")" , style);

          // Base Line
          labelPosition = this.m_Transform.TransformPoint(new(center, baseline, 0));
          Handles.Label(labelPosition, "Base Line", style);

          //Handles.Label(labelPosition, "Base Line (" + baselineMetrics.ToString("f3") + ")" , style);

          // Descent line
          labelPosition = this.m_Transform.TransformPoint(new(center, descentline, 0));
          Handles.Label(labelPosition, "Descent Line", style);

          //Handles.Label(labelPosition, "Descent Line (" + descentlineMetrics.ToString("f3") + ")" , style);

          if (characterInfo.isVisible)
          {
            // Cap Line
            labelPosition = this.m_Transform.TransformPoint(new(center, capline, 0));
            style.alignment = TextAnchor.UpperCenter;
            Handles.Label(labelPosition, "Cap Line", style);

            //Handles.Label(labelPosition, "Cap Line (" + caplineMetrics.ToString("f3") + ")" , style);

            // Mean Line
            labelPosition = this.m_Transform.TransformPoint(new(center, meanline, 0));
            style.alignment = TextAnchor.UpperCenter;
            Handles.Label(labelPosition, "Mean Line", style);

            //Handles.Label(labelPosition, "Mean Line (" + ascentlineMetrics.ToString("f3") + ")" , style);

            // Origin
            labelPosition = this.m_Transform.TransformPoint(new(origin, baseline, 0));
            style.alignment = TextAnchor.UpperRight;
            Handles.Label(labelPosition, "Origin ", style);

            // Advance
            labelPosition = this.m_Transform.TransformPoint(new(advance, baseline, 0));
            style.alignment = TextAnchor.UpperLeft;
            Handles.Label(labelPosition, "  Advance", style);
          }
        }
      }
    }

    /// <summary>
    ///   Method to draw rectangles around each word of the text.
    /// </summary>
    /// <param name="text"></param>
    private void DrawWordBounds()
    {
      for (var i = 0; i < this.m_TextInfo.wordCount; i++)
      {
        var wInfo = this.m_TextInfo.wordInfo[i];

        var isBeginRegion = false;

        var bottomLeft = Vector3.zero;
        var topLeft = Vector3.zero;
        var bottomRight = Vector3.zero;
        var topRight = Vector3.zero;

        var maxAscender = -Mathf.Infinity;
        var minDescender = Mathf.Infinity;

        var wordColor = Color.green;

        // Iterate through each character of the word
        for (var j = 0; j < wInfo.characterCount; j++)
        {
          var characterIndex = wInfo.firstCharacterIndex + j;
          var currentCharInfo = this.m_TextInfo.characterInfo[characterIndex];
          var currentLine = currentCharInfo.lineNumber;

          var isCharacterVisible = characterIndex             > this.m_TextComponent.maxVisibleCharacters ||
                                   currentCharInfo.lineNumber > this.m_TextComponent.maxVisibleLines      ||
                                   (this.m_TextComponent.overflowMode == TextOverflowModes.Page &&
                                    currentCharInfo.pageNumber + 1    != this.m_TextComponent.pageToDisplay)
                                           ? false
                                           : true;

          // Track Max Ascender and Min Descender
          maxAscender = Mathf.Max(maxAscender, currentCharInfo.ascender);
          minDescender = Mathf.Min(minDescender, currentCharInfo.descender);

          if (isBeginRegion == false && isCharacterVisible)
          {
            isBeginRegion = true;

            bottomLeft = new(currentCharInfo.bottomLeft.x, currentCharInfo.descender, 0);
            topLeft = new(currentCharInfo.bottomLeft.x, currentCharInfo.ascender, 0);

            //Debug.Log("Start Word Region at [" + currentCharInfo.character + "]");

            // If Word is one character
            if (wInfo.characterCount == 1)
            {
              isBeginRegion = false;

              topLeft = this.m_Transform.TransformPoint(new(topLeft.x, maxAscender, 0));
              bottomLeft = this.m_Transform.TransformPoint(new(bottomLeft.x, minDescender, 0));
              bottomRight = this.m_Transform.TransformPoint(new(currentCharInfo.topRight.x, minDescender, 0));
              topRight = this.m_Transform.TransformPoint(new(currentCharInfo.topRight.x, maxAscender, 0));

              // Draw Region
              this.DrawRectangle(bottomLeft, topLeft, topRight, bottomRight, wordColor);

              //Debug.Log("End Word Region at [" + currentCharInfo.character + "]");
            }
          }

          // Last Character of Word
          if (isBeginRegion && j == wInfo.characterCount - 1)
          {
            isBeginRegion = false;

            topLeft = this.m_Transform.TransformPoint(new(topLeft.x, maxAscender, 0));
            bottomLeft = this.m_Transform.TransformPoint(new(bottomLeft.x, minDescender, 0));
            bottomRight = this.m_Transform.TransformPoint(new(currentCharInfo.topRight.x, minDescender, 0));
            topRight = this.m_Transform.TransformPoint(new(currentCharInfo.topRight.x, maxAscender, 0));

            // Draw Region
            this.DrawRectangle(bottomLeft, topLeft, topRight, bottomRight, wordColor);

            //Debug.Log("End Word Region at [" + currentCharInfo.character + "]");
          }

          // If Word is split on more than one line.
          else if (isBeginRegion && currentLine != this.m_TextInfo.characterInfo[characterIndex + 1].lineNumber)
          {
            isBeginRegion = false;

            topLeft = this.m_Transform.TransformPoint(new(topLeft.x, maxAscender, 0));
            bottomLeft = this.m_Transform.TransformPoint(new(bottomLeft.x, minDescender, 0));
            bottomRight = this.m_Transform.TransformPoint(new(currentCharInfo.topRight.x, minDescender, 0));
            topRight = this.m_Transform.TransformPoint(new(currentCharInfo.topRight.x, maxAscender, 0));

            // Draw Region
            this.DrawRectangle(bottomLeft, topLeft, topRight, bottomRight, wordColor);

            //Debug.Log("End Word Region at [" + currentCharInfo.character + "]");
            maxAscender = -Mathf.Infinity;
            minDescender = Mathf.Infinity;
          }
        }

        //Debug.Log(wInfo.GetWord(m_TextMeshPro.textInfo.characterInfo));
      }
    }

    /// <summary>
    ///   Draw rectangle around each of the links contained in the text.
    /// </summary>
    /// <param name="text"></param>
    private void DrawLinkBounds()
    {
      var textInfo = this.m_TextComponent.textInfo;

      for (var i = 0; i < textInfo.linkCount; i++)
      {
        var linkInfo = textInfo.linkInfo[i];

        var isBeginRegion = false;

        var bottomLeft = Vector3.zero;
        var topLeft = Vector3.zero;
        var bottomRight = Vector3.zero;
        var topRight = Vector3.zero;

        var maxAscender = -Mathf.Infinity;
        var minDescender = Mathf.Infinity;

        Color32 linkColor = Color.cyan;

        // Iterate through each character of the link text
        for (var j = 0; j < linkInfo.linkTextLength; j++)
        {
          var characterIndex = linkInfo.linkTextfirstCharacterIndex + j;
          var currentCharInfo = textInfo.characterInfo[characterIndex];
          var currentLine = currentCharInfo.lineNumber;

          var isCharacterVisible = characterIndex             > this.m_TextComponent.maxVisibleCharacters ||
                                   currentCharInfo.lineNumber > this.m_TextComponent.maxVisibleLines      ||
                                   (this.m_TextComponent.overflowMode == TextOverflowModes.Page &&
                                    currentCharInfo.pageNumber + 1    != this.m_TextComponent.pageToDisplay)
                                           ? false
                                           : true;

          // Track Max Ascender and Min Descender
          maxAscender = Mathf.Max(maxAscender, currentCharInfo.ascender);
          minDescender = Mathf.Min(minDescender, currentCharInfo.descender);

          if (isBeginRegion == false && isCharacterVisible)
          {
            isBeginRegion = true;

            bottomLeft = new(currentCharInfo.bottomLeft.x, currentCharInfo.descender, 0);
            topLeft = new(currentCharInfo.bottomLeft.x, currentCharInfo.ascender, 0);

            //Debug.Log("Start Word Region at [" + currentCharInfo.character + "]");

            // If Link is one character
            if (linkInfo.linkTextLength == 1)
            {
              isBeginRegion = false;

              topLeft = this.m_Transform.TransformPoint(new(topLeft.x, maxAscender, 0));
              bottomLeft = this.m_Transform.TransformPoint(new(bottomLeft.x, minDescender, 0));
              bottomRight = this.m_Transform.TransformPoint(new(currentCharInfo.topRight.x, minDescender, 0));
              topRight = this.m_Transform.TransformPoint(new(currentCharInfo.topRight.x, maxAscender, 0));

              // Draw Region
              this.DrawRectangle(bottomLeft, topLeft, topRight, bottomRight, linkColor);

              //Debug.Log("End Word Region at [" + currentCharInfo.character + "]");
            }
          }

          // Last Character of Link
          if (isBeginRegion && j == linkInfo.linkTextLength - 1)
          {
            isBeginRegion = false;

            topLeft = this.m_Transform.TransformPoint(new(topLeft.x, maxAscender, 0));
            bottomLeft = this.m_Transform.TransformPoint(new(bottomLeft.x, minDescender, 0));
            bottomRight = this.m_Transform.TransformPoint(new(currentCharInfo.topRight.x, minDescender, 0));
            topRight = this.m_Transform.TransformPoint(new(currentCharInfo.topRight.x, maxAscender, 0));

            // Draw Region
            this.DrawRectangle(bottomLeft, topLeft, topRight, bottomRight, linkColor);

            //Debug.Log("End Word Region at [" + currentCharInfo.character + "]");
          }

          // If Link is split on more than one line.
          else if (isBeginRegion && currentLine != textInfo.characterInfo[characterIndex + 1].lineNumber)
          {
            isBeginRegion = false;

            topLeft = this.m_Transform.TransformPoint(new(topLeft.x, maxAscender, 0));
            bottomLeft = this.m_Transform.TransformPoint(new(bottomLeft.x, minDescender, 0));
            bottomRight = this.m_Transform.TransformPoint(new(currentCharInfo.topRight.x, minDescender, 0));
            topRight = this.m_Transform.TransformPoint(new(currentCharInfo.topRight.x, maxAscender, 0));

            // Draw Region
            this.DrawRectangle(bottomLeft, topLeft, topRight, bottomRight, linkColor);

            maxAscender = -Mathf.Infinity;
            minDescender = Mathf.Infinity;

            //Debug.Log("End Word Region at [" + currentCharInfo.character + "]");
          }
        }

        //Debug.Log(wInfo.GetWord(m_TextMeshPro.textInfo.characterInfo));
      }
    }

    /// <summary>
    ///   Draw Rectangles around each lines of the text.
    /// </summary>
    /// <param name="text"></param>
    private void DrawLineBounds()
    {
      var lineCount = this.m_TextInfo.lineCount;

      for (var i = 0; i < lineCount; i++)
      {
        var lineInfo = this.m_TextInfo.lineInfo[i];
        var firstCharacterInfo = this.m_TextInfo.characterInfo[lineInfo.firstCharacterIndex];
        var lastCharacterInfo = this.m_TextInfo.characterInfo[lineInfo.lastCharacterIndex];

        var isLineVisible =
                (lineInfo.characterCount == 1 &&
                 (firstCharacterInfo.character == 10     ||
                  firstCharacterInfo.character == 11     ||
                  firstCharacterInfo.character == 0x2028 ||
                  firstCharacterInfo.character == 0x2029)) ||
                i > this.m_TextComponent.maxVisibleLines   ||
                (this.m_TextComponent.overflowMode == TextOverflowModes.Page &&
                 firstCharacterInfo.pageNumber + 1 != this.m_TextComponent.pageToDisplay)
                        ? false
                        : true;

        if (!isLineVisible)
        {
          continue;
        }

        var lineBottomLeft = firstCharacterInfo.bottomLeft.x;
        var lineTopRight = lastCharacterInfo.topRight.x;

        var ascentline = lineInfo.ascender;
        var baseline = lineInfo.baseline;
        var descentline = lineInfo.descender;

        float dottedLineSize = 12;

        // Draw line extents
        this.DrawDottedRectangle(
                                 this.m_Transform.TransformPoint(lineInfo.lineExtents.min),
                                 this.m_Transform.TransformPoint(lineInfo.lineExtents.max),
                                 Color.green,
                                 4
                                );

        // Draw Ascent line
        var ascentlineStart = this.m_Transform.TransformPoint(new(lineBottomLeft, ascentline, 0));
        var ascentlineEnd = this.m_Transform.TransformPoint(new(lineTopRight, ascentline, 0));

        Handles.color = Color.yellow;
        Handles.DrawDottedLine(ascentlineStart, ascentlineEnd, dottedLineSize);

        // Draw Base line
        var baseLineStart = this.m_Transform.TransformPoint(new(lineBottomLeft, baseline, 0));
        var baseLineEnd = this.m_Transform.TransformPoint(new(lineTopRight, baseline, 0));

        Handles.color = Color.yellow;
        Handles.DrawDottedLine(baseLineStart, baseLineEnd, dottedLineSize);

        // Draw Descent line
        var descentLineStart = this.m_Transform.TransformPoint(new(lineBottomLeft, descentline, 0));
        var descentLineEnd = this.m_Transform.TransformPoint(new(lineTopRight, descentline, 0));

        Handles.color = Color.yellow;
        Handles.DrawDottedLine(descentLineStart, descentLineEnd, dottedLineSize);

        // Draw text labels for metrics
        if (this.m_HandleSize < 1.0f)
        {
          var style = new GUIStyle();
          style.normal.textColor = new(0.8f, 0.8f, 0.8f, 1.0f);
          style.fontSize = 12;
          style.fixedWidth = 200;
          style.fixedHeight = 20;
          Vector3 labelPosition;

          // Ascent Line
          labelPosition = this.m_Transform.TransformPoint(new(lineBottomLeft, ascentline, 0));
          style.padding = new(0, 10, 0, 5);
          style.alignment = TextAnchor.MiddleRight;
          Handles.Label(labelPosition, "Ascent Line", style);

          // Base Line
          labelPosition = this.m_Transform.TransformPoint(new(lineBottomLeft, baseline, 0));
          Handles.Label(labelPosition, "Base Line", style);

          // Descent line
          labelPosition = this.m_Transform.TransformPoint(new(lineBottomLeft, descentline, 0));
          Handles.Label(labelPosition, "Descent Line", style);
        }
      }
    }

    /// <summary>
    ///   Draw Rectangle around the bounds of the text object.
    /// </summary>
    private void DrawBounds()
    {
      var meshBounds = this.m_TextComponent.bounds;

      // Get Bottom Left and Top Right position of each word
      var bottomLeft = this.m_TextComponent.transform.position + meshBounds.min;
      var topRight = this.m_TextComponent.transform.position + meshBounds.max;

      this.DrawRectangle(bottomLeft, topRight, new(1, 0.5f, 0));
    }

    private void DrawTextBounds()
    {
      var textBounds = this.m_TextComponent.textBounds;

      var bottomLeft = this.m_TextComponent.transform.position + (textBounds.center - textBounds.extents);
      var topRight = this.m_TextComponent.transform.position + (textBounds.center + textBounds.extents);

      this.DrawRectangle(bottomLeft, topRight, new(0f, 0.5f, 0.5f));
    }

    // Draw Rectangles
    private void DrawRectangle(Vector3 BL, Vector3 TR, Color color)
    {
      Gizmos.color = color;

      Gizmos.DrawLine(new(BL.x, BL.y, 0), new(BL.x, TR.y, 0));
      Gizmos.DrawLine(new(BL.x, TR.y, 0), new(TR.x, TR.y, 0));
      Gizmos.DrawLine(new(TR.x, TR.y, 0), new(TR.x, BL.y, 0));
      Gizmos.DrawLine(new(TR.x, BL.y, 0), new(BL.x, BL.y, 0));
    }

    private void DrawDottedRectangle(Vector3 bottomLeft, Vector3 topRight, Color color, float size = 5.0f)
    {
      Handles.color = color;
      Handles.DrawDottedLine(bottomLeft, new Vector3(bottomLeft.x, topRight.y, bottomLeft.z), size);
      Handles.DrawDottedLine(new Vector3(bottomLeft.x,             topRight.y, bottomLeft.z), topRight, size);
      Handles.DrawDottedLine(topRight, new Vector3(topRight.x, bottomLeft.y, bottomLeft.z), size);
      Handles.DrawDottedLine(new Vector3(topRight.x, bottomLeft.y, bottomLeft.z), bottomLeft, size);
    }

    private void DrawSolidRectangle(Vector3 bottomLeft, Vector3 topRight, Color color, float size = 5.0f)
    {
      Handles.color = color;
      var rect = new Rect(bottomLeft, topRight - bottomLeft);
      Handles.DrawSolidRectangleWithOutline(rect, color, Color.black);
    }

    private void DrawSquare(Vector3 position, float size, Color color)
    {
      Handles.color = color;
      var bottomLeft = new Vector3(position.x - size, position.y - size, position.z);
      var topLeft = new Vector3(position.x - size, position.y + size, position.z);
      var topRight = new Vector3(position.x + size, position.y + size, position.z);
      var bottomRight = new Vector3(position.x + size, position.y - size, position.z);

      Handles.DrawLine(bottomLeft,  topLeft);
      Handles.DrawLine(topLeft,     topRight);
      Handles.DrawLine(topRight,    bottomRight);
      Handles.DrawLine(bottomRight, bottomLeft);
    }

    private void DrawCrosshair(Vector3 position, float size, Color color)
    {
      Handles.color = color;

      Handles.DrawLine(
                       new Vector3(position.x - size, position.y, position.z),
                       new Vector3(position.x + size, position.y, position.z)
                      );

      Handles.DrawLine(
                       new Vector3(position.x, position.y - size, position.z),
                       new Vector3(position.x, position.y + size, position.z)
                      );
    }

    // Draw Rectangles
    private void DrawRectangle(Vector3 bl, Vector3 tl, Vector3 tr, Vector3 br, Color color)
    {
      Gizmos.color = color;

      Gizmos.DrawLine(bl, tl);
      Gizmos.DrawLine(tl, tr);
      Gizmos.DrawLine(tr, br);
      Gizmos.DrawLine(br, bl);
    }

    // Draw Rectangles
    private void DrawDottedRectangle(Vector3 bl, Vector3 tl, Vector3 tr, Vector3 br, Color color)
    {
      var cam = Camera.current;
      var dotSpacing = (cam.WorldToScreenPoint(br).x - cam.WorldToScreenPoint(bl).x) / 75f;
      Handles.color = color;

      Handles.DrawDottedLine(bl, tl, dotSpacing);
      Handles.DrawDottedLine(tl, tr, dotSpacing);
      Handles.DrawDottedLine(tr, br, dotSpacing);
      Handles.DrawDottedLine(br, bl, dotSpacing);
    }
  #endif
  }
}
