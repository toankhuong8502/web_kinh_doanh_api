using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace WebKinhDoanh.Infrastructure.Sql;

public sealed class SpRunner(NpgsqlDataSource dataSource)
{
    public async Task<IReadOnlyList<T>> QueryAsync<T>(
        string procedure,
        object? parameters = null,
        CancellationToken cancellationToken = default)
    {
        await using var connection = dataSource.CreateConnection();
        var command = new CommandDefinition(
            procedure,
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);
        var rows = await connection.QueryAsync<T>(command);
        return rows.AsList();
    }
}
