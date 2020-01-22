using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestNotificationParent : MonoBehaviour, IUINotificationParent
{
    [SerializeField] private UINotification _notificationPrefab;
    [SerializeField] private RectTransform _container;
    [SerializeField] private int _initialPoolSize;

    private List<UINotification> _notificationPool = new List<UINotification>();
    private List<NotificationEntry> _activeNotifications = new List<NotificationEntry>();

    public void Initialize() {
        for(int i = 0; i < _initialPoolSize; i++) {
            GenerateNotificationClone();
        }
    }

    private void GenerateNotificationClone() {
        UINotification clone = Instantiate(_notificationPrefab, _container.transform);
        clone.Initialize(this);
        _notificationPool.Add(clone);
    }

    public string DisplayNotification(NotificationInitData initData, string overrideUniqueId = "") {
        string uniqueId = !string.IsNullOrEmpty(overrideUniqueId) ? overrideUniqueId : GenerateUniqueId();
        NotificationEntry entry;
        if (TryGetEntry(uniqueId, out entry)) {
            UpdateNotification(uniqueId, initData);
            return uniqueId;
        }
        if(_notificationPool.Count == 0) {
            GenerateNotificationClone();
        }
        UINotification notification = _notificationPool[0];
        _activeNotifications.Add(new NotificationEntry() {
            UniqueId = uniqueId,
            Notification = notification
        });
        notification.Display(uniqueId, initData);
        _notificationPool.RemoveAt(0);
        return uniqueId;
    }

    private string GenerateUniqueId() {
        return $"QuestNotification_{StringGenerator.RandomString(5)}";
    }

    public void UpdateNotification(string uniqueId, NotificationInitData initData) {
        throw new System.NotImplementedException();
    }

    public void RemoveNotification(string uniqueId) {
        throw new System.NotImplementedException();
    }

    public void ClearAllNotifications() {
        throw new System.NotImplementedException();
    }

    private bool TryGetEntry(string uniqueId, out NotificationEntry entry) {
        entry = null;
        for (int i = 0; i < _activeNotifications.Count; i++) {
            if (_activeNotifications[i].UniqueId.Equals(uniqueId)) {
                entry = _activeNotifications[i];
                return true;
            }
        }
        return false;
    }
}
