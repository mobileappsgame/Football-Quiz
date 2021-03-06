﻿using System;

namespace Cubra.Helpers
{
    [Serializable]
    public class LegendsHelpers
    {
        // Имя футболиста
        public string Name;

        // Достижения футболиста
        public ProgressLegend Progress;
    }

    [Serializable]
    public class ProgressLegend
    {
        // Клубные достижения
        public string Club;

        // Достижения в сборной
        public string Team;

        // Особые достижения
        public string Personal;
    }
}