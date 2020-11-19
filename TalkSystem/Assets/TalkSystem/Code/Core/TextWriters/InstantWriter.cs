using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace uTalk
{
    public class InstantWriter : WriterBase
    {
        public override event Action OnPageWritten;

        public InstantWriter(MonoBehaviour mono, TextAnimationControl animationControl) : base(mono, animationControl) { }

        protected override IEnumerator Write(TextControl control, TextPage page, TextAnimationControl animationControl)
        {
            var wordIndex = 0;
            var whiteSpacesCount = -1;
            var highlightedWord = -1;

            for (int i = 0; i < page.Text.Length; i++)
            {
                if (highlightedWord != wordIndex && page.Highlight.ContainsKey(wordIndex))
                {
                    highlightedWord = wordIndex;

                    var highlight = page.Highlight[wordIndex];
                    var highlightLength = highlight.HighlighLength;
                    var startChar = highlight.StartLocalChar;


                    var prevWriteSpeed = WriteSpeedType;

                    //Set default color to chars not colored by the highlight color.
                    for (int j = 0; j < startChar; j++)
                    {
                        var charIndex = i + j;

                        control.ShowChar(charIndex);
                    }

                    i += startChar;

                    for (int j = 0; j < highlightLength; j++)
                    {
                        var charIndex = i + j;

                        control.ShowChar(charIndex, highlight.Color);
                        animationControl.HighlightedChar(charIndex, highlight);

                    }

                    SetWriteTypeSpeed(prevWriteSpeed);

                    var target = i + highlightLength;

                    //When it could be possible to write normal chars in a highlighted word
                    while (page.Text.ElementAtOrDefault(i).IsValidChar())
                    {
                        if (i >= target)
                        {
                            control.ShowChar(i);
                            animationControl.NormalChar(i);
                        }

                        i++;

                        whiteSpacesCount = 0;
                    }
                }
                else
                {
                    //When is not in highlight
                    while (page.Text.ElementAtOrDefault(i).IsValidChar())
                    {
                        control.ShowChar(i);
                        animationControl.NormalChar(i);

                        i++;
                    }

                    //Only if the next char is the start of a word.
                    if (page.Text.ElementAtOrDefault(i + 1).IsValidChar())
                        whiteSpacesCount = 0;
                }

                whiteSpacesCount++;

                if (whiteSpacesCount == 1)
                {
                    wordIndex++;
                }
            }

            OnPageWritten?.Invoke();

            yield break;
        }
    }
}
