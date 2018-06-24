using System;
using System.Collections.Generic;

namespace Wpf.Data
{
    public interface IRepository<T> : IDisposable
    {
        T GetById(int id);
        IEnumerable<T> GetAll();
        void Add(T entity);
        void Save();
    }
}
