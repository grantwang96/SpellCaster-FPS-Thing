using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UINotificationsManager : MonoBehaviour
{
    public static UINotificationsManager Instance { get; private set; }

    [SerializeField] private List<GameObject> _notificationPrefabGOs = new List<GameObject>();
    // dictionary that stores all notification prefabs
    private Dictionary<string, IUINotificationParent> _notificationParents = new Dictionary<string, IUINotificationParent>();

    [SerializeField] private Transform _notificationsLayer;

    private void Awake() {
        if (Instance != null) {
            Debug.LogWarning("WTF? More than one UINotificationManager exists in the scene!");
            return;
        }
        Instance = this;
    }

    private void Start() {
        PreloadUINotifications();
    }

    private void PreloadUINotifications() {
        for (int i = 0; i < _notificationPrefabGOs.Count; i++) {
            IUINotificationParent notificationParentPrefab = _notificationPrefabGOs[i].GetComponent<IUINotificationParent>();
            if (notificationParentPrefab == null) {
                ErrorManager.LogGameObjectError(_notificationPrefabGOs[i].name, $"Failed to preload! Does not contain {nameof(IUINotificationParent)} component!");
                continue;
            }
            if (_notificationParents.ContainsKey(_notificationPrefabGOs[i].name)) {
                ErrorManager.LogError(nameof(UIPanelManager), $"Already contains entry for {_notificationPrefabGOs}");
                continue;
            }
            GameObject goClone = Instantiate(_notificationPrefabGOs[i], _notificationsLayer.transform);
            IUINotificationParent clone = goClone.GetComponent<IUINotificationParent>();
            clone.Initialize();
            _notificationParents.Add(_notificationPrefabGOs[i].name, clone);
        }
    }

    // display a notification and assign/return its unique id
    public string DisplayNotification(string notificationParentId, NotificationInitData initData) {
        // get the notification parent
        IUINotificationParent parent;
        if (!_notificationParents.TryGetValue(notificationParentId, out parent)) {
            ErrorManager.LogError(nameof(UIPanelManager), $"Failed to retrieve notification parent with id {notificationParentId}");
            return string.Empty;
        }
        string uniqueId = parent.DisplayNotification(initData);
        return uniqueId;
    }

    public void UpdateNotification(string notificationParentId, string uniqueId, NotificationInitData initData) {
        IUINotificationParent parent;
        if (!_notificationParents.TryGetValue(notificationParentId, out parent)) {
            ErrorManager.LogError(nameof(UIPanelManager), $"Failed to retrieve notification parent with id {notificationParentId}");
            return;
        }
        parent.UpdateNotification(uniqueId, initData);
    }

    public void RemoveNotification(string notificationParentId, string uniqueId) {
        IUINotificationParent parent;
        if (!_notificationParents.TryGetValue(notificationParentId, out parent)) {
            ErrorManager.LogError(nameof(UIPanelManager), $"Failed to retrieve notification parent with id {notificationParentId}");
            return;
        }
        parent.RemoveNotification(uniqueId);
    }

    public void ClearNotificationsForParent(string notificationParentId) {
        IUINotificationParent parent;
        if (!_notificationParents.TryGetValue(notificationParentId, out parent)) {
            ErrorManager.LogError(nameof(UIPanelManager), $"Failed to retrieve notification parent with id {notificationParentId}");
            return;
        }
        parent.ClearAllNotifications();
    }

    public void ClearAllNotifications() {
        foreach (KeyValuePair<string, IUINotificationParent> pair in _notificationParents) {
            pair.Value.ClearAllNotifications();
        }
    }
}
