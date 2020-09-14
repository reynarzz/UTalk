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


        public void Init()
        {
            if(_texControl == null)
            {
                _texControl = new TextControl(_text);
            }

            Clear();
        }

        public abstract void ShowCloud();
        public abstract void CloseCloud();
        protected abstract void Clear();
    }
}
