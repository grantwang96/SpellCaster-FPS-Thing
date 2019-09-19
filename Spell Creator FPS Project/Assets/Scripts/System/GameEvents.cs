using System;

// master class that holds high level game event concepts
public static partial class GameEventsManager {

    public static GameEvent TestEvent { get; } = new GameEvent();

    public static GameEvent InteractableUIHighlighted { get; } = new GameEvent();
    public static GameEvent InteractableUISelected { get; } = new GameEvent();

    public static GameEvent SceneLoaded { get; } = new GameEvent();
    public static GameEvent GameHubEntered { get; } = new GameEvent();
    public static GameEvent ArenaLevelEntered { get; } = new GameEvent();

    public static GameEvent LevelStart { get; } = new GameEvent();

    public static GameEvent<StorableSpell> SpellCrafted { get; } = new GameEvent<StorableSpell>();

    public static GameEvent<string> TestEventArgs { get; } = new GameEvent<string>();
}

public class GameEvent {
    public event Action GameEventAction;

    public void Broadcast() {
        GameEventAction?.Invoke();
    }

    public void Subscribe(Action action) {
        GameEventAction += action;
    }

    public void Unsubscribe(Action action) {
        GameEventAction -= action;
    }

    public void UnsubscribeAll() {
        GameEventAction = null;
    }
}

public class GameEvent<T> {
    public event Action<T> GameEventAction;

    public void Broadcast(T arg0) {
        GameEventAction?.Invoke(arg0);
    }

    public void Subscribe(Action<T> action) {
        GameEventAction += action;
    }

    public void Unsubscribe(Action<T> action) {
        GameEventAction -= action;
    }

    public void UnsubscribeAll() {
        GameEventAction = null;
    }
}
