using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// defines basic notification parent functionality
public interface IUINotificationParent
{
    void Initialize();
    string DisplayNotification(NotificationInitData initData, string overrideUniqueId = "");
    void UpdateNotification(string uniqueId, NotificationInitData initData);
    void RemoveNotification(string uniqueId);
    void ClearAllNotifications();
}

// container that holds information to be passed to notification
public class NotificationInitData {
    public readonly Sprite Icon;
    public readonly string Message;

    public NotificationInitData(string message, Sprite icon) {
        Message = message;
        Icon = icon;
    }
}

public class NotificationEntry {
    public string UniqueId;
    public UINotification Notification;
}