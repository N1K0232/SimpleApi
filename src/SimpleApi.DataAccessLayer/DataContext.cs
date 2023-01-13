using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SimpleApi.DataAccessLayer.Entities;
using SimpleApi.DataAccessLayer.Entities.Common;
using SimpleApi.Security;
using System.Data;
using System.Reflection;
using System.Text;

namespace SimpleApi.DataAccessLayer;

public partial class DataContext : IDataContext
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

        disposed = false;

        Initialize();
    }



    public async Task<IEnumerable<Person>> GetListAsync()
    {
        try
        {
            await connection.OpenAsync();
            command = new SqlCommand("SELECT Id,FirstName,LastName FROM People", connection);
            reader = await command.ExecuteReaderAsync();

            var people = new List<Person>();

            while (await reader.ReadAsync())
            {
                people.Add(new Person
                {
                    Id = Guid.Parse(reader["Id"].ToString()),
                    FirstName = reader["FirstName"].ToString(),
                    LastName = reader["LastName"].ToString()
                });
            }

            await connection.CloseAsync();

            return people;
        }
        catch (SqlException ex)
        {
            logger.LogError(ex, "can't retrieve list");
            throw ex;
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "can't retrieve list");
            throw ex;
        }
    }

    public async Task CreateAsync(Person person)
    {
        string commandText;

        try
        {
            await connection.OpenAsync();
            command = new SqlCommand(null, connection);

            commandText = "";
            commandText += $"INSERT INTO People(Id,FirstName,LastName) ";
            commandText += "VALUES(@Id,@FirstName,@LastName)";

            command.CommandText = commandText;

            SaveEntityCore(person);
        }
        catch (SqlException ex)
        {
            logger.LogError(ex, "Can't save person");
            await Task.FromException<SqlException>(ex);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Can't save person");
            await Task.FromException<InvalidOperationException>(ex);
        }
    }

    public async Task<int> SaveAsync()
    {
        ThrowIfDisposed();

        if (connection == null || connection.State == ConnectionState.Closed || command == null)
        {
            throw new InvalidOperationException("The connection isn't open. Can't save changes");
        }

        int savedEntries = await command.ExecuteNonQueryAsync();
        await connection.CloseAsync();

        return savedEntries;
    }


    private void Initialize()
    {
        string connectionString = configuration.GetConnectionString("SqlConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("the connection string is invalid");
        }

        SqlConnection value = new(connectionString);
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

    private void SaveEntityCore<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
        if (entity.Id == Guid.Empty)
        {
            entity.Id = Guid.NewGuid();
        }

        Type entityType = entity.GetType();
        PropertyInfo[] properties = entityType.GetProperties();

        foreach (PropertyInfo property in properties)
        {
            command.Parameters.Add(new SqlParameter(property.Name, property.GetValue(entity)));
        }
    }

    //TO-DO
    private async Task<int> CountAsync(string query)
    {
        using var countCommand = new SqlCommand(query, connection);
        object countObject = await countCommand.ExecuteScalarAsync();

        return Convert.ToInt32(countObject);
    }

    public override int GetHashCode()
    {
        ThrowIfDisposed();

        int hashCode = base.GetHashCode();

        return hashCode |
            connection.GetHashCode() |
            command?.GetHashCode() ?? 0 |
            adapter?.GetHashCode() ?? 0 |
            reader?.GetHashCode() ?? 0;
    }

    public override string ToString()
    {
        ThrowIfDisposed();

        var sb = new StringBuilder();
        sb.AppendLine(base.ToString());
        sb.AppendLine($"Database: {connection.Database}");
        sb.AppendLine($"DataSource: {connection.DataSource}");

        return sb.ToString();
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