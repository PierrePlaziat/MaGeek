using System;
using System.IO;
using Microsoft.Data.Sqlite;
using MaGeek.AppFramework.UI.Utils;

namespace MaGeek.Framework.Data
{

    public class SqliteDbManager
    {

        public AppDbContext NewContext { get { return new AppDbContext(DbInfos); } }  //TODO use a simple DbContext for more abstraction

        #region CTOR

        private SqliteDbInfos DbInfos;

        private SqliteDbManager()
        {
            throw new Exception("SqliteDbManager needs informations");
        }
 
        public SqliteDbManager(string path, string[] tables)
        {
            DbInfos = new SqliteDbInfos(path, tables);
            InitDb();
        }
 
        public SqliteDbManager(SqliteDbInfos dbInfos)
        {
            DbInfos = dbInfos;
            InitDb();
        }

        private void InitDb()
        {
            if (File.Exists(App.Config.Path_Db_ToRestore))
                ProceedToRestore();
            if (!File.Exists(App.Config.Path_Db))
                CreateDbFromScratch();
        }

        #endregion

        #region Methods

        private void CreateDbFromScratch()
        {
            SqliteConnection dbCo = new SqliteConnection(DbInfos.ConnexionString);
            dbCo.Open();
            foreach (string instruction in DbInfos.Tables) new SqliteCommand(instruction, dbCo).ExecuteNonQuery();

            using (var DB = NewContext)
            {
                DB.SaveChanges();
            }
            dbCo.Close();
        }

        public void Backup()
        {
            string saveFolder = BrowserHelper.SelectAFolder();
            if (saveFolder != null)
            {
                try
                {
                    using (var localDb = new SqliteConnection(DbInfos.ConnexionString))
                    using (var backupDb = new SqliteConnection("Data Source = " + Path.Combine(saveFolder, GenerateBackupName())))
                    {
                        localDb.Open();
                        backupDb.Open();
                        localDb.BackupDatabase(backupDb);
                        localDb.Close();
                        backupDb.Close();
                    }
                    Log.Write("DB saved successfully.");
                }
                catch (IOException iox)
                {
                    Log.Write("DB save failed : " + iox.Message);
                }
            }
        }
        private string GenerateBackupName()
        {
            string s = System.Windows.Application.ResourceAssembly.GetName() + DateTime.Now.ToString() + ".db";
            s = s.Replace('/', '_').Replace(':', '_');
            return s;
        }


        public void PlanToRestore()
        {
            if (!Log.AskUser("Current data will be lost, ensure you have a backup if needed.\n Are you sure you still want to launch restoration?")) return;
            string tmpDbPath = App.Config.Path_Db + ".tmp";
            string loadFile = BrowserHelper.SelectAFile("Db Files (.db)|*.db");
            if (loadFile != null)
            {
                try
                {
                    File.Copy(loadFile, tmpDbPath);
                    ProceedToErase();
                }
                catch (IOException iox)
                {
                    Log.Write("DB load failed : " + iox.Message);
                }
            }
        }
        private void ProceedToRestore()
        {
            File.Delete(App.Config.Path_Db);
            File.Copy(App.Config.Path_Db_ToRestore, App.Config.Path_Db);
            File.Delete(App.Config.Path_Db_ToRestore);
        }

        public void PlanToErase()
        {
            if (Log.AskUser("Do you really want to erase all data?")) ProceedToErase();
        }
        private void ProceedToErase()
        {
            throw new Exception("DB erased unlinked for now.");
            //using (var DB = GetNewContext)
            //{
                //TODO use reflection
                //CardModels.ExecuteDeleteAsync();
                //CardVariants.ExecuteDeleteAsync();
                //CardLegalities.ExecuteDeleteAsync();
                //CardRules.ExecuteDeleteAsync();
                //CardRelations.ExecuteDeleteAsync();
                //Decks.ExecuteDeleteAsync();
                //DeckCards.ExecuteDeleteAsync();
                //CardTags.ExecuteDeleteAsync();
                //App.Restart();
            //}
        }

        #endregion

    }

}
