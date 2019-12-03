﻿using UnityEngine;

public class IncreaseListStatuses : MonoBehaviour
{
    /// <summary>Добавление недостающих элементов (объект для записи, количество элементов, ключ сохранения)</summary>
    protected void AddToList(StaJson statuses, int quantity, string key)
    {
        // Если добавлены новые элементы
        if (statuses.status.Count < quantity)
        {
            // Подсчитываем разницу
            var difference = quantity - statuses.status.Count;

            for (int i = 0; i < difference; i++)
                // Добавляем недостающие элементы
                statuses.status.Add("no");

            // Сохраняем обновленный список
            SaveListStatuses(statuses, key);
        }
    }

    /// <summary>Сохранение списка статусов (объект для записи, ключ сохранения)</summary>
    protected void SaveListStatuses(StaJson statuses, string key)
    {
        PlayerPrefs.SetString(key, JsonUtility.ToJson(statuses));
    }
}