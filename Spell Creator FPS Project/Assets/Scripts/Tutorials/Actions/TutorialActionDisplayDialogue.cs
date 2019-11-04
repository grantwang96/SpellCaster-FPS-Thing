using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tutorials/Actions/DisplayDialogue")]
public class TutorialActionDisplayDialogue : TutorialAction
{
    [SerializeField] private Sprite _headImage;
    [SerializeField] private string _text;

    public override TutorialActionStatus Execute() {

        DialoguePanel.DisplayDialogue(_headImage, _text);
        DialoguePanel.Instance.OnContinueDialogue += OnContinueDialogue;

        return TutorialActionStatus.Incomplete;
    }

    private void OnContinueDialogue() {
        TutorialActionCompleted();
        DialoguePanel.Instance.OnContinueDialogue -= OnContinueDialogue;
    }
}
