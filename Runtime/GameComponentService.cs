﻿namespace UniGame.UniNodes.GameFlow.Runtime
{
    using Core.Runtime;
    using UniGame.GameFlow.Runtime.Interfaces;
    using UniRx;
    using UnityEngine;

    public class GameComponentService<TService> : MonoBehaviour, IGameService
        where TService : IGameService,new() 
    {
        protected TService Service = new TService();

        public void Dispose() => Service.Dispose();

        public ILifeTime LifeTime => Service.LifeTime;

        private void OnDestroy() => Dispose();
    }
}
