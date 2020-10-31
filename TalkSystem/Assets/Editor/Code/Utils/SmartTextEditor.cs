using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;

namespace TalkSystem.Editor
{
    /// <summary>Captures the events of the TextEditor</summary>
    public class SmartTextEditor : TextEditor
    {
        public event Action<string> OnCopy;
        public event Action<string> OnPaste;
        public event Action<string> OnCut;

        public string SelectedTextLate { get; set; }
        public int SelectIndexLate { get; set; }
        public int CursorIndexLate { get; set; }
        public string TextLate { get; set; }

        public int StartSelectIndexLate
        {
            get
            {
                if (CursorIndexLate < SelectIndexLate)
                {
                    return CursorIndexLate;
                }
                else
                {
                    return SelectIndexLate;
                }
            }
        }

        new public void Copy()
        {
            base.Copy();

            OnCopy?.Invoke(GUIUtility.systemCopyBuffer);
            //Debug.Log(GUIUtility.systemCopyBuffer + " Copied");
        }

        new public bool Paste()
        {
            var paste = base.Paste();

            OnPaste?.Invoke(GUIUtility.systemCopyBuffer);
            //Debug.Log(GUIUtility.systemCopyBuffer + " Paste");

            return paste;
        }

        new public bool Cut()
        {
            var clipped = base.Cut();

            OnCut?.Invoke(GUIUtility.systemCopyBuffer);
            //Debug.Log(GUIUtility.systemCopyBuffer + " clipped");
            
            return clipped;
        }

        new public bool HandleKeyEvent(Event evt)
        {
            var initKeyActions = typeof(TextEditor).GetMethod("InitKeyActions", BindingFlags.Instance | BindingFlags.NonPublic);
            initKeyActions.Invoke(this, null);

            //Get the dictionary that contains the operation enums.
            var dictionaryFieldInfo = typeof(TextEditor).GetField("s_Keyactions", BindingFlags.NonPublic | BindingFlags.Static);

            var dictionary = dictionaryFieldInfo.GetValue(null);

            var findEntry = dictionary.GetType().GetMethod("FindEntry", BindingFlags.Instance | BindingFlags.NonPublic);

            //Inside the dictionary, get the current operation enum.
            var currentEntry = (int)findEntry.Invoke(dictionary, new object[] { evt });

            //If the current entry is less than 0, 'evt' is an unknown operation.
            if (currentEntry >= 0)
            {
                var entriesField = dictionary.GetType().GetField("entries", BindingFlags.Instance | BindingFlags.NonPublic);

                //Entry[] entries array.
                var entries = entriesField.GetValue(dictionary) as IEnumerable;

                var entry = 0;

                foreach (var x in entries)
                {
                    //Get a casted 'TextEditor.TextEditOp' value to int from the struct 'Entry' inside the dictionary.
                    var operationIndex = (int)x.GetType().GetField("value").GetValue(x);

                    //Compare the current operation with the.
                    if (currentEntry == entry)
                    {
                        //Cast to my own 'TextEditOp'
                        var operation = (TextEditOp)operationIndex;
                       
                        PerformOperation(operation, false);
                        break;
                    }

                    entry++;
                }
            }

            return false;
        }


        private bool PerformOperation(TextEditOp operation, bool textIsReadOnly)
        {
            switch (operation)
            {
                case TextEditOp.MoveLeft:
                    MoveLeft();
                    break;
                case TextEditOp.MoveRight:
                    MoveRight();
                    break;
                case TextEditOp.MoveUp:
                    MoveUp();
                    break;
                case TextEditOp.MoveDown:
                    MoveDown();
                    break;
                case TextEditOp.MoveLineStart:
                    MoveLineStart();
                    break;
                case TextEditOp.MoveLineEnd:
                    MoveLineEnd();
                    break;
                case TextEditOp.MoveWordRight:
                    MoveWordRight();
                    break;
                case TextEditOp.MoveToStartOfNextWord:
                    MoveToStartOfNextWord();
                    break;
                case TextEditOp.MoveToEndOfPreviousWord:
                    MoveToEndOfPreviousWord();
                    break;
                case TextEditOp.MoveWordLeft:
                    MoveWordLeft();
                    break;
                case TextEditOp.MoveTextStart:
                    MoveTextStart();
                    break;
                case TextEditOp.MoveTextEnd:
                    MoveTextEnd();
                    break;
                case TextEditOp.MoveParagraphForward:
                    MoveParagraphForward();
                    break;
                case TextEditOp.MoveParagraphBackward:
                    MoveParagraphBackward();
                    break;
                case TextEditOp.MoveGraphicalLineStart:
                    MoveGraphicalLineStart();
                    break;
                case TextEditOp.MoveGraphicalLineEnd:
                    MoveGraphicalLineEnd();
                    break;
                case TextEditOp.SelectLeft:
                    SelectLeft();
                    break;
                case TextEditOp.SelectRight:
                    SelectRight();
                    break;
                case TextEditOp.SelectUp:
                    SelectUp();
                    break;
                case TextEditOp.SelectDown:
                    SelectDown();
                    break;
                case TextEditOp.SelectWordRight:
                    SelectWordRight();
                    break;
                case TextEditOp.SelectWordLeft:
                    SelectWordLeft();
                    break;
                case TextEditOp.SelectToEndOfPreviousWord:
                    SelectToEndOfPreviousWord();
                    break;
                case TextEditOp.SelectToStartOfNextWord:
                    SelectToStartOfNextWord();
                    break;
                case TextEditOp.SelectTextStart:
                    SelectTextStart();
                    break;
                case TextEditOp.SelectTextEnd:
                    SelectTextEnd();
                    break;
                case TextEditOp.ExpandSelectGraphicalLineStart:
                    ExpandSelectGraphicalLineStart();
                    break;
                case TextEditOp.ExpandSelectGraphicalLineEnd:
                    ExpandSelectGraphicalLineEnd();
                    break;
                case TextEditOp.SelectParagraphForward:
                    SelectParagraphForward();
                    break;
                case TextEditOp.SelectParagraphBackward:
                    SelectParagraphBackward();
                    break;
                case TextEditOp.SelectGraphicalLineStart:
                    SelectGraphicalLineStart();
                    break;
                case TextEditOp.SelectGraphicalLineEnd:
                    SelectGraphicalLineEnd();
                    break;
                case TextEditOp.Delete:
                    SelectedTextLate = SelectedText;
                    SelectIndexLate = selectIndex;
                    CursorIndexLate = cursorIndex;
                    TextLate = text;
                    //Debug.Log("Se");
                    if (textIsReadOnly)
                    {
                        return false;
                    }
                    return Delete();
                case TextEditOp.Backspace:
                    SelectedTextLate = SelectedText;
                    SelectIndexLate = selectIndex;
                    CursorIndexLate = cursorIndex;
                    TextLate = text;

                    //Debug.Log("Se");
                    if (textIsReadOnly)
                    {
                        return false;
                    }
                    return Backspace();
                case TextEditOp.Cut:
                    if (textIsReadOnly)
                    {
                        return false;
                    }
                    return this.Cut();

                case TextEditOp.Copy:
                    this.Copy();

                    break;
                case TextEditOp.Paste:
                    if (textIsReadOnly)
                    {
                        return false;
                    }

                    return this.Paste();

                case TextEditOp.SelectAll:
                    SelectAll();
                    break;
                case TextEditOp.SelectNone:
                    SelectNone();
                    break;
                case TextEditOp.DeleteWordBack:
                    if (textIsReadOnly)
                    {
                        return false;
                    }
                    return DeleteWordBack();
                case TextEditOp.DeleteLineBack:
                    if (textIsReadOnly)
                    {
                        return false;
                    }
                    return DeleteLineBack();
                case TextEditOp.DeleteWordForward:
                    if (textIsReadOnly)
                    {
                        return false;
                    }
                    return DeleteWordForward();
                default:
                    Debug.Log("Unimplemented: " + operation);
                    break;
            }
            return false;
        }

        public void Clear()
        {
            OnCopy = null;
            OnCut = null;
            OnPaste = null;
        }

        private enum TextEditOp
        {
            MoveLeft,
            MoveRight,
            MoveUp,
            MoveDown,
            MoveLineStart,
            MoveLineEnd,
            MoveTextStart,
            MoveTextEnd,
            MovePageUp,
            MovePageDown,
            MoveGraphicalLineStart,
            MoveGraphicalLineEnd,
            MoveWordLeft,
            MoveWordRight,
            MoveParagraphForward,
            MoveParagraphBackward,
            MoveToStartOfNextWord,
            MoveToEndOfPreviousWord,
            SelectLeft,
            SelectRight,
            SelectUp,
            SelectDown,
            SelectTextStart,
            SelectTextEnd,
            SelectPageUp,
            SelectPageDown,
            ExpandSelectGraphicalLineStart,
            ExpandSelectGraphicalLineEnd,
            SelectGraphicalLineStart,
            SelectGraphicalLineEnd,
            SelectWordLeft,
            SelectWordRight,
            SelectToEndOfPreviousWord,
            SelectToStartOfNextWord,
            SelectParagraphBackward,
            SelectParagraphForward,
            Delete,
            Backspace,
            DeleteWordBack,
            DeleteWordForward,
            DeleteLineBack,
            Cut,
            Copy,
            Paste,
            SelectAll,
            SelectNone,
            ScrollStart,
            ScrollEnd,
            ScrollPageUp,
            ScrollPageDown
        }
    }
}
