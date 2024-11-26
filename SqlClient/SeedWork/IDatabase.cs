using SqlClient.Domain;

namespace SqlClient.SeedWork;

public interface IDatabase : IDisposable, IAsyncDisposable
{
    IEnumerable<MyNote> Read();
    void Insert(string note);
    void Update(int id, string note);
    void Delete(int id);
    void InsertMultiple(IEnumerable<string> notes);
}

