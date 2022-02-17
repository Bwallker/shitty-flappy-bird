﻿using UnityEngine;


namespace TMPro.Examples
{
  public class TMP_TextEventCheck : MonoBehaviour
  {
    public TMP_TextEventHandler TextEventHandler;

    private TMP_Text m_TextComponent;

    private void OnEnable()
    {
      if (this.TextEventHandler != null)
      {
        // Get a reference to the text component
        this.m_TextComponent = this.TextEventHandler.GetComponent<TMP_Text>();

        this.TextEventHandler.onCharacterSelection.AddListener(this.OnCharacterSelection);
        this.TextEventHandler.onSpriteSelection.AddListener(this.OnSpriteSelection);
        this.TextEventHandler.onWordSelection.AddListener(this.OnWordSelection);
        this.TextEventHandler.onLineSelection.AddListener(this.OnLineSelection);
        this.TextEventHandler.onLinkSelection.AddListener(this.OnLinkSelection);
      }
    }

    private void OnDisable()
    {
      if (this.TextEventHandler != null)
      {
        this.TextEventHandler.onCharacterSelection.RemoveListener(this.OnCharacterSelection);
        this.TextEventHandler.onSpriteSelection.RemoveListener(this.OnSpriteSelection);
        this.TextEventHandler.onWordSelection.RemoveListener(this.OnWordSelection);
        this.TextEventHandler.onLineSelection.RemoveListener(this.OnLineSelection);
        this.TextEventHandler.onLinkSelection.RemoveListener(this.OnLinkSelection);
      }
    }

    private void OnCharacterSelection(char c, int index)
    {
      Debug.Log("Character [" + c + "] at Index: " + index + " has been selected.");
    }

    private void OnSpriteSelection(char c, int index)
    {
      Debug.Log("Sprite [" + c + "] at Index: " + index + " has been selected.");
    }

    private void OnWordSelection(string word, int firstCharacterIndex, int length)
    {
      Debug.Log(
                "Word ["                           +
                word                               +
                "] with first character index of " +
                firstCharacterIndex                +
                " and length of "                  +
                length                             +
                " has been selected."
               );
    }

    private void OnLineSelection(string lineText, int firstCharacterIndex, int length)
    {
      Debug.Log(
                "Line ["                           +
                lineText                           +
                "] with first character index of " +
                firstCharacterIndex                +
                " and length of "                  +
                length                             +
                " has been selected."
               );
    }

    private void OnLinkSelection(string linkID, string linkText, int linkIndex)
    {
      if (this.m_TextComponent != null)
      {
        var linkInfo = this.m_TextComponent.textInfo.linkInfo[linkIndex];
      }

      Debug.Log(
                "Link Index: "  +
                linkIndex       +
                " with ID ["    +
                linkID          +
                "] and Text \"" +
                linkText        +
                "\" has been selected."
               );
    }
  }
}
