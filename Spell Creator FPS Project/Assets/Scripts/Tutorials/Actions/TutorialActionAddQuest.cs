using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tutorials/Actions/AddQuest")]
public class TutorialActionAddQuest : TutorialAction
{
    [SerializeField] private string _questId;

    // list of objective data
    [SerializeField] private QuestObjectiveData[] _objectives;
    
    public override TutorialActionStatus Execute() {
        QuestManager.Instance.RegisterQuest(GenerateQuest());
        return base.Execute();
    }

    private Quest GenerateQuest() {
        List<QuestObjective> objectives = new List<QuestObjective>();
        for(int i = 0; i < _objectives.Length; i++) {
            objectives.Add(_objectives[i].GenerateQuestObjective());
        }
        return new Quest(_questId, objectives);
    }
}
