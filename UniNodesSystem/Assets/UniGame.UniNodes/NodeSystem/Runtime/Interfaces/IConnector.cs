namespace UniGreenModules.UniNodeSystem.Runtime.Interfaces
{
    using System;

    public interface IConnector<T>
    {
        IDisposable Bind(T connection);
        
    }
}