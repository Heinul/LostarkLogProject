using System;
using System.Data;
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
                    sql.AppendLine(@" ""ADDITIONALMATERIAL"" BOOLEAN,");
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
       
        public DataRowCollection Select(int percentage)
        {
            try
            {
                DataSet ds = new DataSet();

                string sql = $"SELECT * FROM TRIPODDATA WHERE PERCENTAGE = {percentage}";
                adapter = new SQLiteDataAdapter(sql, DBpath);
                adapter.Fill(ds);
                //Console.WriteLine(sql);

                if (ds.Tables.Count > 0)
                    return ds.Tables[0].Rows;
                else
                    return null;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                throw;
            }
        }

        public DataRowCollection Select(int percentage, bool material)
        {
            try
            {
                DataSet ds = new DataSet();

                string sql = $"SELECT * FROM TRIPODDATA WHERE PERCENTAGE = {percentage} AND ADDITIONALMATERIAL = {material}";
                adapter = new SQLiteDataAdapter(sql, DBpath);
                adapter.Fill(ds);
                //Console.WriteLine(sql);

                if (ds.Tables.Count > 0)
                    return ds.Tables[0].Rows;
                else
                    return null;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                throw;
            }
        }

        public DataRowCollection Select(int percentage, bool material, bool success)
        {
            try
            {
                DataSet ds = new DataSet();

                string sql = $"SELECT * FROM TRIPODDATA WHERE PERCENTAGE = {percentage} AND ADDITIONALMATERIAL = {material} AND SUCCESS = {success} ";
                adapter = new SQLiteDataAdapter(sql, DBpath);
                adapter.Fill(ds);
                //Console.WriteLine(sql);

                if (ds.Tables.Count > 0)
                    return ds.Tables[0].Rows;
                else
                    return null;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                throw;
            }
        }

        public void Insert(int percentage, bool success, bool meterial)
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(DBpath))
                {
                    conn.Open();
                    string sql = $"INSERT INTO TRIPODDATA('PERCENTAGE', 'SUCCESS', 'ADDITIONALMATERIAL', 'TIMESTAMP') VALUES ({percentage}, {success}, {meterial}, {DateTime.Now.Ticks})";
                    SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                    Console.WriteLine(cmd.CommandText);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                throw;
            }
        }
    }
}
