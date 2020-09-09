using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TalkSystem
{
    public enum WriteType
    {
        CharByChar,
        Fade,
        CharByCharAnimated
    }

    public enum HighlightType
    {
        None,
        VerticalShake
    }

    public enum Language 
    {
        English,
        French,
        Spanish
        //Others...
    }

    [Serializable]
    public struct WordEvent
    {
        [SerializeField] private string _word;
        public string Word => _word;

        public WordEvent(string word)
        {
            _word = word;
        }
    }

    [Serializable]
    public struct TextPage
    {
        [SerializeField] private string _pageText;
        [SerializeField] private Sprite _sprite;
        [SerializeField] private Highlight[] _highlight;
        [SerializeField] private WordEvent _wordEvent;

        public string Text => _pageText;
        public Sprite Sprite => _sprite;
        public Highlight[] Highlight => _highlight;
        public WordEvent Event => _wordEvent;

        public static TextPage Error => new TextPage("Invalid Page", null, default, TalkSystem.Highlight.Error);

        public TextPage(string text, Sprite sprite, WordEvent wEvent, params Highlight[] highlights)
        {
            _pageText = text;
            _sprite = sprite;
            _wordEvent = wEvent;
            _highlight = highlights;
        }

        public static bool operator ==(TextPage a, TextPage b)
        {
            return a.Text == b.Text && a.Highlight == b.Highlight && a.Sprite == b.Sprite;
        }

        public static bool operator !=(TextPage a, TextPage b)
        {
            return a.Text != b.Text || a.Highlight != b.Highlight || a.Sprite != b.Sprite;
        }
    }

    [Serializable]
    public struct Highlight
    {
        [SerializeField] private string _words;
        [SerializeField] private Color32 _color;
        [SerializeField] private HighlightType _type;

        public string Words => _words;
        public Color32 Color => _color;
        public HighlightType Type => _type;

        public Highlight(string words, Color32 color, HighlightType type)
        {
            _words = words;
            _color = color;
            _type = type;
        }

        public static Highlight Error => new Highlight("Invalid Page", new Color32(255, 0, 0, 1), HighlightType.VerticalShake);
    }

    [CreateAssetMenu]
    public class TalkAsset : ScriptableObject
    {
        [SerializeField, HideInInspector] private Language _language;
        [SerializeField] private TextPage[] _pages;

        public Language Language => _language;
        public int PagesCount => _pages.Length;

        public TextPage GetPage(int pageIndex)
        {
            var page = _pages.ElementAtOrDefault(pageIndex);

            if(page == default)
            {
                page = TextPage.Error;
            }

            return page;
        }
    }
}
