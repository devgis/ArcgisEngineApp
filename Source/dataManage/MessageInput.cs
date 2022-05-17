using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Data.OleDb;
using System.Data.SqlClient;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Output;

namespace dataManage
{
    public partial class MessageInput : Form
    {
        public MessageInput()
        {
            InitializeComponent();
        }

        private void ZDBHlabel_Click(object sender, EventArgs e)
        {

        }

        private void Scalelabel_Click(object sender, EventArgs e)
        {

        }

        private void VerifyDatelabel_Click(object sender, EventArgs e)
        {

        }

        private void zdbhtextBox_TextChanged(object sender, EventArgs e)
        {
            string zdbh= zdbhtextBox.Text;
        }

        private void djthtextBox_TextChanged(object sender, EventArgs e)
        {
            string djth = djthtextBox.Text;
        }

        private void RightHoldertextBox_TextChanged(object sender, EventArgs e)
        {
            string RightHolder = RightHoldertextBox.Text;
        }

        private void ScaletextBox_TextChanged(object sender, EventArgs e)
        {
            string Scale = ScaletextBox.Text;
        }

        private void DraftsmentextBox_TextChanged(object sender, EventArgs e)
        {
            string Draftsmen = DraftsmentextBox.Text;
        }

        private void AuditortextBox_TextChanged(object sender, EventArgs e)
        {
            string Auditor = AuditortextBox.Text;
        }

        private void DrawDatetextBox_TextChanged(object sender, EventArgs e)
        {
            string DrawDate = DrawDatetextBox.Text;
        }

        private void VerifyDatetextBox_TextChanged(object sender, EventArgs e)
        {
            string VerifyDate = VerifyDatetextBox.Text;
        }

        private void Finishbutton_Click(object sender, EventArgs e)
        {
            PrintActiveview myForm1 = new PrintActiveview();
            myForm1.ShowDialog();
        }
    }
}
