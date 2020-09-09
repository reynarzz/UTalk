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

    public class CharByCharWriteStyle : IWriteStyle
    {
        private WaitForSeconds _normalSpeed;
        private WaitForSeconds _fastSpeed;

        private WaitForSeconds _writeSpeed;

        private bool _canWrite = false;

        public void Write(MonoBehaviour mono, TextMeshControl control, TextPage page)
        {
            _normalSpeed = new WaitForSeconds(page.CharByCharInfo.NormalWriteSpeed);
            _fastSpeed = new WaitForSeconds(page.CharByCharInfo.FastWriteSpeed);

            _writeSpeed = _normalSpeed;
            _canWrite = true;

            var coroutine = mono.StartCoroutine(WriteByChar(control, page));
        }

        private IEnumerator WriteByChar(TextMeshControl control, TextPage page)
        {
            control.SetText(page.Text);

            //write in the next frame.
            yield return 0;

            var words = Regex.Split(page.Text, " ");

            for (int i = 0; i < words.Length; i++)
            {
                var hightLight = page.Highlight.FirstOrDefault(x => x.Word.ToLower() == words[i].ToLower());

                for (int j = 0; j < words[i].Length; j++)
                {
                    control.ShowChar(i, j, hightLight);

                    yield return _writeSpeed;
                }
            }
        }
    }
}
