using SQLite;

namespace Integreat.Shared.Services.Persistence
{
	public interface ISqLite
	{
		SQLiteConnection GetConnection();
        SQLiteAsyncConnection GetAsyncConnection();
	}
}

