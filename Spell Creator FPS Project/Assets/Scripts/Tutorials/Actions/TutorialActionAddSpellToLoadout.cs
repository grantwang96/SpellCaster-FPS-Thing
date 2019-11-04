using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tutorials/Actions/AddSpellToLoadout")]
public class TutorialActionAddSpellToLoadout : TutorialAction
{
    [SerializeField] private string _castingMethod;
    [SerializeField] private List<string> _effects = new List<string>();
    [SerializeField] private List<string> _modifiers = new List<string>();
    [SerializeField] private string _spellName;
    [SerializeField] private int _spellSlotIndex;

    public override TutorialActionStatus Execute() {
        StorableSpell storableSpell = new StorableSpell(
            _castingMethod,
            _effects.ToArray(),
            _modifiers.ToArray(),
            false,
            _spellName
        );
        GameManager.GameManagerInstance.CurrentSpellInventory.AddSpell(storableSpell);
        GameManager.GameManagerInstance.CurrentSpellInventory.SetSpellInLoadout(storableSpell.InstanceId, _spellSlotIndex);
        return base.Execute();
    }
}
