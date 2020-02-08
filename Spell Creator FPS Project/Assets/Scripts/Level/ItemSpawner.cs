using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private int _healthOrbs;
    [SerializeField] private int _manaOrbs;
    [SerializeField] private List<string> _runeIds = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        SpawnAll();
    }

    private void SpawnAll() {
        for (int i = 0; i < _healthOrbs; i++) {
            SpawnRecoveryOrb(RecoveryOrbType.Health);
        }
        for (int i = 0; i < _manaOrbs; i++) {
            SpawnRecoveryOrb(RecoveryOrbType.Mana);
        }
        for (int i = 0; i < _runeIds.Count; i++) {
            SpawnInventoryItem(_runeIds[i]);
        }
    }

    private void SpawnRecoveryOrb(RecoveryOrbType recoveryOrbType) {
        PooledObject obj;
        if (!PooledObjectManager.Instance.UsePooledObject(GameplayValues.ObjectPooling.RecoveryOrbPrefabId, out obj)) {
            return;
        }
        RecoveryOrb recoveryOrb = obj as RecoveryOrb;
        if (recoveryOrb != null) {
            recoveryOrb.ActivatePooledObject();
            recoveryOrb.Initialize(recoveryOrbType);
            recoveryOrb.transform.position = transform.position;
        }
    }

    private void SpawnInventoryItem(string itemId) {
        PooledObject obj;
        if (!PooledObjectManager.Instance.UsePooledObject(GameplayValues.ObjectPooling.WorldRunePrefabId, out obj)) {
            return;
        }
        Rune rune = obj as Rune;
        if (rune != null) {
            rune.transform.position = transform.position;
            rune.Initialize(itemId);
        }
    }
}
