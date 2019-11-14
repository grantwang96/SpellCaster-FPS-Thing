using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Panel that manages all dialogue
/// </summary>
public class DialoguePanel : UIPanel
{
    private const string DialoguePanelPrefabId = "prefab.ui_DialoguePanel";

    // prefab for dialogue bubble
    [SerializeField] private UIDialogueObject _dialoguePrefab;
    [SerializeField] private RectTransform _dialogueParent;
    [SerializeField] private UICustomButton _giantScreenButton;

    public static DialoguePanel Instance { get; private set; }
    private Queue<DialogueInitData> _dialogueQueue = new Queue<DialogueInitData>();

    public event Action OnContinueDialogue;

    private void Awake() {
        _giantScreenButton.onClick.AddListener(ContinueDialogue);
        Instance = this;
    }

    public static void DisplayDialogue(Sprite sprite, string text) {
        if (UIPanelManager.Instance.CurrentPanel != Instance) {
            UIPanelManager.Instance.OpenUIPanel(DialoguePanelPrefabId);
        }
        Instance.EnqueueDialogue(sprite, text);
    }

    protected void EnqueueDialogue(Sprite sprite, string text) {
        _dialogueQueue.Enqueue(
            new DialogueInitData() {
                Text = text,
                Image = sprite
            }    
        );
        ActivateDialogue(_dialogueQueue.Peek());
    }

    protected override void SubscribeToGameplayController() {
        base.SubscribeToGameplayController();

        PlayerController.Instance.OnSubmitPressed += ContinueDialogue;
    }

    protected override void UnsubscribeToGameplayController() {
        base.UnsubscribeToGameplayController();

        PlayerController.Instance.OnSubmitPressed -= ContinueDialogue;
    }

    protected override void MenuBtnAction() {
        ContinueDialogue();
    }

    private void ContinueDialogue() {
        _dialogueQueue.Dequeue();
        OnContinueDialogue?.Invoke();
        if (_dialogueQueue.Count != 0) {
            ActivateDialogue(_dialogueQueue.Peek());
            return;
        }
        DeactivateDialogue();
        ClosePanel();
    }

    private void ActivateDialogue(DialogueInitData data) {
        _dialoguePrefab.Display();
        _dialoguePrefab.SetDialogue(data.Image, data.Text);
    }

    private void DeactivateDialogue() {
        _dialoguePrefab.Hide();
    }

    public void ClearDialogueQueue() {
        _dialogueQueue.Clear();
        _dialoguePrefab.Hide();
    }
}

public class DialogueInitData {
    public string Text;
    public Sprite Image;
}