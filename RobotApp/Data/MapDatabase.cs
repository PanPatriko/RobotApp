using RobotApp.Models;
using SQLite;
using SQLiteNetExtensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RobotApp.Data
{
    public class MapDatabase
    {
        readonly SQLiteAsyncConnection _database;

        public MapDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<MapItem>().Wait();
        }

        public Task<List<MapItem>> GetMapsAsync()
        {
           return SQLiteNetExtensionsAsync.Extensions.ReadOperations.GetAllWithChildrenAsync<MapItem>(_database);
           // return _database.Table<MapItem>().ToListAsync();
        }

       // public Task<MapItem> GetMapAsync(int id)
      //  {
      //      return _database.Table<MapItem>()
     //                       .Where(i => i.ID == id)
      //                      .FirstOrDefaultAsync();
     //   }

        public Task SaveMapAsync(MapItem map)
        {
            if (map.ID != 0)
            {
                return  SQLiteNetExtensionsAsync.Extensions.WriteOperations.UpdateWithChildrenAsync(_database, map);
               // return _database.UpdateAsync(map);
            }
            else
            {
                return SQLiteNetExtensionsAsync.Extensions.WriteOperations.InsertWithChildrenAsync(_database, map);
               // return _database.InsertAsync(map);
            }
        }

        public Task DeleteMapAsync(MapItem map)
        {
            return SQLiteNetExtensionsAsync.Extensions.WriteOperations.DeleteAsync(_database, map, true);
            // return _database.DeleteAsync(map);
        }
    }
}
