using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Drawing.Printing;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS;
using ESRI.ArcGIS.OutputExtensions;

namespace dataManage
{
    public class PrintActiveview : System.Windows.Forms.Form
    {
        private System.ComponentModel.Container components = null;
        private System.Windows.Forms.Button btnPageSetup;
        private System.Windows.Forms.Button btnPrint;

        internal PrintPreviewDialog printPreviewDialog1;
        internal PrintDialog printDialog1;
        internal PageSetupDialog pageSetupDialog1;

        private System.Drawing.Printing.PrintDocument document = new System.Drawing.Printing.PrintDocument();
        private ITrackCancel m_TrackCancel = new CancelTrackerClass();
        public ESRI.ArcGIS.Controls.AxPageLayoutControl axPageLayoutControl1;
        private short m_CurrentPrintPage;

        IPrinter printer = new EmfPrinterClass();
        private ESRI.ArcGIS.Controls.AxToolbarControl axToolbarControl1;
        private string strPrinterType = "EmfPrinter";

        public PrintActiveview()
        {
            InitializeComponent();
        }


       // 释放内存
        protected override void Dispose(bool disposing)
        {
          
            ESRI.ArcGIS.ADF.COMSupport.AOUninitialize.Shutdown();

            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrintActiveview));
            this.btnPageSetup = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.axPageLayoutControl1 = new ESRI.ArcGIS.Controls.AxPageLayoutControl();
            this.axToolbarControl1 = new ESRI.ArcGIS.Controls.AxToolbarControl();
            ((System.ComponentModel.ISupportInitialize)(this.axPageLayoutControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axToolbarControl1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnPageSetup
            // 
            this.btnPageSetup.Location = new System.Drawing.Point(383, 10);
            this.btnPageSetup.Name = "btnPageSetup";
            this.btnPageSetup.Size = new System.Drawing.Size(72, 26);
            this.btnPageSetup.TabIndex = 4;
            this.btnPageSetup.Text = "页面设置";
            this.btnPageSetup.Click += new System.EventHandler(this.btnPageSetup_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(474, 10);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(60, 26);
            this.btnPrint.TabIndex = 6;
            this.btnPrint.Text = "打  印";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // axPageLayoutControl1
            // 
            this.axPageLayoutControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.axPageLayoutControl1.Location = new System.Drawing.Point(10, 44);
            this.axPageLayoutControl1.Name = "axPageLayoutControl1";
            this.axPageLayoutControl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axPageLayoutControl1.OcxState")));
            this.axPageLayoutControl1.Size = new System.Drawing.Size(726, 547);
            this.axPageLayoutControl1.TabIndex = 9;
            // 
            // axToolbarControl1
            // 
            this.axToolbarControl1.Location = new System.Drawing.Point(10, 10);
            this.axToolbarControl1.Name = "axToolbarControl1";
            this.axToolbarControl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axToolbarControl1.OcxState")));
            this.axToolbarControl1.Size = new System.Drawing.Size(367, 28);
            this.axToolbarControl1.TabIndex = 12;
            // 
            // PrintActiveview
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(752, 601);
            this.Controls.Add(this.axToolbarControl1);
            this.Controls.Add(this.axPageLayoutControl1);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.btnPageSetup);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PrintActiveview";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "地图打印";
            this.Load += new System.EventHandler(this.PrintActiveView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.axPageLayoutControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axToolbarControl1)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private void PrintActiveView_Load(object sender, EventArgs e)
        {
            InitializePrintPreviewDialog(); //initialize the print preview dialog
            printDialog1 = new PrintDialog(); //create a print dialog object
            InitializePageSetupDialog(); //initialize the page setup dialog         
            axPageLayoutControl1.AutoMouseWheel = true;
            axPageLayoutControl1.Refresh();
        }

        private void InitializePrintPreviewDialog()
        {
            printPreviewDialog1 = new PrintPreviewDialog();
            printPreviewDialog1.Name = "打印预览";
            //set UseAntiAlias to true to allow the operating system to smooth fonts
            printPreviewDialog1.UseAntiAlias = true;

            //printPreviewDialog1.ClientSize = new System.Drawing.Size(800, 600);
            //printPreviewDialog1.Location = new System.Drawing.Point(29, 29);
            //printPreviewDialog1.MinimumSize = new System.Drawing.Size(375, 250);

            //associate the event-handling method with the document's PrintPage event
            this.document.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(document_PrintPage);
        }

        private void InitializePageSetupDialog()
        {
            pageSetupDialog1 = new PageSetupDialog();
            pageSetupDialog1.PageSettings = new System.Drawing.Printing.PageSettings();
            pageSetupDialog1.PrinterSettings = new System.Drawing.Printing.PrinterSettings();
            pageSetupDialog1.ShowNetwork = false;

        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            Stream myStream;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "template files (*.mxt)|*.mxt|mxd files (*.mxd)|*.mxd";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //检测是否选择了一个文件
                if ((myStream = openFileDialog1.OpenFile()) != null)
                {
                    string fileName = openFileDialog1.FileName;
                    //检测选择的文件是否为一mxd 文件
                    if (axPageLayoutControl1.CheckMxFile(fileName))
                    {
                        axPageLayoutControl1.LoadMxFile(fileName, "");
                    }
                    myStream.Close();
                }
            }
        }

        private void btnPageSetup_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = pageSetupDialog1.ShowDialog();
                document.PrinterSettings = pageSetupDialog1.PrinterSettings;
                document.DefaultPageSettings = pageSetupDialog1.PageSettings;

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }

            IEnumerator paperSizes = pageSetupDialog1.PrinterSettings.PaperSizes.GetEnumerator();
            paperSizes.Reset();

            for (int i = 0; i < pageSetupDialog1.PrinterSettings.PaperSizes.Count; ++i)
            {
                paperSizes.MoveNext();
                if (((PaperSize)paperSizes.Current).Kind == document.DefaultPageSettings.PaperSize.Kind)
                {
                    document.DefaultPageSettings.PaperSize = ((PaperSize)paperSizes.Current);
                }
            }
            IPaper paper = new PaperClass();
            paper.Attach(pageSetupDialog1.PrinterSettings.GetHdevmode(pageSetupDialog1.PageSettings).ToInt32(), pageSetupDialog1.PrinterSettings.GetHdevnames().ToInt32());

            printer.Paper = paper;
            axPageLayoutControl1.Printer = printer;
        }

        private void btnPrintPreview_Click(object sender, EventArgs e)
        {
            m_CurrentPrintPage = 0;
            if (axPageLayoutControl1.DocumentFilename == null) return;
            document.DocumentName = axPageLayoutControl1.DocumentFilename;
            try
            {
                printPreviewDialog1.Document = document;
                printPreviewDialog1.ShowDialog();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void btnPrint_Click(object sender, EventArgs e)
        {

            printDialog1.AllowSomePages = true; //允许用户选择打印哪些页面
            printDialog1.ShowHelp = true;
            printDialog1.Document = document;

            try
            {
                DialogResult result = printDialog1.ShowDialog();
                if (result == DialogResult.OK) document.Print();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void document_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            axPageLayoutControl1.Page.PageToPrinterMapping = esriPageToPrinterMapping.esriPageMappingTile;
            short dpi = (short)e.Graphics.DpiX;
            IEnvelope devBounds = new EnvelopeClass();
            IPage page = axPageLayoutControl1.Page;
            short printPageCount = axPageLayoutControl1.get_PrinterPageCount(0);
            m_CurrentPrintPage++;
            IPrinter printer = axPageLayoutControl1.Printer;
            page.GetDeviceBounds(printer, m_CurrentPrintPage, 0, dpi, devBounds);
            tagRECT deviceRect;
            double xmin, ymin, xmax, ymax;
            devBounds.QueryCoords(out xmin, out ymin, out xmax, out ymax);
            deviceRect.bottom = (int)ymax;
            deviceRect.left = (int)xmin;
            deviceRect.top = (int)ymin;
            deviceRect.right = (int)xmax;
            IEnvelope visBounds = new EnvelopeClass();
            page.GetPageBounds(printer, m_CurrentPrintPage, 0, visBounds);
            IntPtr hdc = e.Graphics.GetHdc();
            axPageLayoutControl1.ActiveView.Output(hdc.ToInt32(), dpi, ref deviceRect, visBounds, m_TrackCancel);

            e.Graphics.ReleaseHdc(hdc);
            if (m_CurrentPrintPage < printPageCount)
                e.HasMorePages = true; //document_PrintPage event will be called again
            else
                e.HasMorePages = false;
        }

    }
}
