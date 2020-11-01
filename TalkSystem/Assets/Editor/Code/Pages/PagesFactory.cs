using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalkSystem.Editor;
using UnityEngine;

namespace TalkSystem
{
    public class PagesFactory 
    {
        private Dictionary<Type, IPage> _pages;

        public PagesFactory(PageNavigator navigator)
        {
            _pages = new Dictionary<Type, IPage>()
            {
                { typeof(TalkGroups), new TalkGroups(null, navigator) },
                { typeof(TalksPage), new TalksPage(navigator) },
                { typeof(EditPageText), new EditPageText() },

            };
        }

        public IPage GetPage<T>() where T: IPage
        {
            return _pages[typeof(T)];
        }
    }
}
