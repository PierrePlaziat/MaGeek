using System;
using System.IO;
using Microsoft.Data.Sqlite;
using Plaziat.CommonWpf;
using System.Collections.Generic;
using MaGeek.AppBusiness;
using Mageek.AppData;

namespace MaGeek.AppData
{

    public class SqliteDbManager
    {

        SqliteDbInfos DbInfos = new SqliteDbInfos(App.Config.Path_Db, AppFramework.AppConfig.GetSqliteDbCreationString());

        #region Context gestion

        internal MageekDbContext GetNewContext()
        {
            return new MageekDbContext(DbInfos);
        }

        #endregion

        #region Bulk Manipulations

        public SqliteDbManager()
        {
            RestorationGestion();
            if (!File.Exists(App.Config.Path_Db)) CreateDbFromScratch();

        }

        public void Backup()
        {
            string saveFolder = SelectFileHelper.SelectAFolder();
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
                    MessageBoxHelper.ShowMsg("DB saved successfully.");
                }
                catch (IOException iox)
                {
                    MessageBoxHelper.ShowMsg("DB save failed : " + iox.Message);
                }
            }
        }

        public void EraseDb()
        {
            if (!MessageBoxHelper.AskUser("Do you really want to erase all data?")) return;
            DeleteAllContent();
        }

        public void RestoreDb()
        {
            if (!MessageBoxHelper.AskUser("Current data will be lost, ensure you have a backup if needed.\n Are you sure you still want to launch restoration?")) return;
            string tmpDbPath = App.Config.Path_Db + ".tmp";
            string loadFile = SelectFileHelper.SelectAFile("Db Files (.db)|*.db");
            if (loadFile != null)
            {
                try
                {
                    File.Copy(loadFile, tmpDbPath);
                    DeleteAllContent();
                }
                catch (IOException iox)
                {
                    MessageBoxHelper.ShowMsg("DB load failed : " + iox.Message);
                }
            }
        }

        private void CreateDbFromScratch()
        {
            SqliteConnection dbCo = new SqliteConnection(DbInfos.ConnexionString);
            dbCo.Open();
            foreach (string instruction in DbInfos.Tables) new SqliteCommand(instruction, dbCo).ExecuteNonQuery();

            using (var DB = GetNewContext())
            {
                DB.SaveChanges();
            }
            dbCo.Close();
        }

        private void DeleteAllContent()
        {
            using (var DB = GetNewContext())
            {
                DB.DeleteAllContent();
            }
        }

        private void RestorationGestion()
        {
            string tmpDbPath = App.Config.Path_Db + ".tmp";
            if (File.Exists(tmpDbPath))
            {
                File.Delete(App.Config.Path_Db);
                File.Copy(tmpDbPath, App.Config.Path_Db);
                File.Delete(tmpDbPath);
            }
        }

        private string GenerateBackupName()
        {
            string s = System.Windows.Application.ResourceAssembly.GetName() + DateTime.Now.ToString() + ".db";
            s = s.Replace('/', '_').Replace(':', '_');
            return s;
        }

        #endregion

    }

}
