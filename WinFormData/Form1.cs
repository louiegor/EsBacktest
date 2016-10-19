using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Autofac;
using WinFormData.Tests;
using IContainer = Autofac.IContainer;

namespace WinFormData
{
    public interface IFormWithTxt
    {
        void SetText(string text);
    }

    public partial class Form1 : Form, IFormWithTxt
    {
        private readonly IContainer container;
        private readonly IFileWatcher fw;
        private readonly IXmlHelper api;
        private readonly IMainModel model;
        private readonly ILifetimeScope scope;
       

        public static Form1 Form1Edit;

        public Form1()
        {
            //Init Autofac
            var builder = new ContainerBuilder();

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                   .AsImplementedInterfaces();

            //Force IXmlFeed to use mock
            
            //UseFakeXml(builder);
            UseRealXml(builder);

            container = builder.Build();
            scope = container.BeginLifetimeScope();
            InitializeComponent();
            Form1Edit = this;

            fw = scope.Resolve<IFileWatcher>();
            api = scope.Resolve<IXmlHelper>();
            model = scope.Resolve<IMainModel>();
            InstructionTextBox.Text =
                    @"Instruction:  The output csv file should be in the following format (Symbol,Profit,Win Ratio,Current Price)";
        }

        public void UseRealXml(ContainerBuilder builder)
        {
            builder.RegisterType<XmlFeed>().As<IXmlFeed>();
            Global.IsFakePoco = false;
        }

        public void UseFakeXml(ContainerBuilder builder)
        {
            builder.RegisterType<FakeXmlFeed>().As<IXmlFeed>();
            Global.IsFakePoco = true;
        }

        delegate void SetTextCallback(string text);

        public void SetText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (statusTextBox.InvokeRequired)
            {
                var d = new SetTextCallback(SetText);
                Invoke(d, new object[] { text });
            }
            else
            {
                var timeStamp = DateTime.Now.ToString("h:mm:ss tt");
                statusTextBox.Text = String.Format(statusTextBox.Text + "\r\n" +  timeStamp + " " + text);
                statusTextBox.SelectionStart = statusTextBox.Text.Length;
                statusTextBox.ScrollToCaret();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            eSignalOutputPath.Text = fw.GetEsignalPath();
            stopButton.Enabled = false;
            GetConnection();
            
            if (Global.IsFakePoco)SetText("Mock test. This is fake poco");
            var sh = new SettingHelper();
            var setting = sh.GetSetting();
            Form1Edit.traderId.Text = setting.UserId;


            //not implemented yet
            
        }

        public void UpdateGlobalParameters(){
            var sh = new SettingHelper();
            SetText("Registering TraderId, PProPath and Esignal Path");
            Global.EsignalPath = eSignalOutputPath.Text;
            Global.PproPath = pproPath.Text;
            Global.TraderId = traderId.Text;
            Global.ShareSize = int.Parse(univShareSize.Text);
            Global.NumOfRetries = Convert.ToInt32(numOfRetries.Value);
            Global.AmExceptionList = sh.GetAmList();
            Global.NqExceptionList = sh.GetNqList();

        }
        
        public static DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
               TypeDescriptor.GetProperties(typeof(T));
            var table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;

        }

        private void Timer1_Tick(object sender, EventArgs e) //ticks every 10 sec
        {
            //Don't start if start button is not clicked
            if (Global.TraderId == null) return;

            //This is the correct way of doing it
            //var result = new BindingList<TestResult>();
            //Lv1PositionGrid.DataSource = result;

            if (IsFilter.Checked)
            {
                double  winratio ;
                double.TryParse(WinRatioFilter.Text, out winratio);
                double profitpercent;
                double.TryParse(ProfitFilter.Text, out profitpercent);

                if (winratio>-0.1 && winratio <0.1) //winratio is 0
                {
                    winratio = -100;
                }
                if (profitpercent > -0.1 && profitpercent < 0.1) //profitpercent is 0
                {
                    profitpercent = -100;
                }
                
                totalTextBox.Text = CsvParser.GetTotal().ToString(CultureInfo.InvariantCulture);
                numStock.Text = CsvParser.GetDictCount().ToString(CultureInfo.InvariantCulture);
                Lv1PositionGrid.DataSource = CsvParser.GetFilterDictionary(winratio, profitpercent);
                discardGrid.DataSource = CsvParser.GetUnFilterDictionary(winratio, profitpercent);
                FilteredProfit.Text = CsvParser.GetFilterProfit(winratio,profitpercent).ToString(CultureInfo.InvariantCulture);
                FilterNumStock.Text = CsvParser.GetFilterTotalStocks(winratio, profitpercent).ToString(CultureInfo.InvariantCulture);
                InstructionTextBox.Text =
                    @"Instruction:  The output csv file should be in the following format (Symbol,Profit,Win Ratio,Current Price)";
            }
            else
            {
                Lv1PositionGrid.DataSource = CsvParser.GetDictionaryValues();
                totalTextBox.Text = CsvParser.GetTotal().ToString(CultureInfo.InvariantCulture);
                numStock.Text = CsvParser.GetDictCount().ToString(CultureInfo.InvariantCulture);
            }
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            DeleteAllFileInEsginalPath();
            SetText("Starting Esignal Backtest Watcher");
            Global.StopEntryTime = 90000; // Entry stop time
            Global.StopCheckInterval = 5000; //Entry stop check price interval

            //Save Setting if it's not TMS
            if (!Form1Edit.isTms.Checked)
            {
                var setting = new Setting
                    {
                        UserId = Form1Edit.traderId.Text,

                    };
                var sh = new SettingHelper();
                sh.SaveSetting(setting);
            }

            UpdateGlobalParameters();

            //DeleteAllFileInEsginalPath();
            fw.Watch();
            startButton.Enabled = false;
            stopButton.Enabled = true;


        }

        public void DeleteAllFileInEsginalPath()
        {
            var di = new DirectoryInfo(fw.GetEsignalPath());

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
            
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            fw.StopWatch();
            CsvParser.ClearDictionary();
            //DeleteAllFileInEsginalPath();
            startButton.Enabled = true;
            stopButton.Enabled = false;
        }

        private void GetConnection()
        {
            var setting = new HttpWebDefaultSetting {ChromeId = Global.TraderId};
            var connection = Connection.SiteConnection(setting);
            if (!connection)
            {
                startButton.Visible = false;
                FormHelper.FormSetText("Cannot connect to source");
            }
        }

        private void PassiveCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (PassiveCheckBox.Checked)
            {
                numOfRetries.Enabled = true;
                numOfRetries.Value = 1;
            }
            else if (PassiveCheckBox.Checked == false)
            {
                numOfRetries.Enabled = false;
                numOfRetries.Value = 0;
            }
        }

        private void isTms_CheckedChanged(object sender, EventArgs e)
        {

            if (isTms.Checked)
            {
                Form1Edit.traderId.Enabled = false;
                Form1Edit.traderId.Text = "SCOTHOLL";
            }

            if (!isTms.Checked)
            {
                Form1Edit.traderId.Enabled = true;
                var sh = new SettingHelper();
                var setting =sh.GetSetting();
                Form1Edit.traderId.Text = setting.UserId;
            }
        }



        private void Documentation_Click(object sender, EventArgs e)
        {
            var frm2 = GetDocumentation;
            frm2.Show();
            

        }

        private Form2documentation form2;

        public Form2documentation GetDocumentation
        {
            get
            {
                if (form2 == null || form2.IsDisposed)
                {
                    form2 = new Form2documentation();
                }
                return form2;
            }
        }
    }

    
}
