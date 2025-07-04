using LiteDB;
using NirvanaMAUI.Models;
using System.Collections.ObjectModel;

namespace NirvanaMAUI.Services
{
    public class ProfileStorageService
    {
        private static readonly string DatabasePath =
            Path.Combine(FileSystem.AppDataDirectory, "nirvana_profiles.db");

        private const string CollectionName = "profiles";

        public bool SaveProfiles(IEnumerable<Profile> profiles)
        {
            try
            {
                using var db = new LiteDatabase(DatabasePath);
                var col = db.GetCollection<Profile>(CollectionName);
                col.DeleteAll();
                col.InsertBulk(profiles);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar: {ex.Message}");
                return false;
            }
        }

        public ObservableCollection<Profile> LoadProfiles()
        {
            using var db = new LiteDatabase(DatabasePath);
            var col = db.GetCollection<Profile>(CollectionName);
            var list = col.FindAll().ToList();

            return new ObservableCollection<Profile>(list);
        }

        public void Reset()
        {
            try
            {
                if (File.Exists(DatabasePath))
                    File.Delete(DatabasePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao resetar banco: {ex.Message}");
            }
        }
    }
}
