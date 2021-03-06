﻿using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Cubra
{
    public class FileProcessing : MonoBehaviour
    {
        /// <summary>
        /// Чтение json файла
        /// </summary>
        /// <param name="fileName">имя файла</param>
        /// <returns>строка из файла</returns>
        protected string ReadJsonFile(string fileName)
        {
            // Получаем путь до json файла
            var path = Path.Combine(Application.streamingAssetsPath, fileName + ".json");

            // Получаем данные по указанному пути
            UnityWebRequest reader = UnityWebRequest.Get(path);
            // Выполняем обработку полученнных данных
            reader.SendWebRequest();
            // Ждем завершения обработки
            while (!reader.isDone) {}

            // Возвращаем прочитанную строку
            return reader.downloadHandler.text;
        }

        /// <summary>
        /// Преобразование json строки в объект (объект для записи, )
        /// </summary>
        /// <param name="obj">объект для записи</param>
        /// <param name="json">json строка</param>
        protected void ConvertToObject<T>(ref T obj, string json)
        {
            obj = JsonUtility.FromJson<T>(json);
        }
    }
}