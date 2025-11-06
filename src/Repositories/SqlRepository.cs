using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using eTeacherProject.Interfaces;

namespace eTeacherProject.Repositories;

// NOTE: This is a simple ADO.NET generic repository scaffold.
// To use effectively, define tables with columns matching your models (Id, etc.).
// For complex mapping, consider specialized repositories per entity.
public class SqlRepository<T> : IGenericRepository<T> where T : class, new()
{
    private readonly string _tableName;
    private readonly Func<IDataReader, T> _map;
    private readonly string _connectionString;

    public SqlRepository(string connectionString, string tableName, Func<IDataReader, T> map)
    {
        _connectionString = connectionString;
        _tableName = tableName;
        _map = map;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        var list = new List<T>();
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand($"SELECT * FROM {_tableName}", conn);
        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync()) list.Add(_map(reader));
        return list;
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand($"SELECT * FROM {_tableName} WHERE Id = @Id", conn);
        cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int){ Value = id });
        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync()) return _map(reader);
        return null;
    }

    // Minimal insert/update implementation requires matching schema.
    public async Task AddAsync(T entity)
    {
        // Implement per-entity insert. For generic use, inject a specialized repository instead.
        throw new NotImplementedException("Provide INSERT implementation specific to T/table.");
    }

    public async Task UpdateAsync(T entity)
    {
        // Implement per-entity update. For generic use, inject a specialized repository instead.
        throw new NotImplementedException("Provide UPDATE implementation specific to T/table.");
    }

    public async Task DeleteAsync(int id)
    {
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand($"DELETE FROM {_tableName} WHERE Id = @Id", conn);
        cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int){ Value = id });
        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }
}
