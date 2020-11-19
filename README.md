# uTalk
A talk system that highlights words in all your text very easy.

## How it looks?
Let's choose some colors for all the important words!

![](ReadmeFiles/TalkCloudDemo2.gif)

## Simple to use
Just a few method calls and you will be ready to show some awesome text!
```c#
using uTalk;

namespace MyProject
{
    public class Example : MonoBehaviour
    {
	[SerializeField] private TalkCloudBase _talkCloud;
        
	private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!UTalk.Inst.IsTalking)
                {
		    var talkInfo = new TalkInfo("Group", "SubGroup", "TalkName", "Language");

                    UTalk.Inst.StartTalk(_talkCloud, talkInfo);
                }
                else
                {
                    UTalk.Inst.NextPage();
                }
            }
        }
    }
}
```

## But, that's it?
This is just starting, These are some of the features that will be added soon.

- [x] Localization Support
- [ ] A Robust Editor.
   - [x] Write and hightlight the words.
   - [ ] Import/Export talk files.
- [x] Animations to the words.

## Running ver?
2020.1.9f1, but newer versions could work as well.