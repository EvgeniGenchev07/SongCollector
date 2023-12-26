using SongCollector;
using SQLite;
using SQLiteNetExtensionsAsync.Extensions;

namespace SongCollector
{
    public class Database
    {
        private SQLiteAsyncConnection _database;
        private readonly string _databasePath;

        public Database(string dbPath)
        {
            _databasePath = dbPath;
        }
        private async Task Init()
        {
            if (_database != null)
            {
                return;
            }
            _database = new SQLiteAsyncConnection(_databasePath);
            //await _database.DropTableAsync<Song>();
            await _database.CreateTableAsync<Song>();
        }
        public async Task SaveSong(Song song)
        {
            await Init();
            await _database.InsertWithChildrenAsync(song, true);
        }
        public async Task EditSong(Song song)
        {
            await Init();
            await _database.UpdateWithChildrenAsync(song);
        }
        public async Task<List<Song>> GetAllSongs()
        {
            await Init();
            List<Song> songs = await _database.GetAllWithChildrenAsync<Song>(recursive: true);
            return songs;
        }
        public async Task<Song> GetSongById(int id)
        {
            await Init();
            Song song = await _database.GetWithChildrenAsync<Song>(id,recursive: true);
            return song;
        }
    }
}