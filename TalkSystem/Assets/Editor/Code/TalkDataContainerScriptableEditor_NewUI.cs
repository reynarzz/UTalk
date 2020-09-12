using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TalkSystem;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine.UI;
using UnityEditor.UI;
using System;
using System.Linq;
using System.Text.RegularExpressions;

[CustomEditor(typeof(TalkDataContainerScriptable))]
public class TalkDataContainerScriptableEditor_NewUI : Editor
{
    private VisualElement _rootElement;
    private VisualTreeAsset _visualTree;

    private SerializedProperty _talkDataContainer;
    private VisualElement _talkScroll;

    private void OnEnable()
    {
        _talkDataContainer = serializedObject.FindProperty("_talkDataContainer");
         
        _rootElement = new VisualElement();

        _visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/EditorUI/TalkView.uxml");

        var uss = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/EditorUI/StylesSheets/Home.uss");
        _rootElement.styleSheets.Add(uss);

        // GUILayout.TextArea()
    }
    
    //One time Call
    public override VisualElement CreateInspectorGUI()
    {
        _rootElement.Clear();

        _visualTree.CloneTree(_rootElement);

        var label = _rootElement.Q<Label>("Title");
        var textField = _rootElement.Q<TextField>("TitleField");
        var progressBar = _rootElement.Q<ProgressBar>("ProgBar");

        var talksCount = 20;
        _talkScroll = GetVisualElement("TalkScroll");
        
        //for (int i = 0; i < talksCount; i++)
        //{
        //    var button = new UnityEngine.UIElements.Button();
            
        //    _talkScroll.Add(button);
        //}
        //button.RegisterCallback<ClickEvent>(x =>
        //{
        //    progressBar.value = progressBar.value < 100 ? progressBar.value + 10 : 0;
        //});

        //textField.RegisterValueChangedCallback(x => 
        //{
        //    label.text = textField.text;

        //});

        //textField.RegisterCallback<ClickEvent>(x =>
        //{

        //    var wordSelected = GetSelectedWord(textField.text, textField.cursorIndex);

        //    Debug.Log("Cursor: " + textField.cursorIndex + ", Word: " + wordSelected);

        //});

        //var elements = _rootElement.Children();
         
        //var element = elements.First(x => x.name == "ProceduralElement");

        //var proceduralButton = new UnityEngine.UIElements.Button();
        //proceduralButton.name = "ProceduralButtonsss";

        //proceduralButton.RegisterCallback<ClickEvent>(x =>
        //{
        //    //element.Add();
        //});

        //proceduralButton.text = "ProceduralButtonsss";

        //element.Add(proceduralButton);

        //label.BindProperty(_talkDataContainer);
         
        return _rootElement;
    }

    private VisualElement GetVisualElement(string name)
    {
        return _rootElement.Children().FirstOrDefault(x => x.name == name);
    }

    //very inefficient
    private string GetSelectedWord(string text, int cursor)
    {
        var explit = Regex.Split(text, " |\n");
        var charCount = 0;

        var word = "";

        for (int i = 0; i < explit.Length; i++)
        {
            for (int j = 0; j < explit[i].Length; j++)
            {
                if(charCount == cursor)
                {
                    return explit[i];
                }

                charCount++;
            }

            charCount++;
        }

        return word;
    }
}
