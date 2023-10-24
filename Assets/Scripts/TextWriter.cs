using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextWriter : MonoBehaviour
{
    private static TextWriter instance;

    private List<TextWriterSingle> textWriterSingleList = new List<TextWriterSingle>();

    private void Awake()
    {
        instance = this;
    }

    public static TextWriterSingle AddWriter_Static(TextMeshPro textBox, string textToWrite, float timePerCharacter, bool invisibleCharacters, bool removeWriterFirst, Action onComplete, Action onFinishSentence, Action onStartSentence, Action onStopTalkingSound)
    {
        if(removeWriterFirst)
        {
            instance.RemoveWriter(textBox);
        }
        return instance.AddWriter(textBox, textToWrite, timePerCharacter, invisibleCharacters, onComplete, onFinishSentence, onStartSentence, onStopTalkingSound);
    }
    private TextWriterSingle AddWriter(TextMeshPro textBox, string textToWrite, float timePerCharacter, bool invisibleCharacters, Action onComplete, Action onFinishSentence, Action onStartSentence, Action onStopTalkingSound)
    {
        TextWriterSingle textWriterSingle = new TextWriterSingle(textBox, textToWrite, timePerCharacter, invisibleCharacters, onComplete, onFinishSentence, onStartSentence, onStopTalkingSound);
        textWriterSingleList.Add(textWriterSingle);
        return textWriterSingle;
    }
    private static void RemoveWriter_Static(TextMeshPro textBox)
    {
        instance.RemoveWriter(textBox);
    }

    private void RemoveWriter(TextMeshPro textBox)
    {
        for (int i = 0; i < textWriterSingleList.Count; i++)
        {
            if (textWriterSingleList[i].GetTextMesh() == textBox)
            {
                textWriterSingleList.RemoveAt(i);
                i--;
            }
        }
    }

    private void Update()
    {
        for (int i = 0; i < textWriterSingleList.Count; i++)
        {
            bool destroyInstance = textWriterSingleList[i].Update();

            if(destroyInstance)
            {
                textWriterSingleList.RemoveAt(i);
                i--;
            }
        }
    }
    public class TextWriterSingle
    {
        private TextMeshPro textBox;
        private string textToWrite;
        private int characterIndex;
        private float timePerCharacter;
        private float timer;
        private bool invisibleCharacters;
        private Action onStopTalkingSound;
        private Action onFinishSentence;
        private Action onStartSentence;
        private Action onTextboxFinished;
        private bool currentlyStoppedSounds = false;

        public TextWriterSingle(TextMeshPro textBox, string textToWrite, float timePerCharacter, bool invisibleCharacters, Action onStopTalkingSound, Action onFinishSentence, Action onStartSentence, Action onTextboxFinished)
        {
            this.textBox = textBox;
            this.textToWrite = textToWrite;
            this.timePerCharacter = timePerCharacter;
            this.invisibleCharacters = invisibleCharacters;
            this.onStopTalkingSound = onStopTalkingSound;
            this.onFinishSentence = onFinishSentence;
            this.onStartSentence = onStartSentence;
            this.onTextboxFinished = onTextboxFinished;
            characterIndex = 0;

        }
        public bool Update()
        {
            timer -= Time.deltaTime;
            while (timer <= 0f)
            {
                characterIndex++;
                timer += timePerCharacter;

                string text = textToWrite.Substring(0, characterIndex);

                if (invisibleCharacters)
                {
                    text += "<color=#00000000>" + textToWrite.Substring(characterIndex) + "</color>";
                }
                //Debug.Log(textToWrite[characterIndex]);

                if (characterIndex <= textToWrite.Length)
                {
                    if (textToWrite[characterIndex - 1].ToString() == "." || textToWrite[characterIndex - 1].ToString() == "!" || textToWrite[characterIndex - 1].ToString() == "?")
                    {
                        if (onFinishSentence != null)
                        {
                            onFinishSentence();
                        }
                        timer += timePerCharacter * 6;
                    }
                    else if (textToWrite[characterIndex - 1].ToString() == "," || textToWrite[characterIndex - 1].ToString() == "-")
                    {
                        if (onStopTalkingSound != null)
                        {
                            onStopTalkingSound();
                        }
                        timer += timePerCharacter * 4;
                    }
                    else if (textToWrite[characterIndex -1].ToString() == " " || currentlyStoppedSounds)
                    {
                        onStartSentence();
                    }
                }

                

                textBox.text = text;

                if (characterIndex >= textToWrite.Length)
                {
                    if(onTextboxFinished != null) onTextboxFinished();
                    textBox = null;
                    return true;
                }
            }
            return false;
        }

        public TextMeshPro GetTextMesh() 
        { 
            return textBox; 
        }

        public bool IsActive()
        {
            return characterIndex < textToWrite.Length;
        }

        public void WriteAllAndDestroy()
        {
            textBox.text = textToWrite;
            characterIndex = textToWrite.Length;
            if (onStopTalkingSound != null) onStopTalkingSound();
            TextWriter.RemoveWriter_Static(textBox);
        }

    }
}
