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
using TMPro;
using UnityEngine;

namespace TalkSystem
{
    public class TextControl
    {
        private readonly TextMeshProUGUI _text;
        private const int _quadPoints = 4;
        private Color32 _startColor;
        private List<Vector3> _charVertex;

        //refactor this.
        public TextControl(TextMeshProUGUI text)
        {
            _text = text;
            _startColor = _text.color;

            _text.OnPreRenderText += UpdateHightlight;

            _charVertex = new List<Vector3>();
        }

        public void SetText(string text)
        {
            ClearColors();

            _text.text = text;
        }

        public void ClearColors()
        {
            var colors = new List<Color32>();

            _text.mesh.GetColors(colors);

            for (int i = 0; i < colors.Count; i++)
            {
                colors[i] = Color.clear;
            }

            _text.mesh.SetColors(colors);

            _text.canvasRenderer.SetMesh(_text.mesh);

            _text.color = Color.clear;
        }

        private void UpdateHightlight(TMP_TextInfo textInfo)
        {
            //TODO: update mesh.
        }

        public void HighlightWords(string[] words, Highlight[] hightlight)
        {
            var textInfo = _text.textInfo;

            var colors = _text.mesh.colors;

            int wordCount = textInfo.wordCount;

            //for (int i = 0; i < _text.text.Length; i++)
            //{
            //    var wordInfo = textInfo.wordInfo[i];

            //    if (Array.Exists(hightlight, x => x.WordIndex == i))
            //    {
            //        for (int j = 0; j < words[i].Length; j++)
            //        {
            //            var character = textInfo.characterInfo[wordInfo.firstCharacterIndex];

            //            int vertIndex = character.vertexIndex;

            //            int quadIndex = j * _quadPoints;

            //            var color = hightlight[i].Color;

            //            colors[vertIndex + 0 + quadIndex] = color;
            //            colors[vertIndex + 1 + quadIndex] = color;
            //            colors[vertIndex + 2 + quadIndex] = color;
            //            colors[vertIndex + 3 + quadIndex] = color;
            //        }
            //    }
            //}

            //_text.mesh.SetColors(colors);

            //_text.canvasRenderer.SetMesh(_text.mesh);
        }

        public void ShowChar(int wordIndex, int charIndex, Highlight hightlight)
        {
            var textInfo = _text.textInfo;
            var colors = _text.mesh.colors;

            var wordInfo = textInfo.wordInfo[wordIndex];

            var character = textInfo.characterInfo[wordInfo.firstCharacterIndex];

            int vIndex = character.vertexIndex;

            int quadIndex = charIndex * _quadPoints;

            var color = hightlight != default ? hightlight.Color : _startColor;

            //If the text is not enabled, this will throw an error.

            colors[vIndex + 0 + quadIndex] = color;
            colors[vIndex + 1 + quadIndex] = color;
            colors[vIndex + 2 + quadIndex] = color;
            colors[vIndex + 3 + quadIndex] = color;

            _text.mesh.SetColors(colors);

            _text.canvasRenderer.SetMesh(_text.mesh);
        }

        public void ShowChar(int charIndex)
        {
            ShowChar(charIndex, _startColor);
        }

        public void ShowChar(int charIndex, Color32 color)
        {
            var colors = _text.mesh.colors;

            var charInfo = _text.textInfo.characterInfo[charIndex];

            if (!char.IsWhiteSpace(charInfo.character))
            {
                //If the text is not enabled, this will throw an error.
                colors[charInfo.vertexIndex + 0] = color;
                colors[charInfo.vertexIndex + 1] = color;
                colors[charInfo.vertexIndex + 2] = color;
                colors[charInfo.vertexIndex + 3] = color;

                _text.mesh.SetColors(colors);

                _text.canvasRenderer.SetMesh(_text.mesh);
            }
        }

        public void MoveChar(int charIndex, Vector2 offset)
        {
            var charInfo = _text.textInfo.characterInfo[charIndex];

            _text.mesh.GetVertices(_charVertex);

            var vertexIndex = charInfo.vertexIndex;

            _charVertex[vertexIndex + 0] += new Vector3(offset.x, offset.y);
            _charVertex[vertexIndex + 1] += new Vector3(offset.x, offset.y);
            _charVertex[vertexIndex + 2] += new Vector3(offset.x, offset.y);
            _charVertex[vertexIndex + 3] += new Vector3(offset.x, offset.y);

            _text.mesh.SetVertices(_charVertex);
            _text.canvasRenderer.SetMesh(_text.mesh);
        }
    }
}