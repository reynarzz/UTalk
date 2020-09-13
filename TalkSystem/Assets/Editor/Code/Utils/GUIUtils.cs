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
using NUnit.Framework.Constraints;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TalkSystem.Editor
{
    /// <summary>A hack to get all the properties of the text area.</summary>
    public static class GUIUtils
    {
        private static GUIContent _guiContent;

        private static int _cursorIndex;

        public struct TextEditorInfo
        {
            private string _text;

            private int _cursorIndex;
            private bool _lengthChanged;
            private int _addedChars;

            public int CursorIndex => _cursorIndex;

            public string Text => _text;

            public bool TextLengthChanged => _lengthChanged;
            public int AddedChars => _addedChars;

            public TextEditorInfo(string text, int cursorIndex, int addedChars, bool textLengthChanged)
            {
                _text = text;
                _cursorIndex = cursorIndex;
                _addedChars = addedChars;

                _lengthChanged = textLengthChanged;
            }
        }

        static GUIUtils()
        {
            _guiContent = new GUIContent();
        }

        public static TextEditorInfo TextArea(ref string text, params GUILayoutOption[] options)
        {
            if (text == null)
            {
                text = "";
            }

            return DoTextFieldOrSomething(ref text, true, GUI.skin.textArea, options);
        }

        public static TextEditorInfo TextField(ref string text, params GUILayoutOption[] options)
        {
            if (text == null)
            {
                text = "";
            }

            return DoTextFieldOrSomething(ref text, false, GUI.skin.textField, options);
        }

        //TODO: Copy paste doesn't work correctly. 
        private static TextEditorInfo DoTextFieldOrSomething(ref string text, bool multiline, GUIStyle style, GUILayoutOption[] options)
        {
            int controlID = GUIUtility.GetControlID(FocusType.Keyboard);

            _guiContent.text = text;

            //gUIContent = ((GUIUtility.keyboardControl == controlID) ? _guiContent.text /*+ GUIUtility.compositionString*/ : te);
            var rect = GUILayoutUtility.GetRect(_guiContent, style, options);

            var textEditor = DoTextField(rect, controlID, _guiContent, multiline, -1, style);

            text = textEditor.Text;

            return textEditor;
        }

        private static TextEditorInfo DoTextField(Rect position, int id, GUIContent content, bool multiline, int maxLength, GUIStyle style)
        {
            //GUIUtility.CheckOnGUI(); //If is called from OnGUI method

            if (maxLength >= 0 && content.text.Length > maxLength)
            {
                content.text = content.text.Substring(0, maxLength);
            }

            var textEditor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), id);

            textEditor.text = content.text;
            textEditor.SaveBackup();
            textEditor.position = position;
            textEditor.style = style;
            textEditor.multiline = multiline;
            textEditor.controlID = id;
            textEditor.DetectFocusChange();

            //if (TouchScreenKeyboard.isSupported && !TouchScreenKeyboard.isInPlaceEditingAllowed)
            //{
            //    HandleTextFieldEventForTouchscreen(position, id, content, multiline, maxLength, style, secureText, maskChar, textEditor);
            //}
            //else

            var charsAdded = HandleTextFieldEventForDesktop(position, id, content, multiline, maxLength, style, textEditor);

            textEditor.UpdateScrollOffsetIfNeeded(Event.current);

            return new TextEditorInfo(textEditor.text, _cursorIndex, charsAdded, charsAdded != 0); ;
        }

        private static int HandleTextFieldEventForDesktop(Rect position, int id, GUIContent content, bool multiline, int maxLength, GUIStyle style, TextEditor editor)
        {
            Event current = Event.current;
            bool flag = false;
            var charsAdded = 0;

            _cursorIndex = editor.cursorIndex;

            switch (current.type)
            {
                case EventType.MouseDown:
                    if (position.Contains(current.mousePosition))
                    {
                        GUIUtility.hotControl = id;
                        GUIUtility.keyboardControl = id;
                        //editor.m_HasFocus = true;
                        editor.MoveCursorToPosition(Event.current.mousePosition);
                        if (Event.current.clickCount == 2 && GUI.skin.settings.doubleClickSelectsWord)
                        {
                            editor.SelectCurrentWord();
                            editor.DblClickSnap(TextEditor.DblClickSnapping.WORDS);
                            editor.MouseDragSelectsWholeWords(on: true);
                        }
                        if (Event.current.clickCount == 3 && GUI.skin.settings.tripleClickSelectsLine)
                        {
                            editor.SelectCurrentParagraph();
                            editor.MouseDragSelectsWholeWords(on: true);
                            editor.DblClickSnap(TextEditor.DblClickSnapping.PARAGRAPHS);
                        }
                        current.Use();
                    }
                    break;
                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == id)
                    {
                        if (current.shift)
                        {
                            editor.MoveCursorToPosition(Event.current.mousePosition);
                        }
                        else
                        {
                            editor.SelectToPosition(Event.current.mousePosition);
                        }

                        current.Use();
                    }
                    break;
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == id)
                    {
                        editor.MouseDragSelectsWholeWords(on: false);
                        GUIUtility.hotControl = 0;
                        current.Use();
                    }
                    break;
                case EventType.KeyDown:
                    {
                        var originalTextLength = content.text.Length;
                        if (GUIUtility.keyboardControl != id)
                        {
                            return charsAdded;
                        }



                        if (editor.HandleKeyEvent(current))
                        {
                            current.Use();

                            flag = true;
                            content.text = editor.text;

                            if (editor.text.Length != originalTextLength)
                            {
                                if (editor.text.Length < originalTextLength)
                                {
                                    var deleted = originalTextLength - editor.text.Length;

                                    charsAdded = -deleted;

                                    //Debug.Log($"{-deleted} added, was: " + (_cursorIndex + deleted) + ", is: " + _cursorIndex);
                                }
                                else
                                {
                                    charsAdded = editor.text.Length - originalTextLength;

                                    //Debug.Log("add: " + charAdded);
                                }
                            }

                            break;
                        }
                        if (current.keyCode == KeyCode.Tab || current.character == '\t')
                        {

                            return 0;
                        }

                        char character = current.character;

                        if (character == '\n' && !multiline && !current.alt)
                        {
                            return charsAdded;
                        }

                        Font font = style.font;

                        if (!font)
                        {
                            font = GUI.skin.font;
                        }

                        if (font.HasCharacter(character) || character == '\n')
                        {
                            editor.Insert(character);

                            flag = true;

                            charsAdded++;

                            //Debug.Log("add " + charAdded);
                        }
                        else if (character == '\0') //if is null char
                        {
                            //if (GUIUtility.compositionString.Length > 0)
                            {
                                editor.ReplaceSelection("");
                                flag = true;
                            }
                            current.Use();
                        }

                        if (editor.text.Length != originalTextLength)
                        {
                            if (editor.text.Length < originalTextLength)
                            {
                                var deleted = originalTextLength - editor.text.Length;

                                charsAdded = -deleted;

                                //Debug.Log($"{-deleted} added, was: " + (_cursorIndex + deleted) + ", is: " + _cursorIndex);
                            }
                        }
                        break;
                    }
                case EventType.Repaint:
                    if (GUIUtility.keyboardControl != id)
                    {
                        style.Draw(position, content, id, on: false);
                    }
                    else
                    {
                        editor.DrawCursor(content.text);
                    }

                    break;


            }
            if (GUIUtility.keyboardControl == id)
            {
                //GUIUtility.textFieldInput = true;
            }
            if (flag)
            {
                //changed = true;
                content.text = editor.text;
                if (maxLength >= 0 && content.text.Length > maxLength)
                {
                    content.text = content.text.Substring(0, maxLength);
                }
                current.Use();
            }

            _cursorIndex = editor.cursorIndex;

            return charsAdded;
        }
    }
}
