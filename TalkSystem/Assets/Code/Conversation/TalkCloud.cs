using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

namespace TalkSystem
{
    public class TalkCloud : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        private TextMeshControl _meshControl;

        private void Start()
        {
            Init();
        }

        public void Init()
        {
            _meshControl = new TextMeshControl(_text);
            _meshControl.Write("This is a text");
        }

        public Mesh Mesh => _text.mesh;

        public void SetText(string text)
        {
            _text.text = text;
        }
    }
}
