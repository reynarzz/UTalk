//MIT License

//Copyright (c) 2020 Reynardo Perez (Reynarz)

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace TalkSystem
{
    [Serializable]
    public struct CharByCharInfo
    {
        [SerializeField] private float _normalWriteSpeed;
        [SerializeField] private float _fastWriteSpeed;
        [SerializeField] private float _highlightedWordSpeed;

        public float NormalWriteSpeed => _normalWriteSpeed;
        public float FastWriteSpeed => _fastWriteSpeed;
        public float HighlightedWordSpeed => _highlightedWordSpeed;
    }

    public class CharByCharWriter : IWriter
    {
        private WaitForSeconds _normalSpeed;
        private WaitForSeconds _fastSpeed;

        private WaitForSeconds _writeSpeed;
        private readonly MonoBehaviour _mono;

        private readonly List<string> _words;

        public event Action OnPageWriten;
        public CharByCharWriter(MonoBehaviour mono)
        {
            _mono = mono;

            _words = new List<string>();
        }

        public void Write(TextControl control, TextPage page)
        {
            control.SetText(page.Text);

            _normalSpeed = new WaitForSeconds(page.CharByCharInfo.NormalWriteSpeed);
            _fastSpeed = new WaitForSeconds(page.CharByCharInfo.FastWriteSpeed);

            _writeSpeed = _normalSpeed;

            _mono.StartCoroutine(WriteByChar(control, page));
        }

        private IEnumerator WriteByChar(TextControl control, TextPage page)
        {
            SplitWords(page.Text, _words);

            //show chars in the next frame.
            yield return 0;

            var charIndex = 0;

            for (int i = 0; i < _words.Count; i++)
            {
                var hasKey = page.Highlight.ContainsKey(i);

                if (hasKey)
                {
                    var highlight = page.Highlight[i];

                    for (int j = 0; j < _words[i].Length; j++)
                    {
                        if (j >= highlight.StartLocalChar && j <= highlight.HighlighLength)
                        {
                            control.ShowChar(charIndex, highlight.Color);
                        }
                        else
                        {
                            control.ShowChar(charIndex);
                        }

                        charIndex++;
                        yield return _writeSpeed;
                    }
                }
                else
                {
                    for (int j = 0; j < _words[i].Length; j++)
                    {
                        control.ShowChar(charIndex);
                        charIndex++;

                        yield return _writeSpeed;
                    }
                }

                charIndex++;
            }

            //var i = 0;

            //var applyColor = false;
            //var highlightIndex = 0;
            //var coloredCharCount = 0;

            //while (i < page.Text.Length)
            //{
            //    if (page.Highlight.ContainsKey(i))
            //    {
            //        applyColor = true;
            //        highlightIndex = i;
            //        coloredCharCount = 0;
            //    }

            //    if (applyColor)
            //    {
            //        var highlight = page.Highlight[highlightIndex];

            //        //FIX
            //        applyColor = coloredCharCount++ < highlight.HighlighLength;

            //        if (applyColor)
            //        {
            //            control.ShowChar(i, highlight.Color);
            //        }
            //        else
            //        {
            //            control.ShowChar(i);
            //        }
            //    }
            //    else
            //    {
            //        control.ShowChar(i);
            //    }

            //    i++;

            //    yield return _writeSpeed;
            //}

            OnPageWriten?.Invoke();
        }

        private StringBuilder _string = new StringBuilder();

        private void SplitWords(string text, List<string> words)
        {
            words.Clear();

            _string.Clear();

            for (int i = 0; i < text.Length; i++)
            {
                if (!char.IsWhiteSpace(text[i]) && text[i] != '\n')
                {
                    _string.Append(text[i]);
                }
                else
                {
                    words.Add(_string.ToString());

                    _string.Clear();
                }
            }
        }

        public void OnLanguageChanged(TextPage textPage)
        {

        }

        public void Clear(TextControl control)
        {
            control.ClearColors();
        }
    }
}