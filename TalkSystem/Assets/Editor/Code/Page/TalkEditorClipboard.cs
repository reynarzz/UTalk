using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TalkSystem.Editor
{
    //Maybe this has to be inside the text scriptable object, and be cleaned on build.
    public static class TalkEditorClipboard
    {
        private static List<Highlight> _highlightClipboard;

        static TalkEditorClipboard()
        {
            _highlightClipboard = new List<Highlight>();
        }

        public static void SetToClipBoard(TextPage page, string fullText, string copiedText, int selectIndex, int cursor)
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

            for (int i = startCharIndex; i < endCharIndex; i++)
            {
                var value = Utils.GetWordIndexPair(fullText, i);

                var wordIndex = value.Item1;
                var word = value.Item2;

                var index = splitCopiedText.FindIndex(x => x == word);

                if (index >= 0)
                {
                    if (page.Highlight.ContainsKey(wordIndex))
                    {
                        _highlightClipboard.Add(page.Highlight[wordIndex]);
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

        public static void PasteHighlightOfClipboard()
        {
            //TODO
        }

        public static List<Highlight> GetHighlightClipboard()
        {
            return default;
        }
    }
}
