using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaListener_BetweenRounds : MonoBehaviour {

    [SerializeField] private List<GameObject> _activeGameObjectsBetween = new List<GameObject>();

    private void Start() {
        ArenaManager.ArenaManagerInstance.OnRoundStarted += OnRoundStart;
        ArenaManager.ArenaManagerInstance.OnRoundEnded += OnRoundEnd;
        OnRoundEnd(0);
    }

    private void OnDestroy() {
        ArenaManager.ArenaManagerInstance.OnRoundStarted -= OnRoundStart;
        ArenaManager.ArenaManagerInstance.OnRoundEnded -= OnRoundEnd;
    }

    private void OnRoundStart(int round) {
        SetGOsActive(false);
    }

    private void SetGOsActive(bool active) {
        for(int i = 0; i < _activeGameObjectsBetween.Count; i++) {
            _activeGameObjectsBetween[i].SetActive(active);
        }
    }

    private void OnRoundEnd(int round) {
        SetGOsActive(true);
    }
}
