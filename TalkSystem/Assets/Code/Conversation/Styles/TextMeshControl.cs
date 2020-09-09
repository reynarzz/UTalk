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
    public class TextMeshControl
    {
        private readonly TextMeshProUGUI _text;
        private const int _quadPoints= 4;

        public TextMeshControl(TextMeshProUGUI text)
        {
            _text = text;
        }

        public void Write(string text)
        {
            _text.StartCoroutine(Start(text));
        }

        private IEnumerator Start(string text)
        {
            var color = Color.blue;

            var words = Regex.Split(text, " ");
            _text.text = text;

            var textInfo = _text.textInfo;

            yield return 0;

            var colors = _text.mesh.colors;

            while (true)
            {
                if (_text.havePropertiesChanged)
                    _text.ForceMeshUpdate(); 

                // Get # of Words in text object
                int wordCount = textInfo.wordCount;

                if (wordCount == 0)
                {
                    yield return null;
                    continue;
                }

                for (int i = 0; i < words.Length; i++)
                {
                    var wordInfo = textInfo.wordInfo[i];

                    if(words[i] == "text") //test
                    for (int j = 0; j < words[i].Length; j++)
                    {
                        var character = textInfo.characterInfo[wordInfo.firstCharacterIndex];

                        int vertIndex = character.vertexIndex;

                        int quadIndex = j * _quadPoints;

                        colors[vertIndex + 0 + quadIndex] = color;
                        colors[vertIndex + 1 + quadIndex] = color;
                        colors[vertIndex + 2 + quadIndex] = color;
                        colors[vertIndex + 3 + quadIndex] = color;
                    }
                }

                _text.mesh.SetColors(colors);

                _text.canvasRenderer.SetMesh(_text.mesh);

                yield return null;
            }
        }
    }
}