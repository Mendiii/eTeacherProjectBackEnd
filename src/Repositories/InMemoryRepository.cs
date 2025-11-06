using eTeacherProject.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eTeacherProject.Repositories;

public class InMemoryRepository<T> : IGenericRepository<T> where T : class
{
    private readonly List<T> _store = new();
    private int _nextId = 1;

    private static int GetIdValue(T entity)
    {
        var prop = typeof(T).GetProperty("Id");
        return prop is null ? 0 : (int)(prop.GetValue(entity) ?? 0);
    }

    private static void SetIdValue(T entity, int id)
    {
        var prop = typeof(T).GetProperty("Id");
        if (prop is not null && prop.CanWrite) prop.SetValue(entity, id);
    }

    public Task<IEnumerable<T>> GetAllAsync() => Task.FromResult<IEnumerable<T>>(_store);

    public Task<T?> GetByIdAsync(int id)
    {
        var prop = typeof(T).GetProperty("Id");
        var found = _store.FirstOrDefault(e => prop is not null && (int)(prop.GetValue(e) ?? 0) == id);
        return Task.FromResult(found);
    }

    public Task AddAsync(T entity)
    {
        if (GetIdValue(entity) == 0) SetIdValue(entity, _nextId++);
        _store.Add(entity);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(T entity)
    {
        var id = GetIdValue(entity);
        if (id == 0) return Task.CompletedTask;
        var prop = typeof(T).GetProperty("Id");
        for (int i = 0; i < _store.Count; i++)
        {
            var currentId = prop is not null ? (int)(prop.GetValue(_store[i]) ?? 0) : 0;
            if (currentId == id)
            {
                _store[i] = entity;
                break;
            }
        }
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        var prop = typeof(T).GetProperty("Id");
        _store.RemoveAll(e => prop is not null && (int)(prop.GetValue(e) ?? 0) == id);
        return Task.CompletedTask;
    }
}
