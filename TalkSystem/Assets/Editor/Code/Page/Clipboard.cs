using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TalkSystem.Editor
{
    public class Clipboard
    {
        private List<Highlight> _highlightClipboard;

        public Clipboard()
        {
            _highlightClipboard = new List<Highlight>();
        }

        public void SetToClipBoard(TextPage page, string fullText, string copiedText, int selectIndex, int cursor)
        {
            int startCharIndex;
            int endCharIndex;

            _highlightClipboard.Clear();

            if (cursor > selectIndex)
            {
                startCharIndex = selectIndex;
                endCharIndex = cursor;
            }
            else
            {
                startCharIndex = cursor;
                endCharIndex = selectIndex;
            }

            var splitCopiedText = copiedText.Split(Utils.SplitPattern, StringSplitOptions.RemoveEmptyEntries).ToList();
            splitCopiedText.Print();

            Debug.Log(fullText + ", -startchar: " + startCharIndex + ", End: " + endCharIndex);

            for (int i = startCharIndex; i < endCharIndex; i++)
            {
                var value = Highlight.GetWordIndex(fullText, i);

                var wordIndex = value.Item1;
                var word = value.Item2;

                var index = splitCopiedText.FindIndex(x => x == word);

                if (index >= 0)
                {
                    if (page.Highlight.ContainsKey(wordIndex))
                    {
                        _highlightClipboard.Add(page.Highlight[wordIndex]);
                        // Debug.Log("Clipboard: " + splitCopiedText[index]);
                    }
                    else
                    {
                        //Debug.Log("Don't contains: " + splitCopiedText[index]);
                    }

                    splitCopiedText.RemoveAt(index);
                }
                //else 
                //{
                //    Debug.Log("Nothing found");
                //}
            }
        }

        public void PasteHighlightOfClipboard()
        {
            //TODO
        }

        public List<Highlight> GetHighlightClipboard()
        {
            return default;
        }
    }
}
