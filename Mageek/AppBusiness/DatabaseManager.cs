using System;
using System.IO;
using Microsoft.Data.Sqlite;
using Plaziat.CommonWpf;
using MaCore.Sqlite;
using System.Collections.Generic;
using System.Linq;

namespace MaGeek.AppBusiness
{

    public class MageekDbHandler
    {

        /// <summary>
        /// Tables have to match the Db Context
        /// </summary>
        public SqliteDbInfos DbInfos = new SqliteDbInfos(App.Config.Path_Db, App.Config.GetSqliteDbCreationString());

        /// <summary>
        /// Use this to access database concurrently
        /// </summary>
        public MageekDbContext GetNewContext()
        {
            return new MageekDbContext(DbInfos);
        }

        #region Methods

        public List<string> GetTagsDistinct(MageekDbContext dbContext)
        {
            List<string> tags = new List<string>();
            tags.Add("");
            foreach (var tag in dbContext.Tags.GroupBy(x => x.Tag).Select(x => x.First()))
                tags.Add(tag.Tag);
            return tags;
        }

        public void InitDb(MageekDbContext dbContext)
        {
            RestorationGestion();
            if (!File.Exists(App.Config.Path_Db)) CreateDbFromScratch(dbContext);
        }

        private void CreateDbFromScratch(MageekDbContext dbContext)
        {
            SqliteConnection dbCo = new SqliteConnection(DbInfos.ConnexionString);
            dbCo.Open();
            foreach (string instruction in DbInfos.Tables) new SqliteCommand(instruction, dbCo).ExecuteNonQuery();
            dbContext.SaveChanges();
            dbCo.Close();
        }

        public void BackupDb()
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

        public void EraseDb(MageekDbContext dbContext)
        {
            if (!MessageBoxHelper.AskUser("Do you really want to erase all data?")) return;
            dbContext.DeleteAllContent();
        }

        public void RestoreDb(MageekDbContext dbContext)
        {
            if (!MessageBoxHelper.AskUser("Current data will be lost, ensure you have a backup if needed.\n Are you sure you still want to launch restoration?")) return;
            string tmpDbPath = App.Config.Path_Db + ".tmp";
            string loadFile = SelectFileHelper.SelectAFile("Db Files (.db)|*.db");
            if (loadFile != null)
            {
                try
                {
                    File.Copy(loadFile, tmpDbPath);
                    dbContext.DeleteAllContent();
                }
                catch (IOException iox)
                {
                    MessageBoxHelper.ShowMsg("DB load failed : " + iox.Message);
                }
            }
        }

        private string GenerateBackupName()
        {
            string s = "MaGeek " + DateTime.Now.ToString() + ".db";
            s = s.Replace('/', '_').Replace(':', '_');
            return s;
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

        #endregion

    }

}
