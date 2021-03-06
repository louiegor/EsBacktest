﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormData
{
    public class ExcelReader
    {
        public void GetExcel()
        {
            var fileName = string.Format("{0}Book1.xlsx", "c:\\temp\\");
            var connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0; data source={0}; Extended Properties=Excel 12.0;", fileName);

            var adapter = new OleDbDataAdapter("SELECT * FROM [Sheet1]", connectionString);
            var ds = new DataSet();

            adapter.Fill(ds, "anyNameHere");

            DataTable data = ds.Tables["anyNameHere"];
        }

        public DataTable ReadExcelToTable(string path)
        {

            //Connection String

            string connstring = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties='Excel 8.0;HDR=NO;IMEX=1';";
            //the same name 
            //string connstring = Provider=Microsoft.JET.OLEDB.4.0;Data Source=" + path + //";Extended Properties='Excel 8.0;HDR=NO;IMEX=1';"; 

            using (OleDbConnection conn = new OleDbConnection(connstring))
            {
                conn.Open();
                //Get All Sheets Name
                DataTable sheetsName = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "Table" });

                //Get the First Sheet Name
                string firstSheetName = sheetsName.Rows[0][2].ToString();

                //Query String 
                string sql = string.Format("SELECT * FROM [{0}]", firstSheetName);
                OleDbDataAdapter ada = new OleDbDataAdapter(sql, connstring);
                DataSet set = new DataSet();
                ada.Fill(set);

                return  set.Tables[0];

            }
        }
    }
}
