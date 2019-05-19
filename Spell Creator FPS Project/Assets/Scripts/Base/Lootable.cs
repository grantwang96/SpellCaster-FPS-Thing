using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILootable {

	string Id { get; }
    void Initialize();
    void Initialize(string itemId);
    void ReleaseFromChest(Vector3 force);
}
