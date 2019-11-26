﻿using UnityEngine;
using UnityEngine.UI;

public class TasksFacts : FileProcessing
{
    [Header("Текст для задания")]
    [SerializeField] private Text question;

    [Header("Кнопки вариантов")]
    [SerializeField] private GameObject variants;

    [Header("Компонент таймера")]
    [SerializeField] private Timer timer;

    [Header("Победный эффект")]
    [SerializeField] private ParticleSystem particle;

    [Header("Кнопка обновления")]
    [SerializeField] private Button updateButton;

    // Номер этапа викторины
    private int stage = 0;

    // Объект для работы с json по заданиям
    private FacJson tasks = new FacJson();

    // Объект для json по наборам
    private StaJson statuses = new StaJson();

    // Ссылка на статистику
    private Statistics statistics;

    private void Awake()
    {
        // Обрабатываем json файл и записываем в текстовую переменную
        var jsonString = ReadJsonFile("facts-" + FactsCollections.collections.ToString());
        // Преобразовываем строку в объект
        ConvertToObject(ref tasks, jsonString);

        // Преобразовываем сохраненную json строку в объект
        statuses = JsonUtility.FromJson<StaJson>(PlayerPrefs.GetString("facts"));

        // Получаем компоненты
        timer = timer.GetComponent<Timer>();
        statistics = Camera.main.GetComponent<Statistics>();
    }

    private void Start()
    {
        SetTask();
    }

    /// <summary>Отображение текущего задания</summary>
    private void SetTask()
    {
        // Выводим задание
        question.text = tasks.Facts[stage].Question;

        // Активируем кнопки вариантов
        variants.SetActive(true);

        // Запускаем таймер уровня
        timer.StartCoroutine("LevelTimer");
    }

    /// <summary>Проверка ответа (состояние кнопки)</summary>
    public void ComparisonAnswers(bool state)
    {
        // Останавливаем таймер
        timer.StopAllCoroutines();

        // Если ответ правильный
        if (tasks.Facts[stage].Answer == state)
        {
            // Увеличиваем счет и монеты
            PlayerPrefs.SetInt("score", PlayerPrefs.GetInt("score") + 3);
            PlayerPrefs.SetInt("coins", PlayerPrefs.GetInt("coins") + 5);
            // Обновляем статистику по счету и монетам
            statistics.UpdateScore();
            statistics.UpdateCoins();

            // Запускаем победный эффект
            particle.Play();

            // Выводим полное описание ответа
            question.text =  tasks.Facts[stage].Description;

            // Увеличиваем общее количество правильных ответов
            PlayerPrefs.SetInt("facts-answer", PlayerPrefs.GetInt("facts-answer") + 1);

            // Увеличиваем этап
            stage++;

            // Активируем кнопку обновления вопроса
            updateButton.interactable = true;
        }
        else
        {
            // Выводим проигрышный текст
            question.text = "Неправильно!\nПодборка фактов провалена, так как в данном режиме игры ошибаться нельзя.";

            // Увеличиваем общее количество неправильных ответов
            PlayerPrefs.SetInt("facts-errors", PlayerPrefs.GetInt("facts-errors") + 1);

            // Закрываем категорию
            CloseCategory("loss");
        }
    }

    /// <summary>Обновление задания</summary>
    public void TaskUpdate()
    {
        // Если текущий этап не превышает количество заданий
        if (stage < tasks.Facts.Length)
        {
            // Сбрасываем таймер
            timer.ResetTimer();

            // Выводим новое задание
            SetTask();
        }
        else
        {
            // Выводим победный текст
            question.text = "Великолепно!\nДанная подборка фактов успешно пройдена.";

            // Увеличиваем общее количество победных подборок
            PlayerPrefs.SetInt("facts-victory", PlayerPrefs.GetInt("facts-victory") + 1);

            // Закрываем категорию
            CloseCategory("victory");

            // Начисляем бонус
            PlayerPrefs.SetInt("score", PlayerPrefs.GetInt("score") + 150);
            PlayerPrefs.SetInt("coins", PlayerPrefs.GetInt("coins") + 350);
            // Обновляем статистику
            statistics.UpdateScore();
            statistics.UpdateCoins();
        }
    }

    /// <summary>Закрытие категории (результат подборки)</summary>
    private void CloseCategory(string result)
    {
        // Записываем результат
        statuses.Status[FactsCollections.collections] = result;
        // Сохраняем обновленное значение
        PlayerPrefs.SetString("facts", JsonUtility.ToJson(statuses));

        // Увеличиваем общее количество завершенных подборок
        PlayerPrefs.SetInt("facts-quantity", PlayerPrefs.GetInt("facts-quantity") + 1);
    }
}