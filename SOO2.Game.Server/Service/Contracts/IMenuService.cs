﻿namespace SOO2.Game.Server.Service.Contracts
{
    public interface IMenuService
    {
        string BuildBar(int currentValue, int requiredValue, int numberOfBars, string colorToken = null);
    }
}
