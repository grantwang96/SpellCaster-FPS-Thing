using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tutorials/Actions/PointInteractableTarget")]
public class TutorialActionPointInteractableTarget : TutorialAction {

    [SerializeField] private string _targetId; // the target you want to point at
    [SerializeField] private bool _onInteractSuccess; // does the clear action require success?
    // prefab that will point out interactable object in world
    [SerializeField] private InteractableMarker _interactableMarkerPrefab;

    public override TutorialActionStatus Execute() {
        IInteractable interactable = LevelManager.LevelManagerInstance.GetInteractable(_targetId);
        if(interactable == null) {
            Debug.LogError($"[{nameof(TutorialActionPointInteractableTarget)}] Could not find interactable for ID {_targetId}");
            return TutorialActionStatus.Abort;
        }
        // spawn pointer object
        InteractableMarker marker = Instantiate(_interactableMarkerPrefab);
        // let the pointer object handle clearing itself and continuing the tutorial
        marker.InitializeMarker(interactable, _onInteractSuccess, TutorialActionCompleted);
        // this action will be marked completed when the target has been interacted with
        return TutorialActionStatus.Incomplete;
    }

    public override void Abort() {
        base.Abort();
        IInteractable interactable = LevelManager.LevelManagerInstance.GetInteractable(_targetId);
        if (interactable == null) {
            Debug.LogError($"[{nameof(TutorialActionPointInteractableTarget)}] Could not find interactable for ID {_targetId}");
            return;
        }
        Debug.LogWarning($"[{nameof(TutorialActionPointInteractableTarget)}] UNFINISHED ABORT ACTION. PLEASE IMPLEMENT!");
    }
}
