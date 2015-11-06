// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatabaseSeeder.cs" company="Tapmobi">
//   Copyright (c) Tapmobi
// </copyright>
// <summary>
//   The database seeder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Data.Entity;
using System.IO;
using MySql.Data.MySqlClient;

namespace Data.Initializers.Seeders
{
    public static class DatabaseSeeder
    {
        #region Public Methods and Operators

        public static void LoadFileToDb(string connStr, string tableName, string filePath)
        {
            if (filePath == null || !File.Exists(filePath))
            {
                return;
            }

            var conn = new MySqlConnection(connStr);

            var bl = new MySqlBulkLoader(conn)
                {
                   TableName = tableName, FieldTerminator = ",", LineTerminator = "\r\n", FileName = filePath 
                };
            conn.Open();

            MySqlTransaction trans = conn.BeginTransaction();

            try
            {
                // Upload data from file
                int count = bl.Load();
                trans.Commit();
                conn.Close();
            }
            catch (Exception ex)
            {
                trans.Rollback();
            }
        }

        public static void Seed(DbContext context)
        {
           
        }

        #endregion

        #region Methods

        private static string GetContentDirectory()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string contentDir = Path.Combine(baseDir, "Content");
            return contentDir;
        }

        #endregion
    }
}