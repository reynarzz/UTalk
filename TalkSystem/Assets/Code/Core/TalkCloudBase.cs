using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace TalkSystem
{
    public abstract class TalkCloudBase : MonoBehaviour
    {
        [SerializeField] protected TextMeshProUGUI _text;

        private TextControl _texControl;
        public TextControl TextControl => _texControl;

        public abstract event Action OnCloudShown;
        public abstract event Action OnCloudHidden;

        protected virtual void Awake()
        {
            _texControl = new TextControl(_text);
        }

        public abstract void ShowCloud();
        public abstract void CloseCloud();

        public abstract void Clear();
    }
}
