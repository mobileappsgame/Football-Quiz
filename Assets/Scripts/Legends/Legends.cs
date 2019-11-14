﻿using UnityEngine;
using UnityEngine.UI;

public class Legends : MonoBehaviour
{
    // Номер выбранной карточки
    public static int descriptionCard;

    // Позиция скролла в списке карточек
    public static float scrollPosition = 1;

    [Header("Набор карточек")]
    [SerializeField] private GameObject cards;

    [Header("Компонент скролла")]
    [SerializeField] private ScrollRect scroll;

    [Header("Анимация эффекта открытия")]
    [SerializeField] private Animator effect;

    // Объект для json по статусам карточек
    private StaJson statuses = new StaJson();

    // Ссылка на компонент статистики
    private Statistics statistics;

    private void Awake()
    {
        // Преобразовываем сохраненную json строку в объект
        statuses = JsonUtility.FromJson<StaJson>(PlayerPrefs.GetString("legends"));

        // Получаем компоненты
        effect = effect.GetComponent<Animator>();
        statistics = Camera.main.GetComponent<Statistics>();
    }

    private void Start()
    {
        // Если карточек больше (добавлены новые), чем размер сохраненного списка
        if (statuses.Status.Count < cards.transform.childCount)
        {
            // Подсчитываем разницу
            var difference = cards.transform.childCount - statuses.Status.Count;

            for (int i = 0; i < difference; i++)
            {
                // Добавляем недостающие закрытые элементы
                statuses.Status.Add("no");
            }

            // Сохраняем карточки
            SaveLegendaryCards();
        }

        // Устанавливаем позицию скролла
        scroll.verticalNormalizedPosition = scrollPosition;

        // Проверяем легендарные карточки
        CheckLegendaryCards();
    }

    /// <summary>Сохранение списка легендарных карточек</summary>
    private void SaveLegendaryCards()
    {
        PlayerPrefs.SetString("legends", JsonUtility.ToJson(statuses));
    }

    /// <summary>Проверка легендарных карточек</summary>
    private void CheckLegendaryCards()
    {
        for (int i = 0; i < cards.transform.childCount; i++)
        {
            // Если карточка открыта
            if (statuses.Status[i] == "yes")
                // Отображаем открытый вариант
                cards.transform.GetChild(i).GetComponentInChildren<Legend>().ShowImageCard();
        }
    }

    /// <summary>Открытие легендарной карточки (номер карточки)</summary>
    public void OpenLegendaryCard(int number)
    {
        // Если карточка закрытая
        if (statuses.Status[number] == "no")
        {
            // Если достаточно монет для открытия
            if (PlayerPrefs.GetInt("coins") >= 950)
            {
                // Вычитаем монеты и увеличиваем общий счет
                PlayerPrefs.SetInt("coins", PlayerPrefs.GetInt("coins") - 950);
                PlayerPrefs.SetInt("score", PlayerPrefs.GetInt("score") + 450);
                // Обновляем статистику
                statistics.UpdateCoins();
                statistics.UpdateScore();

                // Записываем обновленное значение
                statuses.Status[number] = "yes";
                // Сохраняем данные по карточкам
                SaveLegendaryCards();

                // Увеличиваем общее количество открытых карточек
                PlayerPrefs.SetInt("legends-open", PlayerPrefs.GetInt("legends-open") + 1);

                // Отображаем открытую карточку
                cards.transform.GetChild(number).GetComponentInChildren<Legend>().ShowImageCard();
                // Отображаем эффект открытия над карточкой
                ShowOpeningEffect(cards.transform.GetChild(number).gameObject);
            }
            else
            {
                // Если монет недостаточно, вызываем мигание монет
                statistics.UpdateCoins(true);
            }
        }
        else
        {
            // Записываем последнюю позицию скролла
            scrollPosition = scroll.verticalNormalizedPosition;
            // Записываем номер карточки
            descriptionCard = number;
            // Переходим на сцену описания легенды
            Camera.main.GetComponent<TransitionsInMenu>().GoToScene(9);
        }
    }

    /// <summary>Отображение эффекта открытия (карточка)</summary>
    private void ShowOpeningEffect(GameObject card)
    {
        // Переставляем эффект к карточке
        effect.transform.position = card.transform.position;
        // Перемещаем эффект в родительский объект карточки
        effect.transform.SetParent(card.transform);
        // Делаем эффект нулевым в иерархии
        effect.transform.SetSiblingIndex(0);
        // Перезапускаем анимацию
        effect.Rebind();
    }
}