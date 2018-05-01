using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using System.Configuration;

namespace OEPrepForBilling
{
    public partial class ManualImport : Form
    {
        private Boolean isTesting = Convert.ToBoolean(ConfigurationManager.AppSettings["IsTest"].ToString());
        String[] sFileContents;
        String sConnString = String.Empty;

        public ManualImport()
        {
            InitializeComponent();
            if (ConfigurationManager.AppSettings["IsTest"].ToString() == "false")
                sConnString = "DATA02Connection";
            else
                sConnString = "DATA802TestConnection";
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            DialogResult result = ofdImportFile.ShowDialog();
            if (result == DialogResult.OK)
            {
                string file = ofdImportFile.FileName;
                this.txtSelectedFile.Text = file.ToString();
                sFileContents = File.ReadAllLines(file);
            }
        }

        private void ImportFileData(String[] sFileContents)
        {
            int iRecordCount = 0;
            SqlConnection sqlCon = new SqlConnection(ConfigurationManager.ConnectionStrings[sConnString].ConnectionString);
            String sSQL = "dbo.sp_ManuallyImportInvoicingFileData";

            SqlCommand sqlCmd = new SqlCommand(sSQL, sqlCon);

            try
            {
                foreach (String line in sFileContents)
                {
                    String[] cols = line.Split(',');
                    if (cols[0].ToString() == "ord_no")
                        continue;

                    sqlCmd = new SqlCommand(sSQL, sqlCon);
                    sqlCmd.CommandText = sSQL;
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Connection = sqlCon;
                    sqlCmd.Parameters.AddWithValue("@ord_no", cols[0].ToString());
                    sqlCmd.Parameters.AddWithValue("@item_no", cols[1].ToString());
                    sqlCmd.Parameters.AddWithValue("@qty_ordered", Convert.ToInt32(cols[2].ToString()));
                    sqlCmd.Parameters.AddWithValue("@req_ship_dt", Convert.ToInt32(cols[3].ToString()));
                    sqlCmd.Parameters.AddWithValue("@sfo", cols[4].ToString());
                    sqlCmd.Parameters.AddWithValue("@oe_po_no", cols[5].ToString());
                    sqlCmd.Parameters.AddWithValue("@entered_dt", Convert.ToInt32(cols[6].ToString()));
                    if (sqlCon.State != ConnectionState.Open)
                        sqlCon.Open();

                    try
                    {
                        sqlCmd.ExecuteNonQuery();
                        iRecordCount++;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error importing transaction data: " + ex.Message, "Import File Data");
                    }
                }

            }
            finally
            {
                MessageBox.Show(String.Format("{0} Transaction records imported to dbo.InvoiceThese succsfully", iRecordCount.ToString()), "Import File Data");
                sqlCmd.Dispose();
                sqlCon.Close();
                sqlCon.Dispose();
            }
        }

        private Boolean FlushInvoiceTheseTableData()
        {
            String sSQL = "TRUNCATE TABLE dbo.InvoiceThese";
            SqlConnection sqlCon = new SqlConnection(ConfigurationManager.ConnectionStrings[sConnString].ConnectionString);
            SqlCommand sqlCmd = new SqlCommand(sSQL, sqlCon);
            sqlCon.Open();
            try
            {
                sqlCmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Truncating dbo.InvoiceThese: " + ex.Message, "Manaul Data Import");
                return false;
            }
            finally
            {
                sqlCon.Close();
                sqlCmd.Dispose();
            }
        }

        private void btnImportFileData_Click(object sender, EventArgs e)
        {
            if (sFileContents.Length > 0)
                FlushInvoiceTheseTableData();

            ImportFileData(sFileContents);
        }
    }
}
