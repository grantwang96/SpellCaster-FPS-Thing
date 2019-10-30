using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public interface ICinematicsManager {

    IReadOnlyDictionary<string, PlayableDirector> Directors { get; }
}

public class CinematicsManager : MonoBehaviour, ICinematicsManager
{
    public static ICinematicsManager Instance { get; private set; }

    [SerializeField] private List<PlayableDirector> _sceneDirectors = new List<PlayableDirector>();
    private Dictionary<string, PlayableDirector> _directors = new Dictionary<string, PlayableDirector>();
    public IReadOnlyDictionary<string, PlayableDirector> Directors => _directors;

    private void Awake() {
        Instance = this;
        RegisterDirectors();
    }

    private void RegisterDirectors() {
        for(int i = 0; i < _sceneDirectors.Count; i++) {
            _directors.Add(_sceneDirectors[i].name, _sceneDirectors[i]);
        }
    }
}
