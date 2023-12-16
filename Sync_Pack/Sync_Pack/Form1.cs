using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using System.Collections;
using System.Net;
using System.Threading;


namespace Sync_Pack
{
    public partial class Form1 : Form
    {
        private string configPath = System.Windows.Forms.Application.StartupPath + "\\config";
        private Configure config;
        private Hashtable mTable;
        private int mCheckCount;
        private string resultCheck;
        private string mBoxids;
        private EntityException mExp = new EntityException();
        public Form1()
        {
            InitializeComponent();
            mTable = new Hashtable();
            config = null;
        }
        /// <summary>
        /// 初始化化 Item list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            DirectoryInfo TheFolder = new DirectoryInfo(configPath);
            foreach (FileInfo filename in TheFolder.GetFiles())
            {
                cmbItem.Items.Add(filename.Name.Split('.')[0]);
            }
            txxBoxid.Enabled = false;
            btnAdd.Enabled = false;
            btnSync.Enabled = false;
        }
        /// <summary>
        /// 根据所选 Item 配置 config
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                using (StreamReader sr = new StreamReader(configPath + "\\" + cmbItem.Text + ".json"))
                {
                    config = JsonConvert.DeserializeObject<Configure>(sr.ReadToEnd());
                    InitTreeView(config);
                    dgvResult.Rows.Clear();
                    mTable.Clear();
                    txxBoxid.Enabled = true;
                    btnAdd.Enabled = true;
                    btnSync.Enabled = false;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Exception，reading config file.", "Exception——", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }            
        }
        private void InitTreeView(Configure config)
        {
            trvInfo.Nodes["nodeSite"].Nodes["WebSite"].Text = config.WEBSITE;
            trvInfo.Nodes["nodeEquipment"].Nodes["Equipment"].Text = config.EQUIPMENT;
            trvInfo.Nodes["nodeLineno"].Nodes["LineNo"].Text = config.LINENO;
            trvInfo.Nodes["nodeBomno"].Nodes["BomNo"].Text = config.BOMNO;
            trvInfo.Nodes["nodeItemno"].Nodes["ItemNo"].Text = config.ITEMNO;
            trvInfo.Nodes["nodeCustomerno"].Nodes["CustomerNo"].Text = config.CUSTOMERNO;
            trvInfo.Nodes["nodeLabelno"].Nodes["LabelNo"].Text = config.LABELCODE;
            trvInfo.Nodes["nodeOperators"].Nodes["Operators"].Text = config.OPERATORS;
            trvInfo.Nodes["nodeOrderno"].Nodes["OrderNo"].Text = config.ORDERNO;
            trvInfo.ExpandAll();
        }
        private void AddList(string boxid,string msg)
        {
            try
            {
                dgvResult.Rows.Insert(0, new object[] { dgvResult.Rows.Count + 1, boxid, msg });
                btnSync.Enabled = true;
                ++mCheckCount;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txxBoxid_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13 && !string.IsNullOrEmpty(txxBoxid.Text))
            {
                string boxid = txxBoxid.Text;
                txxBoxid.Text = null;
                using (BackgroundWorker bw = new BackgroundWorker())
                {
                    bw.WorkerReportsProgress = true;
                    bw.WorkerSupportsCancellation = true;
                    bw.RunWorkerCompleted += (o, ea) =>
                    {
                        updateDgv(boxid);
                    };
                    bw.DoWork += (o, ea) =>
                    {
                        IsValid(boxid);
                    };
                    bw.RunWorkerAsync();
                }

            }
        }
        private void updateDgv(string id)
        {
            try
            {
                foreach (DataGridViewRow row in dgvResult.Rows)
                {
                    if (row.Cells["mBoxid"].Value.ToString() == id)
                    {
                        throw new Exception();
                    }
                }
                AddList(id, mTable[id] == null ? null : mTable[id].ToString());
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Repeat，the BoxID [ " + id + " ] is exist.", "Exception——", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private void updateDgvs()
        {
            try
            {
                foreach (DictionaryEntry de in mTable)
                {
                    //string k = de.Key.ToString();
                    //string v = de.Value.ToString();
                    if (de.Value == null)
                    {
                        AddList(de.Key.ToString(), null);
                    }
                    else
                    {
                        AddList(de.Key.ToString(), de.Value.ToString());
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            using (BackgroundWorker bw = new BackgroundWorker())
            {

                bw.WorkerReportsProgress = true;
                bw.WorkerSupportsCancellation = true;
                bw.RunWorkerCompleted += (o, ea) =>
                {
                    // 完成后的回调
                    updateDgvs();
                };
                bw.DoWork += (o, ea) =>
                {
                    Check();
                };
                bw.RunWorkerAsync();
            }
        }
        private void Check()
        {
                if (!string.IsNullOrEmpty(openFileDialog1.FileName.Trim()))
                {
                    mCheckCount = 0;
                    resultCheck = null;
                    foreach (string id in GetBoxFile(openFileDialog1.FileName))
                    {
                        IsValid(id);
                    }
                }
        }
        private void showResultCheck(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show(string.Format(resultCheck, mCheckCount), "Info——", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private List<string> GetBoxFile(string filename)
        {
            try
            {
                FileStream fsRead;
                StreamReader streamReader;
                List<string> lines = new List<string>();
                using (fsRead = new FileStream(filename, FileMode.Open))
                {
                    streamReader = new StreamReader(fsRead);
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        lines.Add(line);
                    }
                    streamReader.Close();
                    fsRead.Close();
                }
                return lines;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private void btnSync_Click(object sender, EventArgs e)
        {
            contextMenuStrip1.Enabled = false;
            btnAdd.Enabled = false;
            btnSync.Enabled = false;
            using (BackgroundWorker bw = new BackgroundWorker())
            {
                bw.WorkerReportsProgress = true;
                bw.WorkerSupportsCancellation = true;
                bw.RunWorkerCompleted += (o, ea) =>
                {
                    UpdateGridViewT();
                };
                bw.DoWork += (o, ea) =>
                {
                    Sync_Function();
                };
                bw.RunWorkerAsync();
            }
        }
        private void Sync_Function()
        {
            foreach (DataGridViewRow row in dgvResult.Rows)
            {
                if (row.Cells["mStatus"].Value == null)
                {
                    WebReference.CellToSapPack web = new WebReference.CellToSapPack();
                    string result = web.SendCellDataOfLsmesToSapPack(JsonConvert.SerializeObject(config));
                    //string result = "ok";
                    //Thread.Sleep(3000);
                    mTable[row.Cells["mBoxid"].Value] = result;
                }
            }
        }

        private void IsValid(string boxid)
        {
            try
            {
                config.BOXID = boxid;
                WebReference.CellToSapPack web = new WebReference.CellToSapPack();
                mExp = JsonConvert.DeserializeObject<EntityException>(web.IsException(JsonConvert.SerializeObject(config)));
                mTable.Add(config.BOXID, mExp.ExpMessage);
            }
            catch(WebException ex)
            {
                MessageBox.Show("网络异常，请重试！", "Exception——", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Repeat，the BoxID [ " + boxid + " ] is exist.", "Exception——", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                mTable.Remove(boxid);
            }
        }
        private void UpdateGridViewT()
        {
            foreach (DataGridViewRow row in dgvResult.Rows)
            {
                row.Cells["mStatus"].Value = mTable[row.Cells["mBoxid"].Value];
            }
            dgvResult.ClearSelection();
            btnAdd.Enabled = true;
            btnSync.Enabled = true;
            contextMenuStrip1.Enabled = true;
            
        }

        private void dgvResult_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                (sender as DataGridView).CurrentRow.Selected = false;
                (sender as DataGridView).Rows[e.RowIndex].Selected = true;
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (dgvResult.SelectedRows[0].Cells["mState"].Value == null)
            {
                return;
            }
            else if (dgvResult.SelectedRows[0].Cells["mState"].Value.ToString() != "ErrorQty")
            {
                return;
            }
            else
            {
                WebReference.CellToSapPack web = new WebReference.CellToSapPack();
                string result = web.SendCellDataOfLsmesToSapPack(JsonConvert.SerializeObject(config));
                btnAdd.Enabled = false;
                btnSync.Enabled = false;
                //string result = "ok";
                //Thread.Sleep(3000);
                mTable[dgvResult.SelectedRows[0].Cells["mBoxid"].Value] = result;
                UpdateGridViewT();
                btnAdd.Enabled = true;
                btnSync.Enabled = true;
            }
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }
    }
}
