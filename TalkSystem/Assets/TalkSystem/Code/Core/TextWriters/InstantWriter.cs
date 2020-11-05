using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TalkSystem
{
    public class InstantWriter : WriterBase
    {
        public override event Action OnPageWriten;

        public InstantWriter(MonoBehaviour mono, TextAnimationControl textAnimationControl) : base(mono, textAnimationControl) { }

        protected override IEnumerator Write(TextControl control, TextPage page, TextAnimationControl textAnimationControl)
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

                    i += startChar;

                    for (int j = 0; j < highlightLength; j++)
                    {
                        control.ShowChar(i + j, highlight.Color);
                        textAnimationControl.HighlightedChar(i + j, highlight);
                    }

                    var target = i + highlightLength;

                    //When is could be possible to write normal chars in a highlighted word
                    while (page.Text.ElementAtOrDefault(i).IsValidChar())
                    {
                        if (i >= target)
                        {
                            control.ShowChar(i);
                            textAnimationControl.NormalChar(i);
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
                        textAnimationControl.NormalChar(i);

                        i++;

                        whiteSpacesCount = 0;
                    }
                }

                whiteSpacesCount++;

                if (whiteSpacesCount == 1)
                {
                    wordIndex++;
                }
            }

            OnPageWriten?.Invoke();

            yield return null;
        }
    }
}
