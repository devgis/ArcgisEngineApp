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
    public partial class Form1 : Form
    {

        IPointCollection m_pPointCol;
        IPoint m_pPoint;  

        public Form1()
        {
            InitializeComponent();
        }

        private void 导入数据IToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Read();
        }

        public void Read()
         {   
             OpenFileDialog openFileDialog = new OpenFileDialog();//creat a OpenFileDialog object and assign it to openFileDialog  用openfiledialog 打开文件
             openFileDialog.Filter = "原始数据 (*.xlsx)|*.xlsx";//过滤属性设置
             if (openFileDialog.ShowDialog() == DialogResult.OK) //判断是否点击了打开文件窗口的确定按钮
             {
                 string fileName = openFileDialog.FileName;//定义变量fileName并存储内容
                 string strConn = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 12.0;HDR=Yes;IMEX=1;'", fileName);//读取Excel表
                 
                 OleDbConnection conn = new OleDbConnection(strConn);//定义变量并初始化 读取表
                 conn.Open();

                 string strExcel = "";
                 OleDbDataAdapter myCommand = null;
                 DataSet ds = null;

                 strExcel = "select * from [sheet1$]";
                 myCommand = new OleDbDataAdapter(strExcel, strConn);
                 ds = new DataSet();
                 myCommand.Fill(ds, "table1"); 

                 
                 IGraphicsContainer pGraphicsContainer = axMapControl1.Map as IGraphicsContainer;
                 
                 for (int i = 1; i < ds.Tables[0].Rows.Count;i++ )
                 {
                     m_pPoint = new PointClass();
                     object missing1 = Type.Missing;
                     object missing2 = Type.Missing;
                     m_pPoint.X = double.Parse(ds.Tables[0].Rows[i][1].ToString());
                     m_pPoint.Y = double.Parse(ds.Tables[0].Rows[i][2].ToString());
                     m_pPointCol.AddPoint(m_pPoint, missing1, missing2);
                 }


                 //绘制几何图形
                 IRubberBand pRubberPolygon = new RubberPolygonClass();
                 ISimpleFillSymbol pFillSymbol = new SimpleFillSymbolClass();

                 pFillSymbol.Color = getRGB(255, 255, 0);
                 IPolygon pPolygon = getGeometry(m_pPointCol) as IPolygon;
                 pFillSymbol.Style = esriSimpleFillStyle.esriSFSDiagonalCross;
                 pFillSymbol.Color = getRGB(0, 255, 255);

                 IFillShapeElement pPolygonEle = new PolygonElementClass();
                 pPolygonEle.Symbol = pFillSymbol;
                 IElement pEle = pPolygonEle as IElement;
                 pEle.Geometry = pPolygon;
                 pGraphicsContainer.AddElement(pEle, 0);

                 List<double> lengths = new List<double>();

                 //IArea接口计算面积  
                 IArea pArea = (IArea)pPolygon;   
                 double s = Math.Abs(Math.Round(pArea.Area, 2));   
                 label2.Text = "图形面积为："+s;  
                 
                 lengths=getLength(pPolygon);
                 label3.Text = "下边长为：" + lengths[0].ToString();
                 label4.Text = "左边长为：" + lengths[1].ToString();
                 label5.Text = "上边长为：" + lengths[2].ToString();
                 label6.Text = "右边长为：" + lengths[3].ToString();

                 this.panel1.Visible = true;//属性设置

                 axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);//刷新图
             }
         }

        //计算边长
        private List<double> getLength(IPolygon pPolygon)
        {
            List<double> lengths = new List<double>();
            
            IPoint LowerLeft = pPolygon.Envelope.LowerLeft;
            IPoint LowerRight = pPolygon.Envelope.LowerRight;
            IPoint UpperLeft = pPolygon.Envelope.UpperLeft;
            IPoint UpperRight = pPolygon.Envelope.UpperRight;

            lengths.Add(computedLength(LowerLeft, LowerRight));
            lengths.Add(computedLength(LowerLeft, UpperLeft));
            lengths.Add(computedLength(UpperRight, UpperLeft));
            lengths.Add(computedLength(UpperRight, LowerRight));

            return lengths;
        }


        //勾股定理
        private double computedLength(IPoint start,IPoint stop) 
        {
            double length = Math.Sqrt(Math.Abs(stop.X - start.X) * Math.Abs(stop.X - start.X) + Math.Abs(stop.Y - start.Y) * Math.Abs(stop.Y - start.Y));
            return length;
        }


        private IGeometry getGeometry(IPointCollection Points)
        {
            //通过坐标点成面 传入几何点 返回几何图形
            IPointCollection iPointCollection = new PolygonClass();
            Ring ring = new RingClass();
            object missing = Type.Missing;
            ring.AddPointCollection(Points);
            IGeometryCollection pointPolygon = new PolygonClass();
            pointPolygon.AddGeometry(ring as IGeometry, ref missing, ref missing);
            IPolygon polyGonGeo = pointPolygon as IPolygon;
            polyGonGeo.SimplifyPreserveFromTo();
            return polyGonGeo as IGeometry;
        }

        private IRgbColor getRGB(int r, int g, int b)
        {

            //设置线框
            IRgbColor pColor;
            pColor = new RgbColorClass();
            pColor.Red = r;
            pColor.Green = g;
            pColor.Blue = b;
            return pColor;
        }

        private void 退出TToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            m_pPointCol = new PolygonClass();
        }
        PrintActiveview printActiveView = new PrintActiveview();
        private void 打印地图ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            MessageInput myForm = new MessageInput();
            myForm.ShowDialog();
        }

        private void axMapControl1_OnAfterScreenDraw(object sender, ESRI.ArcGIS.Controls.IMapControlEvents2_OnAfterScreenDrawEvent e)
        {
            //获取拷贝接口对象
            IObjectCopy objectCopy = new ObjectCopyClass();//实例化并赋值
            object toCopyMap = axMapControl1.Map;
            object copiedMap = objectCopy.Copy(toCopyMap);
            object toOverwriteMap = axPageLayoutControl1.ActiveView.FocusMap;//获取焦点地图
            objectCopy.Overwrite(copiedMap, ref toOverwriteMap);//复制
            axPageLayoutControl1.ActiveView.Refresh();//刷新

            object toMap = printActiveView.axPageLayoutControl1.ActiveView.FocusMap;
            objectCopy.Overwrite(copiedMap, ref toMap);
            axPageLayoutControl1.ActiveView.Refresh();
        }

        private void 导出图片ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog pSaveDialog = new SaveFileDialog();
                pSaveDialog.FileName = "";//设置文件名
                pSaveDialog.Filter = "JPG图片(*.JPG)|*.jpg|tif图片(*.tif)|*.tif|PDF文档(*.PDF)|*.pdf";//类型
                if (pSaveDialog.ShowDialog() == DialogResult.OK)//判断是否点了确定按钮
                {

                    //  获取布局视图屏幕分辨率，用于设置输出分辨率                   
                    double iScreenDispalyResolution = this.axPageLayoutControl1.ActiveView.ScreenDisplay.DisplayTransformation.Resolution;

                    // 根据用户选择保存的文件类型，来创建不同的输出类
                    IExporter pExporter = null;


                    if (pSaveDialog.FilterIndex == 0)
                    {
                        pExporter = new JpegExporterClass();
                    }
                    else if (pSaveDialog.FilterIndex == 1)
                    {
                        pExporter = new TiffExporterClass();
                    }
                    else if (pSaveDialog.FilterIndex == 2)
                    {
                        pExporter = new PDFExporterClass();
                    }


                    //设置输出文件名
                    pExporter.ExportFileName = pSaveDialog.FileName;

                    //设置输出分辨率
                    pExporter.Resolution = (short)iScreenDispalyResolution;

                    tagRECT deviceRect;
                    deviceRect.left = 0;
                    deviceRect.top = 0;
                    deviceRect.right = this.axPageLayoutControl1.ActiveView.ExportFrame.right * (300 / 96);
                    deviceRect.bottom = this.axPageLayoutControl1.ActiveView.ExportFrame.bottom * (300 / 96);


                    IEnvelope pDeviceEnvelope = new EnvelopeClass();

                    //设置边框范围
                    pDeviceEnvelope.PutCoords(deviceRect.left, deviceRect.bottom, deviceRect.right, deviceRect.top);

                    // 将打印像素范围 设置给输出对象
                    pExporter.PixelBounds = pDeviceEnvelope;

                    //设置跟踪取消对象
                    ITrackCancel pCancle = new CancelTrackerClass();

                    //输出操作
                    this.axPageLayoutControl1.ActiveView.Output(pExporter.StartExporting(), pExporter.Resolution, ref deviceRect, this.axPageLayoutControl1.ActiveView.Extent, pCancle);
                   
                    //启动程序
                    Application.DoEvents();

                    //完成输出
                    pExporter.FinishExporting();
                    MessageBox.Show("输入成功，请查看。", "提示");
                }
            }

            catch (Exception Err)
            {
                MessageBox.Show(Err.Message, "输出图片", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.panel1.Visible = false;
        }


    }
}