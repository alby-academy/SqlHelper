using Microsoft.Data.SqlClient;
using SqlClient.Domain;
using SqlClient.SeedWork;
using Dapper;

namespace SqlClient;

public class Database(string connectionString) : IDatabase
{
    private readonly SqlConnection connection = new(connectionString);

    public IEnumerable<MyNote> Read()
    {
        const string query = "SELECT Id, Note, Inserted FROM Notes";
        connection.Open();
        var result = connection.Query<MyNote>(query);
        connection.Close();
        return result;
    }

    public void Insert(string note)
    {
        const string query = "INSERT INTO Notes (Note, Inserted) VALUES (@Note, @Inserted)";
        connection.Open();

        connection.Execute(query, new { Note = note, Inserted = DateTime.Now });
        connection.Close();
    }

    public void Update(int id, string note)
    {
        const string query = "UPDATE Notes SET Note = @Note WHERE Id = @Id";
        connection.Open();
        connection.Execute(query, new { Id = id, Note = note });
        connection.Close();
    }

    public void Delete(int id)
    {
        const string query = "DELETE FROM Notes WHERE Id = @Id";
        connection.Open();
        connection.Execute(query, new { Id = id });
        connection.Close();
    }

    public void MassiveInsert(ICollection<string> notes)
    {
        const string query = "INSERT INTO Notes (Note, Inserted) VALUES (@Note, @Inserted)";

        connection.Open();
        var time = DateTimeOffset.Now;

        var transaction = connection.BeginTransaction();

        try
        {
            foreach (var note in notes)
            {
                connection.Execute(query, new { Note = note, Inserted = time }, transaction: transaction);
            }
            transaction.Commit();
        }
        catch (Exception e)
        {
            transaction.Rollback();
        }
        finally
        {
            connection.Close();
        }
    }

    public async ValueTask DisposeAsync() => await connection.DisposeAsync();

    public void Dispose() => connection.Dispose();
}