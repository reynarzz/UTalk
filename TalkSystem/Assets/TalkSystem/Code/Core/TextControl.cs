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
        private Color32 _startColor;
        private List<Vector3> _charVertices;
        private List<Color32> _clearColors;

        private Vector2 _textConstAnchoredPosition;

        public struct CharQuad
        {
            public Vector3 BL { get; set; }
            public Vector3 TL { get; set; }
            public Vector3 TR { get; set; }
            public Vector3 BR { get; set; }

            public CharQuad(Vector3 bl, Vector3 tl, Vector3 tr, Vector3 br)
            {
                BL = bl;
                TL = tl;
                TR = tr;
                BR = br;
            }
        }

        public TextControl(TextMeshProUGUI text)
        {
            _text = text;
            _startColor = _text.color;

            _textConstAnchoredPosition = _text.rectTransform.anchoredPosition;

            _text.OnPreRenderText += UpdateHightlight;

            _charVertices = new List<Vector3>();
            _clearColors = new List<Color32>();
        }

        public void SetText(string text)
        {
            ClearColors();

            _text.rectTransform.anchoredPosition = _textConstAnchoredPosition;
            _text.text = text;
        }

        public void ClearColors()
        {
            _clearColors.Clear();

            _text.mesh.GetColors(_clearColors);

            for (int i = 0; i < _clearColors.Count; i++)
            {
                _clearColors[i] = Color.clear;
            }

            _text.mesh.SetColors(_clearColors);

            _text.canvasRenderer.SetMesh(_text.mesh);

            _text.color = Color.clear;//new Color(0,0,0, 1);
        }

        //If you change text position, you will have to update the mesh, because it will disappear!
        private void UpdateHightlight(TMP_TextInfo textInfo)
        {
            //TODO: update mesh.
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

        public void OffsetChar(int charIndex, Vector2 offset)
        {
            var vIndex = _text.textInfo.characterInfo[charIndex].vertexIndex;

            var pos = new Vector3(offset.x, offset.y);

            _charVertices[vIndex + 0] += pos;
            _charVertices[vIndex + 1] += pos;
            _charVertices[vIndex + 2] += pos;
            _charVertices[vIndex + 3] += pos;

            _text.mesh.SetVertices(_charVertices);

            _text.canvasRenderer.SetMesh(_text.mesh);
        }

        public void SetCharPos(int charIndex, CharQuad pos)
        {
            var vIndex = _text.textInfo.characterInfo[charIndex].vertexIndex;

            _charVertices[vIndex + 0] = pos.BL;
            _charVertices[vIndex + 1] = pos.TL;
            _charVertices[vIndex + 2] = pos.TR;
            _charVertices[vIndex + 3] = pos.BR;

            _text.mesh.SetVertices(_charVertices);
            _text.canvasRenderer.SetMesh(_text.mesh);
        }

        public CharQuad GetCharPos(int charIndex)
        {
            var vIndex = _text.textInfo.characterInfo[charIndex].vertexIndex;

            var point1 = _charVertices[vIndex + 0];
            var point2 = _charVertices[vIndex + 1];
            var point3 = _charVertices[vIndex + 2];
            var point4 = _charVertices[vIndex + 3];

            return new CharQuad(point1, point2, point3, point4);
        }

        /// <summary>Call this function after setting the text, and before showing the text.</summary>
        public void ReloadCharsVertices()
        {
            _text.mesh.GetVertices(_charVertices);
        }

        public void OffsetText(Vector2 offset)
        {
            _text.rectTransform.anchoredPosition += offset;
        }

        public CharQuad OffsetVectors(CharQuad charVertices, Vector3 offset)
        {
            charVertices.BL += offset;
            charVertices.TL += offset;
            charVertices.TR += offset;
            charVertices.BR += offset;

            return charVertices;
        }

        public CharQuad LerpCharPos(CharQuad a, CharQuad b, float t)
        {
            a.BL = Vector3.Lerp(a.BL, b.BL, t);
            a.TL = Vector3.Lerp(a.TL, b.TL, t);
            a.TR = Vector3.Lerp(a.TR, b.TR, t);
            a.BR = Vector3.Lerp(a.BR, b.BR, t);

            return a;
        }
    }
}