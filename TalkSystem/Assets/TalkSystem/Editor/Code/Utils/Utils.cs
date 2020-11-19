using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace uTalk.Editor
{
    public static class Utils
    {
        private static StringBuilder _printArray;
        public static char[] SplitPattern = { ' ', '\n' };
        public static char[] SplitPatternNotNewline = { ' ', '\n' };
        private const int _whiteSpace = 1;
        //private const char EmptyChar = '\0';
        private static List<WordInfo> _selectedWords;

        static Utils()
        {
            _printArray = new StringBuilder();
            _selectedWords = new List<WordInfo>();
        }

        public static void Print(this IEnumerable collection, string title = null)
        {
            _printArray.Clear();

            _printArray.Append("{ ");

            foreach (var item in collection)
            {
                _printArray.Append(item.ToString() + ", ");
            }

            _printArray.Remove(_printArray.Length - 2, 1);
            _printArray.Append("}");

            Debug.Log(_printArray.ToString());
        }

        /// <summary>If you want to do an exact search, you have to choose which char test agains to.</summary>
        public enum SearchCharType
        {
            None,
            Left, Right,
        }

        public static (int, string) GetWordIndexPair(string text, int charIndex, SearchCharType type = SearchCharType.None)
        {
            var wIndex = GetWordIndex(text, charIndex, type);
            //Debug.Log(wIndex);
            return (wIndex, text.Split(SplitPattern, StringSplitOptions.RemoveEmptyEntries).ElementAtOrDefault(wIndex));
        }

        public static string GetWord(int wordIndex, string fullText)
        {
           return fullText.Split(SplitPattern, StringSplitOptions.RemoveEmptyEntries).ElementAtOrDefault(wordIndex);
        }
        //A word is a string or char between white spaces or new lines or null chars
        public static int GetWordIndex(string text, int charIndex, SearchCharType type = SearchCharType.None)
        {
            var wIndex = 0;
            var whiteSpacesCount = -1;
            var exit = false;

            for (int i = 0; i < text.Length; i++)
            {
                if (charIndex == i)
                    break;

                while (text.ElementAtOrDefault(i).IsValidChar())
                {
                    i++;
                    whiteSpacesCount = 0;

                    if (charIndex == i)
                        exit = true;
                }

                if (exit)
                {
                    break;
                }

                whiteSpacesCount++;

                if (whiteSpacesCount == 1)
                {
                    wIndex++;
                }
                else
                {
                    whiteSpacesCount = -1;
                }
            }

            if (type != SearchCharType.None)
            {
                var value = type == SearchCharType.Right ? 1 : -1;

                if (!text.ElementAtOrDefault(charIndex + value).IsValidChar())
                {
                    wIndex = -1;
                }
            }

            return wIndex;
        }

        /// <summary>Is a letter, a number or a symbol?</summary>
        /// <returns></returns>
        public static bool IsValidChar(this char character)
        {
            return character != default &&
                   character != ' ' &&
                   character != '\n';
        }

        /// <summary>Takes in consideration white spaces.</summary>
        public static int GetStartingCharIndexRaw(string text, int wordIndex)
        {
            var charIndex = 0;
            var currentWordIndex = 0;

            for (int i = 0; i < text.Length; i++)
            {
                var isCurrentWhiteSpaceOrNewLine = text[i] == ' ' || text[i] == '\n';
                var isNextAWord = text.ElementAtOrDefault(i + 1) != ' ' && text.ElementAtOrDefault(i + 1) != '\n';

                if (currentWordIndex != wordIndex)
                {
                    if (isCurrentWhiteSpaceOrNewLine && isNextAWord)
                    {
                        currentWordIndex++;
                    }
                }
                else
                {
                    break;
                }

                charIndex++;
            }

            return charIndex;
        }

        public struct WordInfo
        {
            public int GlobalCharIndex { get; set; }
            public int WordIndex { get; set; }
            public string Word { get; set; }

            public override string ToString()
            {
                return $"Char I: {GlobalCharIndex}, WordIndex: {WordIndex}, Word: {Word} ";
            }
        }
        //I have to optimise this.
        public static List<WordInfo> GetSelectedWords(int startSelectionIndex, string selectedText, string fullText)
        {
            _selectedWords.Clear();

            var charIndex = 0;

            var selectedSplitted = selectedText.Split(SplitPattern, StringSplitOptions.RemoveEmptyEntries);
             
            for (int i = 0; i < selectedSplitted.Length; i++)
            {
                for (int j = startSelectionIndex; j < startSelectionIndex + selectedText.Length; j++)
                {
                    if (fullText[j].IsValidChar())
                    {
                        charIndex = j;

                        while (fullText.ElementAtOrDefault(j).IsValidChar())
                        {
                            j++;
                        }

                        startSelectionIndex = j;
                        break;
                    }
                }

                var pair = GetWordIndexPair(fullText, charIndex);

                var info = new WordInfo()
                {
                    GlobalCharIndex = charIndex,
                    WordIndex = pair.Item1,
                    Word = pair.Item2
                };

                _selectedWords.Add(info);
            }

            return _selectedWords;
        }

        public static int ToLocalStartChar(int globalStartCharIndex, string fullText, string word)
        {
            return word.IndexOf(fullText[globalStartCharIndex]);
        }

        //public static string ToLocalTextSelected(int globalStartCharIndex, string globalSelected, string word)
        //{

        //}

        public static int GetChangedWordsCount(string current, string compare)
        {
            var currentSplit = current.Split(SplitPattern, StringSplitOptions.RemoveEmptyEntries);
            var compareSplit = compare.Split(SplitPattern, StringSplitOptions.RemoveEmptyEntries);

            return Mathf.Abs(currentSplit.Length - compareSplit.Length);
        }
          
        public static TalkDataContainerScriptable GetTalkScriptable()
        {
            var paths = AssetDatabase.GetAllAssetPaths();

            var data = default(TalkDataContainerScriptable);

;           for (int i = 0; i < paths.Length; i++)
            {
                if (paths[i].Contains("TalkSystem/Scriptables") && paths[i].EndsWith(".asset"))
                {
                    data = AssetDatabase.LoadAssetAtPath<TalkDataContainerScriptable>(paths[i]);
                    //Debug.Log(paths[i]);

                    break;
                }
                else if (i + 1 == paths.Length)
                {
                    Debug.Log("Doesn't exist, create one");
                    //Create a new one and return it
                }
            }

            return data;
        }
    }
}
