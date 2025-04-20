// Services/NotificationStore.cs
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

public class NotificationStore
{
    // Clé = adminId (Guid), Valeur = liste des notifications
    private readonly ConcurrentDictionary<Guid, List<string>> _notifications = new();

    // Ajouter une notification pour un admin
    public void AddNotification(Guid adminId, string message)
    {
        _notifications.AddOrUpdate(
            adminId,
            new List<string> { message },
            (key, existingList) =>
            {
                existingList.Add(message);
                return existingList;
            });
    }

    // Récupérer les notifications d’un admin
    public List<string> GetNotifications(Guid adminId)
    {
        return _notifications.TryGetValue(adminId, out var notifications)
            ? notifications
            : new List<string>();
    }

    // Supprimer toutes les notifications d’un admin
    public void ClearNotifications(Guid adminId)
    {
        _notifications.TryRemove(adminId, out _);
    }
}
