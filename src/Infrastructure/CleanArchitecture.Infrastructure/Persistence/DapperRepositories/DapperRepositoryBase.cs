using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Dapper;

public abstract class DapperRepositoryBase
{
    protected readonly IDbConnectionFactory ConnectionFactory;
    protected readonly ILogger Logger;

    protected DapperRepositoryBase(IDbConnectionFactory connectionFactory, ILogger logger)
    {
        ConnectionFactory = connectionFactory;
        Logger = logger;
    }

    protected async Task<T> ExecuteStoredProcedureAsync<T>(
        string storedProcedure,
        object parameters = null,
        int? commandTimeout = null)
    {
        try
        {
            using var connection = await ConnectionFactory.CreateConnectionAsync();
            return await connection.QueryFirstOrDefaultAsync<T>(
                storedProcedure,
                parameters,
                commandType: CommandType.StoredProcedure,
                commandTimeout: commandTimeout);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error executing stored procedure {StoredProcedure}", storedProcedure);
            throw;
        }
    }

    protected async Task<IEnumerable<T>> ExecuteStoredProcedureListAsync<T>(
        string storedProcedure,
        object parameters = null,
        int? commandTimeout = null)
    {
        try
        {
            using var connection = await ConnectionFactory.CreateConnectionAsync();
            return await connection.QueryAsync<T>(
                storedProcedure,
                parameters,
                commandType: CommandType.StoredProcedure,
                commandTimeout: commandTimeout);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error executing stored procedure {StoredProcedure}", storedProcedure);
            throw;
        }
    }

    protected async Task<int> ExecuteStoredProcedureNonQueryAsync(
        string storedProcedure,
        object parameters = null,
        int? commandTimeout = null)
    {
        try
        {
            using var connection = await ConnectionFactory.CreateConnectionAsync();
            return await connection.ExecuteAsync(
                storedProcedure,
                parameters,
                commandType: CommandType.StoredProcedure,
                commandTimeout: commandTimeout);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error executing stored procedure {StoredProcedure}", storedProcedure);
            throw;
        }
    }
} 