using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace uTalk
{ 
    public abstract class ConverzationCloud : TalkCloudBase
    {
        [SerializeField] private TextMeshProUGUI _talkerNameText;
        [SerializeField] private Image[] _talkerImages;

        protected Image[] TalkerImages => _talkerImages;
        protected TextMeshProUGUI TalkerNameText => _talkerNameText;

        private bool _turnOffOnNoSpriteFound;
         
        //All the extra behaviour should be in another class.
        public override void SetPage(TextPage currentPage, int pageIndex) 
        {
            Clear();

            for (int i = 0; i < currentPage.Sprites.Count; i++)
            {
                if (i < _talkerImages.Length)
                {
                    bool hasSprite = currentPage.Sprites[i];

                    if (_turnOffOnNoSpriteFound && !hasSprite)
                    {
                        _talkerImages[i].enabled = false;
                    }
                    else
                    {
                        _talkerImages[i].enabled = true;
                    }

                    _talkerImages[i].sprite = currentPage.Sprites[i];
                }
            }

            if (_talkerNameText)
            {
                if (string.IsNullOrEmpty(currentPage.TalkerName))
                {
                    _talkerNameText.enabled = false;
                }
                else
                {
                    _talkerNameText.enabled = true;

                    _talkerNameText.text = currentPage.TalkerName;
                }
            }

            base.SetPage(currentPage, pageIndex);
        }

        protected void TurnImageOffWhenNotSpriteIsFound(bool value)
        {
            _turnOffOnNoSpriteFound = value;
        }

        public override void OnCloseCloud()
        {
            Clear();
        }

        private void Clear()
        {
            if (_talkerImages != null)
            {
                for (int i = 0; i < _talkerImages.Length; i++)
                {
                    _talkerImages[i].sprite = default;
                    _talkerImages[i].enabled = false;
                }
            }

            if(_talkerNameText != null)
            {
                _talkerNameText.text = "";
                _talkerNameText.enabled = false;
            }
        }
    }
}
