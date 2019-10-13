using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LootConfig")]
public class LootConfig : ScriptableObject {

    [SerializeField] private List<string> _whiteTierLootIds;
    [SerializeField] private List<string> _greenTierLootIds;
    [SerializeField] private List<string> _purpleTierLootIds;
    [SerializeField] private List<string> _goldTierLootIds;
}
