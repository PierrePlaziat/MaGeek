﻿using System;
using System.IO;
using Microsoft.Data.Sqlite;
using Plaziat.CommonWpf;
using MaGeek.AppBusiness;
using Mageek.AppData;

namespace MaGeek.AppData
{

    public class SqliteDbManager
    {

        static string[] SqliteDbCreationString = new string[] {
            "CREATE TABLE \"CardRelations\" (\r\n\t\"RelationId\"\tINTEGER,\r\n\t\"Card1Id\"\tTEXT,\r\n\t\"Card2Id\"\tTEXT,\r\n\t\"LastUpdate\"\tTEXT,\r\n\t\"RelationType\"\tTEXT,\r\n\tPRIMARY KEY(\"RelationId\")\r\n);",
            "CREATE TABLE \"CardTraductions\" (\r\n\t\"TraductionId\"\tINTEGER,\r\n\t\"CardId\"\tTEXT,\r\n\t\"Language\"\tTEXT,\r\n\t\"TraductedName\"\tTEXT,\r\n\tPRIMARY KEY(\"TraductionId\")\r\n);",
            "CREATE TABLE \"CardValues\" (\r\n\t\"MultiverseId\"\tTEXT,\r\n\t\"LastUpdate\"\tTEXT,\r\n\t\"ValueEur\"\tTEXT,\r\n\t\"ValueUsd\"\tTEXT,\r\n\t\"EdhRecRank\"\tINTEGER,\r\n\tPRIMARY KEY(\"MultiverseId\")\r\n);",
            "CREATE TABLE \"CardVariants\" (\r\n\t\"Id\"\tTEXT,\r\n\t\"MultiverseId\"\tTEXT,\r\n\t\"ImageUrl_Front\"\tTEXT,\r\n\t\"Rarity\"\tTEXT,\r\n\t\"SetName\"\tTEXT,\r\n\t\"CardId\"\tTEXT,\r\n\t\"IsCustom\"\tINTEGER,\r\n\t\"CustomName\"\tTEXT,\r\n\t\"Got\"\tINTEGER,\r\n\t\"LastUpdate\"\tTEXT,\r\n\t\"Lang\"\tTEXT,\r\n\t\"TraductedTitle\"\tTEXT,\r\n\t\"TraductedText\"\tTEXT,\r\n\t\"TraductedType\"\tTEXT,\r\n\t\"ValueEur\"\tTEXT,\r\n\t\"ValueUsd\"\tTEXT,\r\n\t\"EdhRecRank\"\tINTEGER,\r\n\t\"Artist\"\tTEXT,\r\n\t\"ImageUrl_Back\"\tTEXT,\r\n\t\"Watermark\"\tTEXT\r\n);",
            "CREATE TABLE \"Cards\" (\r\n\t\"CardId\"\tTEXT,\r\n\t\"Type\"\tTEXT,\r\n\t\"ManaCost\"\tTEXT,\r\n\t\"Cmc\"\tINTEGER,\r\n\t\"Text\"\tTEXT,\r\n\t\"Power\"\tTEXT,\r\n\t\"Toughness\"\tTEXT,\r\n\t\"FavouriteVariant\"\tTEXT,\r\n\t\"ColorIdentity\"\tTEXT,\r\n\t\"DevotionB\"\tINTEGER,\r\n\t\"DevotionW\"\tINTEGER,\r\n\t\"DevotionU\"\tINTEGER,\r\n\t\"DevotionR\"\tINTEGER,\r\n\t\"DevotionG\"\tINTEGER\r\n);",
            "CREATE TABLE \"CardsInDecks\" (\r\n\t\"DeckId\"\tINTEGER,\r\n\t\"CardId\"\tTEXT,\r\n\t\"Quantity\"\tINTEGER,\r\n\t\"RelationType\"\tINTEGER,\r\n\tPRIMARY KEY(\"CardId\",\"DeckId\")\r\n);",
            "CREATE TABLE \"Decks\" (\r\n\t\"DeckId\"\tINTEGER,\r\n\t\"Title\"\tTEXT,\r\n\t\"Description\"\tTEXT,\r\n\t\"DeckColors\"\tTEXT,\r\n\t\"CardCount\"\tINTEGER,\r\n\tPRIMARY KEY(\"DeckId\")\r\n);",
            "CREATE TABLE \"Legalities\" (\r\n\t\"LegalityId\"\tINTEGER,\r\n\t\"CardId\"\tTEXT,\r\n\t\"LastUpdate\"\tTEXT,\r\n\t\"Format\"\tTEXT,\r\n\t\"IsLegal\"\tTEXT,\r\n\tPRIMARY KEY(\"LegalityId\")\r\n);",
            "CREATE TABLE \"Params\" (\r\n\t\"ParamName\"\tTEXT,\r\n\t\"ParamValue\"\tTEXT\r\n);",
            "CREATE TABLE \"Tags\" (\r\n\t\"Id\"\tINTEGER,\r\n\t\"Tag\"\tTEXT,\r\n\t\"CardId\"\tINTEGER,\r\n\tPRIMARY KEY(\"Id\")\r\n);",
        };

        SqliteDbInfos DbInfos = new SqliteDbInfos(App.Config.Path_Db, SqliteDbCreationString);

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
