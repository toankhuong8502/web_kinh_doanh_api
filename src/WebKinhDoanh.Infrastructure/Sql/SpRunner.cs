using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace WebKinhDoanh.Infrastructure.Sql;

public sealed class SpRunner(IConfiguration configuration)
{
    private readonly string _connectionString =
        configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' chưa được cấu hình.");

    public async Task<IReadOnlyList<T>> QueryAsync<T>(
        string procedure,
        object? parameters = null,
        CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(_connectionString);
        var command = new CommandDefinition(
            procedure,
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);
        var rows = await connection.QueryAsync<T>(command);
        return rows.AsList();
    }
}
