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
using System.Threading.Tasks;
using UnityEngine;

namespace TalkSystem.Editor
{
    /// <summary>A hack to get all the properties of the text area.</summary>
    public static class GUIUtils
    {
        private static GUIContent _guiContent;

        static GUIUtils()
        {
            _guiContent = new GUIContent();
        }

        public static TextEditor TextArea(ref string text, params GUILayoutOption[] options)
        {
            return DoTextFieldOrSomething(ref text, true, GUI.skin.textArea, options);
        }

        public static TextEditor TextField(ref string text, params GUILayoutOption[] options)
        {
            return DoTextFieldOrSomething(ref text, false, GUI.skin.textField, options);
        }

        private static TextEditor DoTextFieldOrSomething(ref string text, bool multiline, GUIStyle style, GUILayoutOption[] options)
        {
            int controlID = GUIUtility.GetControlID(FocusType.Keyboard);
            _guiContent.text = text;

            //gUIContent = ((GUIUtility.keyboardControl == controlID) ? _guiContent.text /*+ GUIUtility.compositionString*/ : te);
            Rect rect = GUILayoutUtility.GetRect(_guiContent, style, options);

            //if (GUIUtility.keyboardControl == controlID)
            //{
            //    //gUIContent = Temp(text);
            //}

            var textEditor = DoTextField(rect, controlID, _guiContent, multiline, -1, style);

            text = textEditor.text;

            return textEditor;
        }

        private static TextEditor DoTextField(Rect position, int id, GUIContent content, bool multiline, int maxLength, GUIStyle style)
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
            {
                HandleTextFieldEventForDesktop(position, id, content, multiline, maxLength, style, textEditor);
            }

            textEditor.UpdateScrollOffsetIfNeeded(Event.current);

            return textEditor;
        }

        private static void HandleTextFieldEventForDesktop(Rect position, int id, GUIContent content, bool multiline, int maxLength, GUIStyle style, TextEditor editor)
        {
            Event current = Event.current;
            bool flag = false;
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
                        if (GUIUtility.keyboardControl != id)
                        {
                            return;
                        }
                        if (editor.HandleKeyEvent(current))
                        {
                            current.Use();
                            flag = true;
                            content.text = editor.text;
                            break;
                        }
                        if (current.keyCode == KeyCode.Tab || current.character == '\t')
                        {
                            return;
                        }
                        char character = current.character;
                        if (character == '\n' && !multiline && !current.alt)
                        {
                            return;
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
        }

        internal static object TextArea(ref object text2, GUILayoutOption gUILayoutOption)
        {
            throw new NotImplementedException();
        }
    }
}
