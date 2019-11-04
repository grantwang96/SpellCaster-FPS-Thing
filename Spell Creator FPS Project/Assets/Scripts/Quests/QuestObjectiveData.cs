using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestObjectiveData : ScriptableObject {

    public abstract QuestObjective GenerateQuestObjective();
}
