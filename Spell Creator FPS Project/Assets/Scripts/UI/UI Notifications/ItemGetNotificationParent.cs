using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemGetNotificationParent : MonoBehaviour, IUINotificationParent {
    
    [SerializeField] private UINotification _notificationPrefab;
    [SerializeField] private RectTransform _container;
    [SerializeField] private int _messageCountLimit;
    [SerializeField] private int _messageCountPreload;

    private List<UINotification> _availableNotificationsPool = new List<UINotification>();
    private List<NotificationEntry> _activeNotifications = new List<NotificationEntry>();

    public void Initialize() {
        for(int i = 0; i < _messageCountPreload; i++) {
            UINotification clone = Instantiate(_notificationPrefab, _container);
            clone.Initialize(this);
            _availableNotificationsPool.Add(clone);
        }
    }

    public string DisplayNotification(NotificationInitData initData, string overrideUniqueId = "") {
        string uniqueId = !string.IsNullOrEmpty(overrideUniqueId) ? overrideUniqueId : GenerateUniqueId();
        NotificationEntry entry;
        if (TryGetEntry(uniqueId, out entry)) {
            UpdateNotification(uniqueId, initData);
            return uniqueId;
        }
        // if max number of notifications are displayed
        if(_activeNotifications.Count >= _messageCountLimit) {
            RemoveNotification(0);
        }
        // display a new notification
        UINotification notification = _availableNotificationsPool[0];
        _activeNotifications.Add(new NotificationEntry() {
            UniqueId = uniqueId,
            Notification = notification
        });
        notification.Display(uniqueId, initData);
        _availableNotificationsPool.RemoveAt(0);
        return uniqueId;
    }

    private string GenerateUniqueId() {
        return $"ItemGetNotification_{StringGenerator.RandomString(5)}";
    }

    public void UpdateNotification(string uniqueId, NotificationInitData initData) {
        NotificationEntry entry;
        if (!TryGetEntry(uniqueId, out entry)) {
            ErrorManager.LogError(nameof(ItemGetNotificationParent), $"Failed to get notification with id {uniqueId}");
            return;
        }
        entry.Notification.UpdateData(initData);
    }

    public void RemoveNotification(string uniqueId) {
        NotificationEntry entry;
        if (!TryGetEntry(uniqueId, out entry)) {
            ErrorManager.LogError(nameof(ItemGetNotificationParent), $"Failed to get notification with id {uniqueId}");
            return;
        }
        RemoveNotification(entry);
    }

    private void RemoveNotification(int index) {
        if (index < 0 || index > _activeNotifications.Count) {
            return;
        }
        RemoveNotification(_activeNotifications[index]);
    }

    private void RemoveNotification(NotificationEntry entry) {
        entry.Notification.Hide();
        _activeNotifications.Remove(entry);
        _availableNotificationsPool.Add(entry.Notification);
    }

    public void ClearAllNotifications() {
        foreach(NotificationEntry entry in _activeNotifications) {
            RemoveNotification(entry);
        }
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