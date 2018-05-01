using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Wisys.AllSystem;
using Wisys.Im;
using Wisys.Oe;
using Wisys.OeTrx;
using Wisys.MfgTrx;

namespace OEPrepForBilling
{
    public partial class OEPrepForBilling : Form
    {
        private Boolean isTesting = Convert.ToBoolean(ConfigurationManager.AppSettings["IsTest"].ToString());
        /* The IsTest value is NOT used to pull/modify data in the DEV/QA environment.
         * It is used to perform a ROLLBACK on all transactions to the PRODUCTION environment.
         * The SFOs don't exist, or match up, when attempting to pull/update data on the DEV/QA environment
         */
        private cConnection con;
        private String sFailureMessage = String.Empty;
        private String sErrorMessage = String.Empty;
        private int iRtn = 0;
        private bool bFound = false;
        private bool bAdjustQtyAll = false;
        private bool bConfirmShipAll = false;
        private bool bBillingAll = false;
        private bool bInvoiceAll = false;
        private bool bCheckBinnedOrSerialLot = false;
        private DataTable m_dtOpenOrders;
        private DataTable m_dtOrdersBeingPrepped;
        private DataTable m_dtItemToTransact;
        private DataTable m_dtFailed;

        private Boolean blnTransactCompleted = false;
        private Boolean blnConfirmShipCompleted = false;
        private Boolean blnSelectBillingComplete = false;
        private Boolean blnInvoiceCompleted = false;

        private Boolean blnDataModificationActionHasBeenTaken = false;

        private Boolean blnIsManualImport = false;

        /* FOR PRODUCTION */
        private String p_sServer = "MMMACSQL01";
        //  private String p_sDatabase = "DATA_02";
        private String p_MMDatabase = "DATA_02";

        /* FOR DEV/QA */
        //private String p_sServer = "MMQAWEBSQL01";
        //private String p_sDatabase = "DATA_102";
        //private String p_MMDatabase = "DATA_102";

        private const String mLocation = "SU";
        Wisys.AllSystem.ConnectionInfo p_oConnectionInfo = new ConnectionInfo();

        public OEPrepForBilling()
        {
            InitializeComponent();

            //this is to keep the app from working
            int HrsSinceLastRun = CheckLastInvoicedDate();
            int HrsBetweenRuns = Convert.ToInt32(ConfigurationManager.AppSettings["HoursBetweenRuns"].ToString());
            if (HrsSinceLastRun < HrsBetweenRuns)
            {
                MessageBox.Show(String.Format("It has only been {0} Hours since this application was run.\nThis functionality can only be performed once per day", HrsSinceLastRun.ToString()), "20 Hour Requirement");
                this.Enabled = false;
            }
            con = new cConnection();
            con.setConnString("DATA02");

            txtUserName.Text = Environment.UserName.ToString();

            if (Environment.UserName.ToString().Contains("norma") || Environment.UserName.ToString().Contains("keith"))
            {
                btnManualImport.Visible = true;
                btnManualImport.Enabled = true;
            }
            else
            {
                btnManualImport.Visible = false;
                btnManualImport.Enabled = false;
            }

            if (HrsSinceLastRun >= HrsBetweenRuns)
                BindDGVShipConfirm();
        }

        private void GetOpenOrders()
        {
            String sRtnErr = String.Empty;

            if (blnDataModificationActionHasBeenTaken)
                m_dtOpenOrders.Dispose();

            m_dtOpenOrders = new DataTable("OpenFMOrders");

            m_dtFailed = new DataTable("FailedOrders"); //This will be used to store the order number for any order that fails a process
            m_dtFailed.Columns.Add("ord_no", System.Type.GetType("System.String")); //Subsequent operations/process will skip these orders
            //posibly - make CheckBox disabled in grid.
            p_sServer = con.getServer();
            p_MMDatabase = con.getMMDB();
            String sConnString = con.getConnStringValue();
            SqlConnection sqlCon = new SqlConnection(sConnString);
            p_oConnectionInfo = new ConnectionInfo();
            p_oConnectionInfo.Parameters(p_sServer, p_MMDatabase, "sa", "m1llm4ts!");

            String sSQL = "dbo.spGetFMOrdersToInvoice";
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandText = sSQL;
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.Parameters.AddWithValue("@IsManualImport", blnIsManualImport);
            sqlCmd.Connection = sqlCon;
            sqlCmd.CommandTimeout = 0;

            SqlDataAdapter da = new SqlDataAdapter(sSQL, sqlCon);
            da.SelectCommand = sqlCmd;
            sqlCon.Open();
            try
            {
                da.Fill(m_dtOpenOrders);

                if (blnIsManualImport == true || m_dtOrdersBeingPrepped == null || m_dtOrdersBeingPrepped.Rows.Count == 0)
                    m_dtOrdersBeingPrepped = m_dtOpenOrders.Copy();

                RemoveOrdersNotBeingPrepped();
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Error Retrieving SLS Orderss to Process:{0}", ex.Message, "Data Retrieval Error"));
                return;
            }
            finally
            {
                sqlCmd.Dispose();
                sqlCon.Close();
                sqlCon.Dispose();
                da.Dispose();
            }
        }

        private void BindDGVShipConfirm()
        {
            if (m_dtOpenOrders == null || blnDataModificationActionHasBeenTaken)
                GetOpenOrders();

            if (m_dtOpenOrders == null || m_dtOpenOrders.Rows.Count == 0)
            {
                MessageBox.Show("No Orders matched the Transact/Ship Confirm criteria.", "No Data Returned");
                return;
            }

            if (m_dtOpenOrders.Columns["AdjustQty"] == null)
            {
                DataColumn cbAdjustQty = m_dtOpenOrders.Columns.Add("AdjustQty", System.Type.GetType("System.Boolean"));
                cbAdjustQty.ReadOnly = false;
                cbAdjustQty.Unique = false;
                cbAdjustQty.DefaultValue = false;
            }

            if (m_dtOpenOrders.Columns["Confirm"] == null)
            {
                DataColumn cbConfirm = m_dtOpenOrders.Columns.Add("Confirm", System.Type.GetType("System.Boolean"));
                cbConfirm.ReadOnly = false;
                cbConfirm.Unique = false;
                cbConfirm.DefaultValue = false;
            }

            if (m_dtOpenOrders.Columns["Billing"] != null)
                m_dtOpenOrders.Columns.Remove("Billing");

            if (m_dtOpenOrders.Columns["Invoice"] != null)
                m_dtOpenOrders.Columns.Remove("Invoice");

            dgvShipConfirm.DataBindings.Clear();

            DataView dv = new DataView(m_dtOpenOrders, null, "qty_ordered DESC,qty_to_ship DESC", DataViewRowState.CurrentRows);
            dgvShipConfirm.DataSource = dv;

            dgvShipConfirm.Columns["bin_no"].Visible = false;
            dgvShipConfirm.Columns["ord_status"].Visible = false;
            dgvShipConfirm.Columns["select_cd"].Visible = false;

            foreach (DataGridViewRow dgvRow in dgvShipConfirm.Rows)
            {
                String sOrderNo = CleanOrderLeftPaddedZeros(dgvRow.Cells["ord_no"].Value.ToString());
                DataView dvFailed = new DataView(m_dtFailed, String.Format("ord_no = '{0}' ", sOrderNo), "", DataViewRowState.CurrentRows);

                if ((dvFailed != null && dvFailed.Count > 0))
                {
                    DataGridViewCheckBoxCell cbTransact = dgvRow.Cells["AdjustQty"] as DataGridViewCheckBoxCell;
                    DataGridViewCheckBoxCell cbConfirm = dgvRow.Cells["Confirm"] as DataGridViewCheckBoxCell;
                    cbTransact.Value = false;
                    cbTransact.ReadOnly = true;
                    cbConfirm.Value = false;
                    cbConfirm.ReadOnly = true;
                    dgvRow.DataGridView.CommitEdit(DataGridViewDataErrorContexts.Formatting);
                }
                dvFailed.Dispose();
            }
            dgvShipConfirm.Refresh();

            if ((blnConfirmShipCompleted) && blnIsManualImport == false)
                btnConfirmShip.Enabled = false;
            if (blnTransactCompleted && blnIsManualImport == false)
                btnTransact.Enabled = false;
        }

        private void BindDGVBilling()
        {
            if (m_dtOpenOrders == null || m_dtOpenOrders.Rows.Count == 0 || blnDataModificationActionHasBeenTaken)
                GetOpenOrders();
            if (m_dtOpenOrders == null || m_dtOpenOrders.Rows.Count == 0)
            {
                MessageBox.Show("No Orders matched the Billing/Invoicing criteria.", "No Data Returned");
                return;
            }

            if (m_dtOpenOrders.Columns["Confirm"] != null)
            {
                m_dtOpenOrders.Columns.Remove("AdjustQty");
                m_dtOpenOrders.Columns.Remove("Confirm");
            }
            if (m_dtOpenOrders.Columns["Billing"] == null)
            {
                DataColumn cbBilling = m_dtOpenOrders.Columns.Add("Billing", System.Type.GetType("System.Boolean"));
                cbBilling.ReadOnly = false;
                cbBilling.Unique = false;
                cbBilling.DefaultValue = false;
            }
            if (m_dtOpenOrders.Columns["Invoice"] == null)
            {
                DataColumn cbInvoice = m_dtOpenOrders.Columns.Add("Invoice", System.Type.GetType("System.Boolean"));
                cbInvoice.ReadOnly = false;
                cbInvoice.Unique = false;
                cbInvoice.DefaultValue = false;
            }

            dgvBilling.DataBindings.Clear();
            DataView dvBilling = new DataView(m_dtOpenOrders, null, "qty_ordered DESC,qty_to_ship DESC", DataViewRowState.CurrentRows);
            dgvBilling.DataSource = dvBilling;

            dgvBilling.Columns["bin_no"].Visible = false;
            dgvBilling.Columns["ord_status"].Visible = false;
            dgvBilling.Columns["select_cd"].Visible = false;

            foreach (DataGridViewRow dgvRow in dgvShipConfirm.Rows)
            {
                String sOrderNo = CleanOrderLeftPaddedZeros(dgvRow.Cells["ord_no"].Value.ToString());
                DataView dvFailed = new DataView(m_dtFailed, String.Format("ord_no = '{0}'", sOrderNo), "", DataViewRowState.CurrentRows);

                if (dvFailed != null && dvFailed.Count > 0)
                {
                    DataGridViewCheckBoxCell cbBilling = dgvRow.Cells["Billing"] as DataGridViewCheckBoxCell;
                    DataGridViewCheckBoxCell cbInvoice = dgvRow.Cells["Invoice"] as DataGridViewCheckBoxCell;
                    cbBilling.Value = false;
                    cbBilling.ReadOnly = true;
                    cbInvoice.Value = false;
                    cbInvoice.ReadOnly = true;
                    dgvRow.DataGridView.CommitEdit(DataGridViewDataErrorContexts.Formatting);
                }
                dvFailed.Dispose();
            }
            if ((blnSelectBillingComplete) && blnIsManualImport == false)
                btnSelectBilling.Enabled = false;
            if ((blnInvoiceCompleted) && blnIsManualImport == false)
                btnInvoice.Enabled = false;
        }

        private Int32 OEConfirmShip(String sOrderNo, String sItemNo, String sLocation, short iLineSeqNo, Double dQty, Double dQtyToShip, String sBin)//,Double dQtyOrdered)//,Double dQtyCompl)
        {
            Wisys.OeTrx.ConfirmShip oCS = new ConfirmShip();
            Int32 iRtn = 0;
            String sRtnErr = String.Empty;
            Boolean isFound = false;
            Boolean isBinned = false;
            Boolean isSerialLot = false;
            Wisys.AllSystem.DataStructures.ItemBins oItemBins = new DataStructures.ItemBins();
            Wisys.AllSystem.DataStructures.ItemBin oItemBin;
            Wisys.AllSystem.DataStructures.ItemSerialLots oSerLots = new DataStructures.ItemSerialLots();
            Wisys.AllSystem.DataStructures.ItemSerialLot oSerLot;

            if (dQtyToShip == 0)
                dQtyToShip = dQty;
            else if (dQty > dQtyToShip)
                dQtyToShip = dQty - dQtyToShip;

            try
            {
                //set the connection of the object to the WisysTrans Connection
                oCS.Connection(ref p_oConnectionInfo);
                //privde transaction data
                oCS.ItemNumber = sItemNo;
                oCS.OrderNumber = CleanOrderLeftPaddedZeros(sOrderNo);
                oCS.Location = sLocation;
                oCS.UserName = txtUserName.Text;
                oCS.Quantity = dQtyToShip;
               // oCS.SequenceNumber = 1;
                oCS.LineNumber = iLineSeqNo;

                if (IsItemBinned(sItemNo, sLocation, ref isFound, ref isBinned) == Wisys.AllSystem.ReturnCodes.SUCCESSFUL)
                {
                    if (isFound && isBinned)
                    {
                        oItemBin = oItemBins.add(sBin, dQty);
                        oCS.IssueBins = oItemBins;
                        if (IsItemSerialLot(sItemNo, sLocation, ref isFound, ref isSerialLot) == Wisys.AllSystem.ReturnCodes.SUCCESSFUL)
                        {
                            if (isFound && isSerialLot)
                            {
                                oSerLot = oSerLots.Add("TESTERIALLOT1", dQty);
                                oCS.SerialLots = oSerLots;
                            }
                        }
                    }
                }

                //Send Transaction to Macola ES/Progression
                iRtn = oCS.PostTrx(ref sRtnErr);
                //check the result
                if (iRtn != Wisys.AllSystem.ReturnCodes.SUCCESSFUL)
                {
                    String Msg = String.Format("Error Performing Ship Confirm: {0}", sRtnErr);
                    //MessageBox.Show(Msg, "Ship Confirm Error");
                    m_dtFailed.Rows.Add(CleanOrderLeftPaddedZeros(sOrderNo));
                    sFailureMessage += String.Format("\nFAILED: Ship Confirm - Order:{0}\tItem:{1}\tQuantiy:{2}", sOrderNo, sItemNo, dQty.ToString());
                    sErrorMessage += String.Format("\nShip Confirm Error (Order:{0}\tItem:{1}\t - {2}", sOrderNo, sItemNo, sRtnErr);
                    return iRtn;
                }
                //good to go
                blnDataModificationActionHasBeenTaken = true;
                oCS = null;
                return Wisys.AllSystem.ReturnCodes.SUCCESSFUL;
            }
            catch (Exception ex)
            {
               // MessageBox.Show(String.Format("Error Performing Ship Confirm: {0}", ex.Message), "Ship Confirm Error");
                sFailureMessage += String.Format("\nFAILED: Ship Confirm - Order:{0}\tItem:{1}\tQuantiy:{2}", sOrderNo, sItemNo, dQty.ToString());
                sErrorMessage += String.Format("\nShip Confirm Error (Order:{0}\tItem:{1}\t - {2}", sOrderNo, sItemNo, ex.Message);
                m_dtFailed.Rows.Add(CleanOrderLeftPaddedZeros(sOrderNo));
                p_oConnectionInfo.CloseWisysConnection(Wisys.AllSystem.TrxEnums.TransactionAction.Rollback, ref sRtnErr);
                oCS = null;
                return Wisys.AllSystem.ReturnCodes.FAILED;
            }
        }

        private Int32 OESelectForBilling(String sOrderNo, String sItemNo, String sLocation, Int16 iLineNumber, Double dQty)
        {
            Wisys.OeTrx.PostBillingUpdates oPBU = new PostBillingUpdates();

            int iRtn = 0;
            String sRtnErr = String.Empty;
            String sCleanOrderNo = CleanOrderLeftPaddedZeros(sOrderNo);

            try
            {
                oPBU.Connection(ref p_oConnectionInfo);
                p_oConnectionInfo.OpenWisysConnection(true, ref sRtnErr);
                oPBU.OrderType = "O";
                oPBU.OrderNo = sCleanOrderNo;
                oPBU.UserName = txtUserName.Text.Trim();

                iRtn = oPBU.PostFinal(ref sRtnErr);
                if (iRtn != Wisys.AllSystem.ReturnCodes.SUCCESSFUL)
                {
                    sFailureMessage += String.Format("\nPost To Billing Failure - Order:{0}", sCleanOrderNo);
                    sErrorMessage += String.Format("\nSelect for Billing Error (Order:{0})\t - {1}", sOrderNo,  sRtnErr);
                    m_dtFailed.Rows.Add(sCleanOrderNo);
                    p_oConnectionInfo.CloseWisysConnection(Wisys.AllSystem.TrxEnums.TransactionAction.Rollback, ref sRtnErr);
                }
                else
                {
                    p_oConnectionInfo.CloseWisysConnection(Wisys.AllSystem.TrxEnums.TransactionAction.Commit, ref sRtnErr);
                }
                //good to go
                blnDataModificationActionHasBeenTaken = true;
                oPBU = null;
                return Wisys.AllSystem.ReturnCodes.SUCCESSFUL;
            }
            catch (Exception ex)
            {
                //exception handling code
                sFailureMessage += String.Format("\nFAILED: Post To Billing - Order:{0}\tItem:{1}\tQuantiy:{2}", sOrderNo, sItemNo, dQty.ToString());
                sErrorMessage += String.Format("\nSelect for Billing Error (Order:{0}\tItem:{1})\t - {2}", sOrderNo, sItemNo, ex.Message);
                m_dtFailed.Rows.Add(CleanOrderLeftPaddedZeros(sOrderNo));
                p_oConnectionInfo.CloseWisysConnection(Wisys.AllSystem.TrxEnums.TransactionAction.Rollback, ref sRtnErr);
                oPBU = null;
                return Wisys.AllSystem.ReturnCodes.FAILED;
            }
        }

        #region Wisys Item Inventory Checks
        private int IsItemSerialLot(String sItemNo, String sLocation, ref Boolean bExists, ref  Boolean bSerialLot)
        {
            Wisys.Im.InventoryTables oItem = new InventoryTables();
            int iRtn = 0;
            String sErrorMsg = String.Empty;
            Boolean bFound = false;
            bExists = false;
            bSerialLot = false;
            //Pass the connection object
            oItem.Connection(ref p_oConnectionInfo);
            //perform the lookup
            iRtn = oItem.Imitmidx.Read(sItemNo, sLocation, false, ref bFound, ref sErrorMsg);
            //check if found
            if (bFound)
            {
                //item exists
                bExists = true;
                if (oItem.Imitmidx.SerialLotFlag < Wisys.Im.InventoryTables.SerialLotTypes.Neither)
                {
                    //item is serial/lotted
                    bSerialLot = true;
                    return Wisys.AllSystem.ReturnCodes.SUCCESSFUL;
                }
                else
                {
                    //item is not serial/lotted
                    bSerialLot = false;
                    return Wisys.AllSystem.ReturnCodes.SUCCESSFUL;
                }
            }
            else
            {
                //nothing found
                bExists = false;
                bSerialLot = false;
                return Wisys.AllSystem.ReturnCodes.FAILED;
            }
            // return Wisys.AllSystem.ReturnCodes.FAILED;
        }

        private int IsItemBinned(String sItemNo, String sLocation, ref Boolean bExists, ref Boolean bBinned)
        {
            Wisys.Im.InventoryTables oItem = new InventoryTables();
            int iRtrn = 0;
            String sErrorMsg = String.Empty;
            Boolean bFound = false;
            bExists = false;
            bBinned = false;
            //pass the connection object
            oItem.Connection(ref p_oConnectionInfo);
            //perform the lookup
            iRtrn = oItem.Imitmidx.Read(sItemNo, sLocation, false, ref bFound, ref sErrorMsg);
            //check if found
            if (bFound)
            {
                //item exists
                bExists = true;
                if (oItem.Imitmidx.Iminvloc.Binned == true)
                {
                    bBinned = true;
                    return Wisys.AllSystem.ReturnCodes.SUCCESSFUL;
                }
                else
                {
                    //item is not binnded
                    bBinned = false;
                    return Wisys.AllSystem.ReturnCodes.SUCCESSFUL;
                }
            }
            else
            {
                //notyhing found
                bExists = false;
                bBinned = false;
                return Wisys.AllSystem.ReturnCodes.FAILED;
            }
        }
        #endregion

        #region Events

        private void btnConfirmShip_Click(object sender, EventArgs e)
        {
            //column indiices
            //Adjust Qty - 9
            //Confirm Ship - 10
            if (blnConfirmShipCompleted)
            {
                MessageBox.Show("Confirm Shipping has already been performed!", "Invalid Selection");
                return;
            }

            String sRtnErr = String.Empty;
            String sShipConfirmMsg = String.Empty;
            sFailureMessage = String.Empty;
            SetButtonStatus(false, 2);

            foreach (DataGridViewRow dgvRow in dgvShipConfirm.Rows)
            {
                DataGridViewCheckBoxCell cbCell = dgvRow.Cells["Confirm"] as DataGridViewCheckBoxCell;
                //call teh Business Logic Layer
                if (cbCell.Value != DBNull.Value && Convert.ToBoolean(cbCell.Value) == true)
                {
                    //Call the OpenWisysConnection Method from our instance of ConnectionInfo Object
                    p_oConnectionInfo.OpenWisysConnection(true, ref sRtnErr);

                    if (dgvRow.Cells["ord_status"].Value.ToString() != "7" && dgvRow.Cells["select_cd"].Value.ToString() != "S" && OEConfirmShip(dgvRow.Cells["ord_no"].Value.ToString(), dgvRow.Cells["item_no"].Value.ToString(), mLocation, Convert.ToInt16(dgvRow.Cells["line_seq_no"].Value.ToString()), Convert.ToDouble(dgvRow.Cells["qty_ordered"].Value.ToString().Trim()), Convert.ToDouble(dgvRow.Cells["qty_to_ship"].Value.ToString()), dgvRow.Cells["bin_no"].Value.ToString()) != Wisys.AllSystem.ReturnCodes.SUCCESSFUL)
                    {
                        sShipConfirmMsg += String.Format("\nOrder:\t{0}\tItem:\t{1} could not be Ship Confirmed.", dgvRow.Cells["ord_no"].Value.ToString(), dgvRow.Cells["item_no"].Value.ToString());
                        m_dtFailed.Rows.Add(CleanOrderLeftPaddedZeros(dgvRow.Cells["ord_no"].Value.ToString()));
                        p_oConnectionInfo.CloseWisysConnection(Wisys.AllSystem.TrxEnums.TransactionAction.Rollback, ref sRtnErr);
                        continue;
                    }
                    //Close Wisys Connection
                    p_oConnectionInfo.CloseWisysConnection(Wisys.AllSystem.TrxEnums.TransactionAction.Commit, ref sRtnErr);
                }
            }
            // Cursor.Current = Cursors.Default;
            SetButtonStatus(true, 2);

            String Msg = String.Empty;
            if (sShipConfirmMsg.Length > 0)
                Msg = String.Format("OE Confirm Shipping Errors ({0}):{1}", DateTime.Now.ToLongTimeString(), sShipConfirmMsg);

            if (sFailureMessage.Length > 0)
                Msg += sFailureMessage;

            if (Msg.Length > 0)
            {
                Notification n = new Notification();
                n.SetTitle("Confirm Shipping");
                n.SetMsg(Msg);
                n.Show();
                n.Focus();
            }
            else
            {
                MessageBox.Show("OE Confirm Shipping completed", "OE Confirm Shipping");
            }
            dgvShipConfirm.Refresh();
            btnConfirmShip.Enabled = false;
            blnConfirmShipCompleted = true;
        }

        private void dgvShipConfirm_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            //qty_orderd is index 6
            //qty_to_ship is index 7
            DataGridViewRow dgvRow = dgvShipConfirm.Rows[e.RowIndex];
            DataGridViewCellStyle dgvCellYellow = new DataGridViewCellStyle();
            dgvCellYellow.BackColor = Color.Yellow;

            if (dgvRow.Cells["qty_ordered"].Value == null || dgvRow.Cells["qty_to_ship"].Value == null || dgvRow.Cells["qty_ordered"].Value.ToString() == String.Empty || dgvRow.Cells["qty_to_ship"].Value.ToString() == String.Empty)
                dgvRow.DefaultCellStyle = dgvCellYellow;
            else if (Convert.ToInt32(dgvRow.Cells["qty_ordered"].Value.ToString()) != Convert.ToInt32(dgvRow.Cells["qty_to_ship"].Value.ToString()))
                dgvRow.DefaultCellStyle = dgvCellYellow;
        }

        private void cbAdjustQty_CheckedChanged(object sender, EventArgs e)
        {
            if (cbAdjustQty.Checked)
                bAdjustQtyAll = true;
            else
                bAdjustQtyAll = false;

            foreach (DataGridViewRow dgvRow in dgvShipConfirm.Rows)
            {
                String sOrderNo = CleanOrderLeftPaddedZeros(dgvRow.Cells["ord_no"].Value.ToString());

                DataView dvFailed = new DataView(m_dtFailed, String.Format("ord_no = '{0}'", sOrderNo), "", DataViewRowState.CurrentRows);

                if (dvFailed != null && dvFailed.Count > 0)
                {
                    DataGridViewCheckBoxCell cbTransact = dgvRow.Cells["AdjustQty"] as DataGridViewCheckBoxCell;
                    DataGridViewCheckBoxCell cbConfirm = dgvRow.Cells["Confirm"] as DataGridViewCheckBoxCell;
                    cbTransact.Value = false;
                    cbTransact.ReadOnly = true;
                    cbConfirm.Value = false;
                    cbConfirm.ReadOnly = true;
                    dgvRow.DataGridView.CommitEdit(DataGridViewDataErrorContexts.Formatting);
                }
                else
                {
                    if (Convert.ToInt16(dgvRow.Cells["qty_ordered"].Value) > Convert.ToInt16(dgvRow.Cells["qty_to_ship"].Value))
                    {
                        dgvRow.Cells["AdjustQty"].Value = bAdjustQtyAll;
                        ((DataGridViewCheckBoxCell)dgvRow.Cells["AdjustQty"]).Value = bAdjustQtyAll;
                    }
                }
                dvFailed.Dispose();
            }
        }

        private void cbConfirmShip_CheckedChanged(object sender, EventArgs e)
        {
            if (cbConfirmShip.Checked)
                bConfirmShipAll = true;
            else
                bConfirmShipAll = false;

            foreach (DataGridViewRow dgvRow in dgvShipConfirm.Rows)
            {
                String sOrderNo = CleanOrderLeftPaddedZeros(dgvRow.Cells["ord_no"].Value.ToString());

                DataView dvFailed = new DataView(m_dtFailed, String.Format("ord_no = '{0}'", sOrderNo), "", DataViewRowState.CurrentRows);

                if ((dvFailed != null && dvFailed.Count > 0) || dgvRow.Cells["ord_status"].Value.ToString() == "7" || dgvRow.Cells["select_cd"].Value.ToString() == "S")
                {
                    DataGridViewCheckBoxCell cbTransact = dgvRow.Cells["AdjustQty"] as DataGridViewCheckBoxCell;
                    DataGridViewCheckBoxCell cbConfirm = dgvRow.Cells["Confirm"] as DataGridViewCheckBoxCell;
                    cbTransact.Value = false;
                    cbTransact.ReadOnly = true;
                    cbConfirm.Value = false;
                    cbConfirm.ReadOnly = true;
                    dgvRow.DataGridView.CommitEdit(DataGridViewDataErrorContexts.Formatting);
                }
                else
                {
                    if (Convert.ToInt16(dgvRow.Cells["qty_ordered"].Value) == Convert.ToInt16(dgvRow.Cells["qty_to_ship"].Value))
                    {
                        dgvRow.Cells["Confirm"].Value = bConfirmShipAll;
                        ((DataGridViewCheckBoxCell)dgvRow.Cells["Confirm"]).Value = bConfirmShipAll;
                    }
                }
                dvFailed.Dispose();
            }
        }

        private void cbBilling_CheckedChanged(object sender, EventArgs e)
        {
            if (cbBilling.Checked)
                bBillingAll = true;
            else
                bBillingAll = false;

            foreach (DataGridViewRow dgvRow in dgvBilling.Rows)
            {
                String sOrderNo = CleanOrderLeftPaddedZeros(dgvRow.Cells["ord_no"].Value.ToString());

                DataView dvFailed = new DataView(m_dtFailed, String.Format("ord_no = '{0}'", sOrderNo), "", DataViewRowState.CurrentRows);

                if (dvFailed != null && dvFailed.Count > 0)
                {
                    DataGridViewCheckBoxCell cbTransact = dgvRow.Cells["Billing"] as DataGridViewCheckBoxCell;
                    DataGridViewCheckBoxCell cbConfirm = dgvRow.Cells["Invoice"] as DataGridViewCheckBoxCell;
                    cbTransact.Value = false;
                    cbTransact.ReadOnly = true;
                    cbConfirm.Value = false;
                    cbConfirm.ReadOnly = true;
                    dgvRow.DataGridView.CommitEdit(DataGridViewDataErrorContexts.Formatting);
                }
                else
                {
                    if (Convert.ToInt16(dgvRow.Cells["qty_ordered"].Value) == Convert.ToInt16(dgvRow.Cells["qty_to_ship"].Value))
                    {
                        dgvRow.Cells["Billing"].Value = bBillingAll;
                        ((DataGridViewCheckBoxCell)dgvRow.Cells["Billing"]).Value = bBillingAll;
                    }
                }
                dvFailed.Dispose();
            }
        }

        private void cbInvoice_CheckedChanged(object sender, EventArgs e)
        {
            if (cbInvoice.Checked)
                bInvoiceAll = true;
            else
                bInvoiceAll = false;

            foreach (DataGridViewRow dgvRow in dgvBilling.Rows)
            {
                String sOrderNo = CleanOrderLeftPaddedZeros(dgvRow.Cells["ord_no"].Value.ToString());

                DataView dvFailed = new DataView(m_dtFailed, String.Format("ord_no = '{0}'", sOrderNo), "", DataViewRowState.CurrentRows);

                if (dvFailed != null && dvFailed.Count > 0)
                {
                    DataGridViewCheckBoxCell cbTransact = dgvRow.Cells["Billing"] as DataGridViewCheckBoxCell;
                    DataGridViewCheckBoxCell cbConfirm = dgvRow.Cells["Invoice"] as DataGridViewCheckBoxCell;
                    cbTransact.Value = false;
                    cbTransact.ReadOnly = true;
                    cbConfirm.Value = false;
                    cbConfirm.ReadOnly = true;
                    dgvRow.DataGridView.CommitEdit(DataGridViewDataErrorContexts.Formatting);
                }
                else
                {
                    if (Convert.ToInt16(dgvRow.Cells["qty_ordered"].Value) == Convert.ToInt16(dgvRow.Cells["qty_to_ship"].Value))
                    {
                        dgvRow.Cells["Invoice"].Value = bInvoiceAll;
                        ((DataGridViewCheckBoxCell)dgvRow.Cells["Invoice"]).Value = bInvoiceAll;
                    }
                }
                dvFailed.Dispose();
            }
        }

        private void tcMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tcMain.SelectedIndex)
            {
                case 1:
                    BindDGVBilling();
                    break;
                case 0:
                    BindDGVShipConfirm();
                    break;
            }
        }

        private void btnInvoice_Click(object sender, EventArgs e)
        {
            if (blnInvoiceCompleted)
            {
                MessageBox.Show("Invalid Selection", "Invoicing has already been performed!");
                return;
            }
            SetButtonStatus(false, 4);
            sFailureMessage = String.Empty;
            String sRtnErrMsg = String.Empty;
            DateTime dtStartTime;
            DateTime dtInvoiceDate = Convert.ToDateTime(dtpInvoiceDate.Value.ToShortDateString());
            String sOrderNo = String.Empty;
            Wisys.Oe.OrderEntryTables oeOrderEntry = new OrderEntryTables();
            oeOrderEntry.Connection(ref p_oConnectionInfo, ref sRtnErrMsg);

            foreach (DataGridViewRow dgvRow in dgvBilling.Rows)
            {
                DataGridViewCheckBoxCell cbCell = dgvRow.Cells["Invoice"] as DataGridViewCheckBoxCell;

                if (cbCell.Value == DBNull.Value || Convert.ToBoolean(cbCell.Value) == false)
                    continue;

                p_oConnectionInfo.OpenWisysConnection(true, ref sRtnErrMsg);

                sOrderNo = CleanOrderLeftPaddedZeros(dgvRow.Cells["ord_no"].Value.ToString());
                dtStartTime = DateTime.Now;
                //invoice the order
                oeOrderEntry.Username = txtUserName.Text.Trim();
                if (oeOrderEntry.AdvanceToOrderInvoice(sOrderNo, dtInvoiceDate, ref sRtnErrMsg) != Wisys.AllSystem.ReturnCodes.SUCCESSFUL)
                {
                    m_dtFailed.Rows.Add(sOrderNo);
                    p_oConnectionInfo.CloseWisysConnection(TrxEnums.TransactionAction.Rollback, ref sRtnErrMsg);
                    sErrorMessage += String.Format("\nAdvance to Order Invoice Error (Order:{0})\t{1}", sOrderNo, sRtnErrMsg);
                }
                else
                {
                    p_oConnectionInfo.CloseWisysConnection(TrxEnums.TransactionAction.Commit, ref sRtnErrMsg);
                }
            }
            SetButtonStatus(true, 4);

            String Msg = String.Format("Invoicing Completed ({0}).", DateTime.Now.ToLongTimeString());
            if (sFailureMessage.Length > 0)
            {
                Msg += sFailureMessage;
                Notification n = new Notification();
                n.SetTitle("Advance to Order Invoice");
                n.SetMsg(Msg);
                n.Show();
                n.Focus();
            }
            else
            {
                MessageBox.Show(Msg, "Advance to Order Invoice");
            }

            if (sErrorMessage.Length > 0)
                SaveErrMsgLog();

            btnInvoice.Enabled = false;
            blnInvoiceCompleted = true;
            oeOrderEntry.Dispose();
            SetLastInvoicedDate();
        }

        private void btnSelectBilling_Click(object sender, EventArgs e)
        {
            if (blnSelectBillingComplete)
            {
                MessageBox.Show("Invalid Selection", "Select for Billing has already been performed!");
                return;
            }
            String sRtnErr = String.Empty;
            sFailureMessage = String.Empty;

            SetButtonStatus(false, 3);

            foreach (DataGridViewRow dgvRow in dgvBilling.Rows)
            {
                DataGridViewCheckBoxCell cbCell = dgvRow.Cells["Billing"] as DataGridViewCheckBoxCell;

                if (cbCell.Value != DBNull.Value && Convert.ToBoolean(cbCell.Value) == true)
                {
                    //call the OpenWIsysConnection Method from our instance of ConnectionInfo object
                    p_oConnectionInfo.OpenWisysConnection(true, ref sRtnErr);
                    //call the business logic layer
                    if (OESelectForBilling(dgvRow.Cells["ord_no"].Value.ToString(), dgvRow.Cells["item_no"].Value.ToString(), mLocation, Convert.ToInt16(dgvRow.Cells["line_seq_no"].Value.ToString()), Convert.ToDouble(dgvRow.Cells["qty_to_ship"].Value.ToString())) != Wisys.AllSystem.ReturnCodes.SUCCESSFUL)
                    {
                        m_dtFailed.Rows.Add(CleanOrderLeftPaddedZeros(dgvRow.Cells["ord_no"].Value.ToString()));
                        p_oConnectionInfo.CloseWisysConnection(Wisys.AllSystem.TrxEnums.TransactionAction.Rollback, ref sRtnErr);
                        continue;
                    }
                    p_oConnectionInfo.CloseWisysConnection(Wisys.AllSystem.TrxEnums.TransactionAction.Commit, ref sRtnErr);
                }
            }

            SetButtonStatus(true, 3);

            String Msg = String.Format("Select for Billing Completed.({0}).", DateTime.Now.ToLongTimeString());
            if (sFailureMessage.Length > 0)
            {
                Msg += sFailureMessage;
                Notification n = new Notification();
                n.SetTitle("Post/Select for Billing");
                n.SetMsg(Msg);
                n.Show();
                n.Focus();
            }
            else
            {
                MessageBox.Show(Msg, "Post/Select for Billing");
            }
            btnSelectBilling.Enabled = false;
            blnSelectBillingComplete = true;
        }

        private void btnTransact_Click(object sender, EventArgs e)
        {
            if (blnTransactCompleted)
            {
                MessageBox.Show("Invalid Selection", "Transact All has already been performed!");
                return;
            }
            SetButtonStatus(false, 1);

            sFailureMessage = String.Empty;
            foreach (DataGridViewRow dgvRow in dgvShipConfirm.Rows)
            {
                DataGridViewCheckBoxCell cbCell = dgvRow.Cells["AdjustQty"] as DataGridViewCheckBoxCell;

                if (cbCell.Value != DBNull.Value && Convert.ToBoolean(cbCell.Value) == true)
                {
                    m_dtItemToTransact = null;
                    GetItemToTransact(dgvRow.Cells["ord_no"].Value.ToString().PadLeft(10, '0'), dgvRow.Cells["item_no"].Value.ToString());
                    if (m_dtItemToTransact != null && m_dtItemToTransact.Rows.Count > 0)
                        TransactItem();
                }
            }
            SetButtonStatus(true, 1);

            String Msg = String.Format("Transactions completed.({0}).", DateTime.Now.ToLongTimeString());
            if (sFailureMessage.Length > 0)
            {
                Msg += sFailureMessage;
                Notification n = new Notification();
                n.SetTitle("Transact SFO");
                n.SetMsg(Msg);
                n.Show();
                n.Focus();
            }
            else
            {
                MessageBox.Show(Msg, "SF Transactions");
            }
            BindDGVShipConfirm();
            btnTransact.Enabled = false;
            blnTransactCompleted = true;
        }

        #endregion

        #region Helpers

        private void RemoveOrdersNotBeingPrepped()
        {
            foreach (DataRow row in m_dtOpenOrders.Rows)
            {
                String sOrder = row["ord_No"].ToString();
                String sItem = row["item_no"].ToString();
                DataView dv = new DataView(m_dtOrdersBeingPrepped, String.Format("ord_no='{0}' AND item_no='{1}'", sOrder, sItem), String.Empty, DataViewRowState.CurrentRows);
                if (dv == null || dv.Count == 0)
                    row.Delete();

                dv.Dispose();
            }
        }

        private void SetLastInvoicedDate()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            KeyValueConfigurationCollection settings = config.AppSettings.Settings;

            settings["LastInvoicedDate"].Value = DateTime.Now.ToString();

            //save the file
            config.Save(ConfigurationSaveMode.Modified);
            //relaod the section you modified
            ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
            config = null;
        }

        private int CheckLastInvoicedDate()
        {
            String sLastInvDt = ConfigurationManager.AppSettings["LastInvoicedDate"].ToString();
            DateTime dtLastInvDt = Convert.ToDateTime(sLastInvDt);
            TimeSpan timespan = DateTime.Now - dtLastInvDt;
            if (Environment.UserName.ToString() == "keithb")
                return 25;
            else
                return Convert.ToInt16(Math.Floor(timespan.TotalHours));
           // return 25;
        }

        private void SetButtonStatus(Boolean bEnable, Int16 iActionStep)
        {
            if (blnIsManualImport)
            {
                btnTransact.Enabled = true;
                btnConfirmShip.Enabled = true;
                btnSelectBilling.Enabled = true;
                btnInvoice.Enabled = true;
            }
            else
            {
                btnTransact.Enabled = iActionStep < 2 ? bEnable : false;
                btnConfirmShip.Enabled = iActionStep < 3 ? bEnable : false;
                btnSelectBilling.Enabled = iActionStep < 4 ? bEnable : false;
                btnInvoice.Enabled = iActionStep < 5 ? bEnable : false;
            }

            if (bEnable)
                Cursor.Current = Cursors.Default;
            else
                Cursor.Current = Cursors.WaitCursor;
        }

        private String CleanOrderLeftPaddedZeros(String ord)
        {
            int iCounter = 0;
            int iCount = ord.Trim().Length;
            int iZerosToRemove = 0;
            for (iCounter = 0; iCounter < iCount; iCounter++)
            {
                if (ord.Substring(iCounter, 1) == "0")
                    iZerosToRemove++;
                else
                    break;
            }
            return ord.Substring(iZerosToRemove, ord.Trim().Length - iZerosToRemove);
        }
        #endregion

        private void GetItemToTransact(String sOrderNo, String sItemNo)
        {
            String sRtnErrMsg = String.Empty;

            if (m_dtItemToTransact == null || m_dtItemToTransact.Rows.Count == 0)
                m_dtItemToTransact = new DataTable("ItemToTransact");

            String sSQL = "SELECT l.mfg_ord_no,l.ord_no, l.item_no,l.line_seq_no, qty_ordered, qty_to_ship,sfd.qty , sfd.qty_compl,i.rtg_no, " +
                          "  sfd.path_no,sfd.dept,sfd.wc, sfd.oper_no,sfd.oper_seq_no , " +
                          "  1 as isMaxSeqNo " +
                          "  FROM dbo.OEORDLIN_SQL l INNER JOIN dbo.OEORDHDR_SQL h ON h.ord_no = l.ord_no " +
                          "  INNER JOIN SFORDFIL_SQL sfo ON l.mfg_ord_no = sfo.ord_no AND l.item_no = sfo.item_no  " +
                          "  INNER JOIN SFDTLFIL_SQL sfd ON sfo.ord_No = sfd.ord_no and sfo.item_no = sfd.item_no  AND sfd.rec_type = 'O' " +
                          "  INNER JOIN IMITMIDX_SQL i ON i.item_No = l.item_no " +
                          "  WHERE RIGHT('0000000000' + h.ord_No,10) = '" + sOrderNo + "' " +
                          "  AND RTRIM(l.item_no) = RTRIM('" + sItemNo + "') " +
                          "  AND l.qty_ordered > sfd.qty_compl " +
                          "  ORDER BY h.user_def_fld_1, l.mfg_ord_no, l.ord_no, l.item_no, sfd.oper_no, sfd.oper_seq_no ";

            iRtn = p_oConnectionInfo.OpenWisysConnection(false, ref sRtnErrMsg);

            if (iRtn != 0)
            {
                MessageBox.Show(String.Format("Could not establish connection to {0}.{1}: {2}", p_sServer, p_MMDatabase, sRtnErrMsg), "Data Connection");
                return;
            }
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandText = sSQL;
            sqlCmd.CommandType = CommandType.Text;
            try
            {
                iRtn = p_oConnectionInfo.SqlProcessor.ReturnValues(sqlCmd, false, ref m_dtItemToTransact, ref  bFound, ref sRtnErrMsg);

                if (iRtn != 0)
                {
                    MessageBox.Show(String.Format("Error retrieving SF details for {0} - {1}.", sOrderNo, sItemNo), "Data Retrieval");
                    return;
                }
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Error retrieving SF details for {0} - {1}.", sOrderNo, sItemNo), "Data Retrieval");
                return;
            }
            finally
            {
                p_oConnectionInfo.CloseWisysConnection(TrxEnums.TransactionAction.NoTransaction, ref sRtnErrMsg);
                sqlCmd.Dispose();
            }
        }

        private void TransactItem()
        {
            String sRtnErrMsg = String.Empty;
            int iRowIndex = -1;
            int iTotalRowCount = m_dtItemToTransact.Rows.Count;

            foreach (DataRow row in m_dtItemToTransact.Rows)
            {
                p_oConnectionInfo.OpenWisysConnection(true, ref sRtnErrMsg);
                iRowIndex++;
                short iOperNo = Convert.ToInt16(row["oper_no"].ToString());
                short iOperSeqNo = Convert.ToInt16(row["oper_seq_no"].ToString());
                short iPathNo = 0;
                String sRtgNo = row["rtg_no"].ToString().Trim();
                double dQty = Convert.ToDouble(row["qty"].ToString());
                double dQtyOrdered = Convert.ToDouble(row["qty_ordered"].ToString());
                double dQtyToShip = Convert.ToDouble(row["qty_to_ship"].ToString());
                double dQtyCompl = Convert.ToDouble(row["qty_compl"].ToString());

                dQty = dQtyCompl;

                if (dQtyCompl == 0)
                    dQty = dQtyOrdered;
                else if (dQtyCompl < dQtyOrdered)
                    dQty = dQtyOrdered - dQtyCompl;

                String sSFO = row["mfg_ord_no"].ToString().Trim();
                String sItemNo = row["item_no"].ToString().Trim();
                String sOrderNo = row["ord_no"].ToString().Trim();
                String sLoc = row["wc"].ToString().Trim();
                String sDept = row["dept"].ToString().Trim();
                String sWC = row["wc"].ToString().Trim();
                short iLineSeqNo = Convert.ToInt16(row["line_seq_no"].ToString());
                Boolean bOperComplete = row["isMaxSeqNo"].ToString().Trim() == "0" ? false : true;

                if (SFCReportProduction(sOrderNo, sSFO, iOperNo, dQty, iPathNo, iLineSeqNo, iOperSeqNo, "", "", sItemNo, sLoc, bOperComplete, sDept, sWC) == Wisys.AllSystem.ReturnCodes.SUCCESSFUL)
                {
                    blnDataModificationActionHasBeenTaken = true;
                    p_oConnectionInfo.CloseWisysConnection(Wisys.AllSystem.TrxEnums.TransactionAction.Commit, ref sRtnErrMsg);
                }
                else
                {
                    p_oConnectionInfo.CloseWisysConnection(TrxEnums.TransactionAction.Rollback, ref sRtnErrMsg);
                }
            }
        }

        private int SFCReportProduction(String sOrderNo, String sMfgOrderNo, short sOperNo, Double dQty, short sPath, short iLineSeqNo, short sSeqNo, String sBin, String sLot, String sItemNo, String sLoc, Boolean bOperComplete, String sDept, String sWC)
        {
            Wisys.MfgTrx.ActivityTrx oSFC = new ActivityTrx();
            int iRtn = 0;
            String sRtnErr = String.Empty;

            //Call the OpenWIsysConnection method from our instance of COnnectionInfo object.     
            iRtn = p_oConnectionInfo.OpenWisysConnection(true, ref sRtnErr);
            if (iRtn != 0)
            {
                MessageBox.Show(String.Format("Could not establish connection to {0}.{1}: {2}", p_sServer, p_MMDatabase, sRtnErr));
                return -1;
            }

            try
            {
                //set the connection of the object to the WiSysTran connection
                oSFC.Connection(ref p_oConnectionInfo);
                //Provide Transaction DAta
                oSFC.OrderNo = sMfgOrderNo;
                oSFC.OpNo = sOperNo;
                oSFC.PathNo = sPath;
                oSFC.OpSeqNo = sSeqNo;

                oSFC.QtyComplete = dQty;
                oSFC.OpComplete = bOperComplete;
                oSFC.Department = sDept;
                oSFC.WorkCenter = sWC;
                oSFC.UserName = "OEPREPBILL";

                //send transaction to Macola ES/Progression
                iRtn = oSFC.ReportProduction(ref sRtnErr);
                if (iRtn != 0)
                {
                    p_oConnectionInfo.CloseWisysConnection(Wisys.AllSystem.TrxEnums.TransactionAction.Rollback, ref sRtnErr);
                    m_dtFailed.Rows.Add(CleanOrderLeftPaddedZeros(sOrderNo));
                    sFailureMessage += String.Format("\nTransaction Failed - SFO:{0}\tItem:{1}\tOper:{2}\tSeq:{3}", sMfgOrderNo, sItemNo, sOperNo, sSeqNo);
                    sErrorMessage += String.Format("\nPost SFO Transaction Error (SFO:{0}\tItem:{1})\t - {2}", sMfgOrderNo, sItemNo, sRtnErr);
                    return iRtn;
                }
                else
                {
                    p_oConnectionInfo.CloseWisysConnection(Wisys.AllSystem.TrxEnums.TransactionAction.Commit, ref sRtnErr);
                    blnDataModificationActionHasBeenTaken = true;
                    return ReturnCodes.SUCCESSFUL;
                }
            }
            catch (Exception ex)
            {
                //Exception handling code
                p_oConnectionInfo.CloseWisysConnection(Wisys.AllSystem.TrxEnums.TransactionAction.Rollback, ref sRtnErr);
                m_dtFailed.Rows.Add(CleanOrderLeftPaddedZeros(sOrderNo));
                sFailureMessage += String.Format("\nTransaction Failed - SFO:{0}\tItem:{1}\tOper:{2}\tSeq:{3}", sMfgOrderNo, sItemNo, sOperNo, sSeqNo);
                sErrorMessage += String.Format("\nPost SFO Transaction Error (SFO:{0}\tItem:{1})\t - {2}", sMfgOrderNo, sItemNo, ex.Message);
                return ReturnCodes.FAILED;
            }
            finally
            {
                oSFC = null;
            }
        }

        #region Connection Class Object
        public class cConnection
        {
            public string sConnString;
            public string sDB;
            public string sMMDB;
            public string sServer;

            public cConnection()
            {
                /*FOR PRODUCTION */
                if (ConfigurationManager.AppSettings["IsTest"].ToString() == "false")
                {
                    sConnString = "DATA02Connection";
                    sDB = "DATA_02";
                    sMMDB = "DATA_02";
                    sServer = "MMMACSQL01";
                }
                else
                {
                    /* FOR DEV TESTING */
                    sConnString = "DATA802TestConnection";
                    sDB = "DATA_802";
                    sMMDB = "DATA_802";
                    sServer = "MMDEVMACSQL01";
                }
            }

            public void setConnString(string newString)
            {
                sConnString = newString;
                if (ConfigurationManager.AppSettings["IsTest"].ToString() == "false")
                    sConnString = "DATA02Connection";
                else
                    sConnString = "DATA802TestConnection";
            }

            public string getConnString()
            {
                return sConnString;
            }
            public string getConnStringValue()
            {

                return ConfigurationManager.ConnectionStrings[sConnString].ConnectionString;

            }

            public void setDB(string newDB)
            {
                sDB = newDB;
            }

            public string getDB()
            {
                return sDB;
            }

            public void setServer(string newServer)
            {
                sServer = newServer;
            }

            public string getServer()
            {
                return sServer;
            }

            public string getMMDB()
            {
                return sMMDB;
            }

            public void setMMDB(string newMMDB)
            {
                sMMDB = newMMDB;
            }
        }
        #endregion

        private void btnManualImport_Click(object sender, EventArgs e)
        {
            ManualImport frm = new ManualImport();
            frm.ShowDialog();
            if (frm.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                blnIsManualImport = true;
                GetOpenOrders();
                BindDGVShipConfirm();
            }
        }

        private void OEPrepForBilling_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Dispose();
        }

        /// <summary>
        /// This gets the current date and formats it as YYYMMDD. It then takes the text area contents (receiving results) and
        /// inserts them into a new file, and saves the file to the network.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveErrMsgLog()
        {
            String fileText = sErrorMessage;
            int year = DateTime.Now.Year;

            int month = DateTime.Now.Month;
            int day = DateTime.Now.Day;

            //if (ConfigurationManager.AppSettings["IsTest"].ToString() == "true")
            //{
            //    year = dtpEnd.Value.Year;
            //    month = dtpEnd.Value.Month;
            //    day = dtpEnd.Value.Day;
            //    //END FOR TESTING
            //}

            String saveFilePath = ConfigurationManager.AppSettings["SaveFilePath"].ToString();
            String saveFileName = "OEPrepForBilling_" + year.ToString() + month.ToString().PadLeft(2, '0') + day.ToString().PadLeft(2, '0');
            if (ConfigurationManager.AppSettings["IsTest"].ToString() == "true")
                saveFileName += "TEST";
            saveFileName += ".txt";
            int x = 1;
            while (System.IO.File.Exists(saveFilePath + saveFileName))
            {
                saveFileName = saveFileName.Substring(0, saveFileName.IndexOf('.') - 1) + "." + x.ToString() + ".txt";
                x = x + 1;
            }
            System.IO.File.WriteAllText(saveFilePath + saveFileName, fileText);
           // MessageBox.Show(saveFilePath + saveFileName + " saved succesfully", "Process Receiving");
        }
    }
}
