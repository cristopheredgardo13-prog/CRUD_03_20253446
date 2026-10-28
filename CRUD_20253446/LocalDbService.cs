using SQLite;
namespace CRUD_20253446;
// Una conexión por aplicación; la tabla se inicializa de forma asíncrona antes de usarla.
public sealed class LocalDbService
{
 private readonly SQLiteAsyncConnection _connection;
 private readonly SemaphoreSlim _initialization = new(1, 1);
 private bool _ready;
 public LocalDbService() : this(Path.Combine(FileSystem.AppDataDirectory, "sueldos20253446.db3")) { }
 public LocalDbService(string path)
 {
  _connection = new SQLiteAsyncConnection(path,
   SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
 }
 private async Task InitializeAsync()
 {
  await _initialization.WaitAsync();
  try { if (!_ready) { await _connection.CreateTableAsync<Trabajador>(); _ready = true; } }
  finally { _initialization.Release(); }
 }
 public async Task<List<Trabajador>> GetAllAsync()
 { await InitializeAsync(); return await _connection.Table<Trabajador>().OrderByDescending(x => x.Id).ToListAsync(); }
 public async Task<Trabajador?> GetByIdAsync(int id)
 { await InitializeAsync(); return await _connection.FindAsync<Trabajador>(id); }
 public async Task CreateAsync(Trabajador item)
 { await InitializeAsync(); await _connection.InsertAsync(item); }
 public async Task UpdateAsync(Trabajador item)
 {
  await InitializeAsync();
  if (await _connection.UpdateAsync(item) != 1) throw new InvalidOperationException("El registro ya no existe.");
 }
 public async Task DeleteAsync(Trabajador item)
 {
  await InitializeAsync();
  if (await _connection.DeleteAsync(item) != 1) throw new InvalidOperationException("El registro ya no existe.");
 }
}
