using Google.Cloud.Firestore;
using LostarkLogProject.AbilityStoneLog;
using LostarkLogProject.ControllFuncion;
using LostarkLogProject.Properties;
using LostarkLogProject.TripodLog;
using Microsoft.Win32;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace LostarkLogProject
{
    public partial class MainForm : Form
    {
        bool TestMode = false;
        public MainForm()
        {
            InitializeComponent();
            Init();
            StartLogger();
            webView21.Source = new Uri("https://lostarklogproject.web.app");
        }
        
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect
                                                      , int nTopRect
                                                      , int nRightRect
                                                      , int nBottomRect
                                                      , int nWidthEllipse
                                                      , int nHeightEllipse);

        private void MainForm_Load(object sender, EventArgs e)
        {
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, this.Width, this.Height, 20, 20));
        }

        DashBoardPage dashboard;
        ResourceLoader resourceLoader;
        DetailPage detailPage;
        TripodDashBoard tripodDashBoard;
        ProcessDetector processDetector;

        private void Init()
        {
            LoadOption();
            AutoTrayRun();

            resourceLoader = new ResourceLoader();
            PictureBox[] itemImages = new PictureBox[] { ItemImage1, ItemImage2, ItemImage3, ItemImage4, ItemImage5, ItemImage6, ItemImage7 }; ;
            Label[] imageNames = new Label[] { ImageName1, ImageName2, ImageName3, ImageName4, ImageName5, ImageName6, ImageName7 }; ;
            Label[] successText = new Label[] { SuccessText1, SuccessText2, SuccessText3, SuccessText4, SuccessText5, SuccessText6, SuccessText7 }; ;
            PictureBox[] dashboardEnhanceGraph = { EnhanceGraph25, EnhanceGraph35, EnhanceGraph45, EnhanceGraph55, EnhanceGraph65, EnhanceGraph75 };
            PictureBox[] dashboardReductionGraph = { ReductionGraph25, ReductionGraph35, ReductionGraph45, ReductionGraph55, ReductionGraph65, ReductionGraph75 };
            dashboard = new DashBoardPage(this, resourceLoader, dashboardEnhanceGraph, dashboardReductionGraph, TryLabel, SuccessLabel, FailLabel, CoinLabel, itemImages, imageNames, successText);

            Label[] successPercentage = { Detail25, Detail35, Detail45, Detail55, Detail65, Detail75 };
            detailPage = new DetailPage(this, StartDateTimePicker, EndDateTimePicker, successPercentage, DetailGraphPictureBox);

            Label[] tripodSuccessPercentageLabel = { Tripod5PercentLabel, Tripod10upPercentLabel, Tripod15PercentLabel, Tripod30upPercentLabel, Tripod30PercentLabel , Tripod60upPercentLabel, Tripod100PercentLabel };
            Label[] tripodCountLabel = { Tripod5CountLabel, Tripod10upCountLabel, Tripod15CountLabel, Tripod30upCountLabel, Tripod30CountLabel, Tripod60upCountLabel, Tripod100CountLabel };

            tripodDashBoard = new TripodDashBoard(this, tripodSuccessPercentageLabel, tripodCountLabel);

            string path = AppDomain.CurrentDomain.BaseDirectory + @"lostarklogproject-18d6693e2647.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);

        }

        
        private void VersionChecker()
        {
            //����üũ
            WebRequest request = WebRequest.Create("https://lostarklogproject.web.app/VersionCheck.html");
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);

            string value = reader.ReadToEnd();

            var systemVer = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            if (!value.Contains(systemVer.ToString()))
            {
                MessageBox.Show("������Ʈ�� �ֽ��ϴ�. �ֽŹ����� �̿����ּ���!");
                System.Diagnostics.Process.Start(new ProcessStartInfo("https://github.com/Heinul/LostarkLogProject/releases") { UseShellExecute = true });
                processDetector.Stop();

            }
        }

        private void StartLogger()
        {
            // ���� üũ
            new Thread(VersionChecker).Start();
            //�ػ� Ȯ��
            if (Screen.PrimaryScreen.Bounds.Height != 1080 && Screen.PrimaryScreen.Bounds.Height != 1440 && Screen.PrimaryScreen.Bounds.Height != 2160 && Screen.PrimaryScreen.Bounds.Height != 2880)
            {
                MessageBox.Show($"����� �ػ��� ���α��� 1080, 1440, 2160, 2880�� �ػ󵵸� �����մϴ�.\n�ش� ������� �ػ󵵴� ���� {Screen.PrimaryScreen.Bounds.Height} �Դϴ�.");
                ImageAnalysisState1.Text = "Error : ��ġ���� �ʴ� �ػ��Դϴ�!";
            }
            else
            {
                try
                {
                    if (!TestModeCheck())
                    {
                        processDetector = new ProcessDetector(this, resourceLoader, this.webView21);
                        processDetector.Run();
                    }

                    if (TestModeCheck())
                    {
                        //���μ��� Ž�� ���� �ǳʶٰ� �ٷ� �̹��� ��ġ ����
                        ProcessDetector processDetector = new ProcessDetector(this, resourceLoader, this.webView21);
                        processDetector.TestRun();
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        #region �̺�Ʈ ���� �޼���

        public void AddItemToListBox(string engravingNAme, int percentage, bool success)
        {
            dashboard.AddItemToListBox(engravingNAme, percentage, success);
        }
        #endregion

        #region Form �̺�Ʈ �޼���
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        // �� �̵�
        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }

        private void Home_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex != 0)
            {
                dashboard.SetPageState(false);
                detailPage.SetPageState(false);

                tabControl1.SelectedIndex = 0;
            }
        }

        private void AbilityStoneDashBoard_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex != 1)
            {
                dashboard.SetPageState(true);
                dashboard.UpdateDashboard();

                detailPage.SetPageState(false);
                tripodDashBoard.SetPageState(false);


                tabControl1.SelectedIndex = 1;

                
            }
        }

        private void AbilityStonePage2_Click(object sender, EventArgs e)
        {
            detailPage.SetPageState(true);
            detailPage.UpdateDetailPage();

            tripodDashBoard.SetPageState(false);
            dashboard.SetPageState(false);

            tabControl1.SelectedIndex = 2;
        }

        private void TripodDashBorad_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex != 3)
            {
                tripodDashBoard.SetPageState(true);
                tripodDashBoard.UpdateDetailPage();

                dashboard.SetPageState(false);
                detailPage.SetPageState(false);

                tabControl1.SelectedIndex = 3;
            }
        }

        private void Option_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex != 4)
            {
                dashboard.SetPageState(false);
                detailPage.SetPageState(false);

                tabControl1.SelectedIndex = 4;
            }
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            
            if (PowerOptionTray.Checked)
            {
                this.WindowState = FormWindowState.Minimized;
                this.ShowInTaskbar = false;
                TrayIcon.Visible = true;
                TrayIcon.ContextMenuStrip = TrayMenu;
            }
            else
            {
                TrayIcon.Dispose();
                Process.GetCurrentProcess().Kill();
                Application.Exit();
            }

        }

        private void GraphMouseMove(object sender, EventArgs e)
        {
            dashboard.GraphMouseMove(sender, e, DashboardGraphToolTip);
        }

        private void WindowsAutoStartCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.�������ڵ����� = WindowsAutoStartCheckBox.Checked;
            Properties.Settings.Default.Save();

            string regPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            string programName = "LostArkLog";
            try
            {
                using (var regKey = GetRegKey(regPath, true))
                {
                    if (WindowsAutoStartCheckBox.Checked)
                    {
                        if (regKey.GetValue(programName) == null)
                        {
                            regKey.SetValue(programName, AppDomain.CurrentDomain.BaseDirectory + "\\" + AppDomain.CurrentDomain.FriendlyName);
                        }
                    }
                    else
                    {
                        if(regKey.GetValue(programName) != null)
                        {
                            regKey.DeleteValue(programName, false);
                        }
                    }

                    regKey.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private Microsoft.Win32.RegistryKey GetRegKey(string regPath, bool writable)
        {
            return Registry.CurrentUser.OpenSubKey(regPath, writable);
        }

        private void PowerOption_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.�����ư�ɼ� = PowerOptionTray.Checked == true ? true : false;
            Properties.Settings.Default.Save();
        }

        private void TrayStart_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Ʈ���̷ν��� = TrayStartCheckBox.Checked;
            Properties.Settings.Default.Save();
        }
        private void LoadOption()
        {
            if(Properties.Settings.Default.�����ư�ɼ� == true)
            {
                PowerOptionTray.Checked = true;
            }
            else
            {
                PowerOptionOff.Checked = true;
            }

            if (!TestModeCheck())
            {
                if (Settings.Default.UID == "")
                {
                    Settings.Default.UID = Guid.NewGuid().ToString();
                    Settings.Default.Save();
                    UID_Label.Text = Settings.Default.UID;
                }
                else
                {
                    UID_Label.Text = Settings.Default.UID;
                }
            }
            else
            {
                Settings.Default.UID = "";
                Settings.Default.Save();
                UID_Label.Text = "TestMode ������";
            }

            WindowsAutoStartCheckBox.Checked = Properties.Settings.Default.�������ڵ�����;
            TrayStartCheckBox.Checked = Properties.Settings.Default.Ʈ���̷ν���;
        }

        public void SetStateImage(int stateNum)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(delegate ()
                {
                    switch (stateNum)
                    {
                        case 0:
                            // �ν�Ʈ��ũ ���� �����
                            LLStateImage.Image = Properties.Resources.�ν�Ʈ��ũ_����_�����;
                            break;
                        case 1:
                            // ���� �����
                            LLStateImage.Image = Properties.Resources.��ȭ�۾�_�����; ;
                            break;
                        case 2:
                            // �����Ƽ���� ���� �ν�
                            LLStateImage.Image = Properties.Resources.�����Ƽ����_����_�����;
                            break;
                        case 3:
                            // Ʈ�������� �ο� �ν�
                            LLStateImage.Image = Properties.Resources.Ʈ��������_�ο�_�����;
                            break;
                        case 4:
                            // ����
                            LLStateImage.Image = Properties.Resources.����;
                            break;
                    }
                }));
            }
        }

        private void UID_Label_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(UID_Label.Text);
        }

        #endregion

        #region Ʈ���̾�����

        private void AutoTrayRun()
        {
            TrayIcon.Visible = true;
            TrayIcon.ContextMenuStrip = TrayMenu;
            if (TrayStartCheckBox.Checked)
            {
                this.WindowState = FormWindowState.Minimized;
                this.ShowInTaskbar = false;
                this.Visible = false;
            }
        }

        private void LoadTrayIcon(object? sender, EventArgs e)
        {
            TrayIcon.Visible = true;
            TrayIcon.ContextMenuStrip = TrayMenu;
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TrayIcon.Dispose();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            Application.Exit();
        }

        private void ShowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowInTaskbar = true;
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;
        }

        private void TrayClick(object sender, EventArgs e)
        {
            this.WindowState |= FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Visible = false;
            TrayIcon.Visible = true;
            TrayIcon.ContextMenuStrip = TrayMenu;
        }

        private void TrayIconMouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.ShowInTaskbar = true;
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;
        }

        #endregion

        public bool TestModeCheck()
        {
            return TestMode;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 5;
        }
    }
}