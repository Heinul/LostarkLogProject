using System;
using System.Data.SQLite;
using System.Text;

namespace LostarkLogProject.TripodLog
{
    internal class TripodDBManager
    {
        string DBpath = "Data Source=" + Application.StartupPath + "LLDatabase.db";
        SQLiteDataAdapter adapter = null;

        public TripodDBManager()
        {
            CreateTable();
        }

        private void CreateTable()
        {
            using (SQLiteConnection conn = new SQLiteConnection(DBpath))
            {
                string tablecheckQuery = @"SELECT COUNT(*) FROM sqlite_master WHERE Name = 'TRIPODDATA'";
                conn.Open();

                SQLiteCommand cmd1 = new SQLiteCommand(tablecheckQuery, conn);
                int resrult = Convert.ToInt32(cmd1.ExecuteScalar());
                if (resrult < 1)
                {
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(@"CREATE TABLE ""TRIPODDATA"" (");
                    sql.AppendLine(@" ""PERCENTAGE"" INTAGER, ");
                    sql.AppendLine(@" ""SUCCESS"" BOOLEAN,");
                    sql.AppendLine(@" ""TIMESTAMP"" INTAGER NOT NULL");
                    sql.AppendLine(@" ); ");

                    try
                    {
                        SQLiteCommand cmd = new SQLiteCommand(sql.ToString(), conn);
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }
    }
}
