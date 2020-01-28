using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CoroutineGod : MonoBehaviour {

    public static CoroutineGod Instance { get; private set; }

    private void Awake() {
        Instance = this;
    }

    public void ClearAllCoroutines() {
        StopAllCoroutines();
    }

    public void ExecuteAfterOneFrame(Action action) {
        StartCoroutine(WaitOneFrameThenExecute(action));
    }

    private IEnumerator WaitOneFrameThenExecute(Action action) {
        yield return new WaitForEndOfFrame();
        action?.Invoke();
    }

    public void ExecuteAfterTime(Action action, float time) {
        StartCoroutine(WaitForTimeThenExecute(action, time));
    }

    private IEnumerator WaitForTimeThenExecute(Action action, float time) {
        yield return new WaitForSeconds(time);
        action?.Invoke();
    }
}
