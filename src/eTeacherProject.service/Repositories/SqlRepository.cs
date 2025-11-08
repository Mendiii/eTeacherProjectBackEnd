using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using eTeacherProject.Interfaces;

namespace eTeacherProject.Repositories;

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

    public async Task AddAsync(T entity)
    {
        var properties = typeof(T).GetProperties()
            .Where(p => !p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase)) // skip Id
            .ToList();

        var columnNames = string.Join(", ", properties.Select(p => p.Name));
        var paramNames = string.Join(", ", properties.Select(p => "@" + p.Name));

        var sql = $"INSERT INTO {_tableName} ({columnNames}) VALUES ({paramNames})";

        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, conn);

        foreach (var prop in properties)
        {
            var value = prop.GetValue(entity) ?? DBNull.Value;
            cmd.Parameters.AddWithValue("@" + prop.Name, value);
        }

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }


    public async Task UpdateAsync(T entity)
    {
        var properties = typeof(T).GetProperties();
        var keyProp = properties.FirstOrDefault(p =>
            p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));

        if (keyProp == null)
            throw new InvalidOperationException("Entity must have an Id property for Update.");

        // Build SET clause excluding Id
        var setClause = string.Join(", ", properties
            .Where(p => !p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
            .Select(p => $"{p.Name}=@{p.Name}"));

        var sql = $"UPDATE {_tableName} SET {setClause} WHERE Id=@Id";

        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(sql, conn);

        // Add parameters for all properties (including Id for WHERE clause)
        foreach (var prop in properties)
        {
            var value = prop.GetValue(entity) ?? DBNull.Value;
            cmd.Parameters.AddWithValue("@" + prop.Name, value);
        }

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
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
