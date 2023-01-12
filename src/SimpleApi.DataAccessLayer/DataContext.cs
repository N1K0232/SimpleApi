using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SimpleApi.Security;
using System.Data;

namespace SimpleApi.DataAccessLayer;

public class DataContext : IDataContext
{
    private readonly IConfiguration configuration;
    private readonly ILogger<DataContext> logger;

    private SqlConnection connection;
    private SqlCommand command;

    private SqlDataAdapter adapter;
    private SqlDataReader reader;

    private IPasswordHasher passwordHasher;

    private bool disposed;


    public DataContext(IConfiguration configuration, IPasswordHasher passwordHasher, ILogger<DataContext> logger)
    {
        this.configuration = configuration;
        this.passwordHasher = passwordHasher;
        this.logger = logger;

        connection = null;
        command = null;

        adapter = null;
        reader = null;

        Initialize();
    }


    public Task<int> SaveAsync()
    {
        ThrowIfDisposed();
        if (connection == null || connection.State == ConnectionState.Closed || command == null)
        {
            throw new InvalidOperationException("the connection isn't open can't save changes");
        }

        return command.ExecuteNonQueryAsync();
    }


    private void Initialize()
    {
        string connectionString = configuration.GetConnectionString("SqlConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("the connection string is invalid");
        }

        SqlConnection value = new(StringHasher.GetString(connectionString));
        Exception e = null;

        try
        {
            value.Open();
            value.Close();
        }
        catch (SqlException ex)
        {
            e = ex;
        }
        catch (InvalidOperationException ex)
        {
            e = ex;
        }

        if (e != null)
        {
            logger.LogError(e, "Can't open connection");
            throw e;
        }

        connection = value;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (disposing && !disposed)
        {
            if (connection != null)
            {
                if (connection.State is ConnectionState.Open)
                {
                    connection.Close();
                }

                connection.Dispose();
                connection = null;
            }

            if (command != null)
            {
                command.Dispose();
                command = null;
            }

            if (adapter != null)
            {
                adapter.Dispose();
                adapter = null;
            }

            if (reader != null)
            {
                reader.Dispose();
                reader = null;
            }

            if (passwordHasher != null)
            {
                passwordHasher.Dispose();
                passwordHasher = null;
            }

            disposed = true;
        }
    }

    private void ThrowIfDisposed()
    {
        if (disposed)
        {
            throw new ObjectDisposedException(GetType().FullName);
        }
    }
}