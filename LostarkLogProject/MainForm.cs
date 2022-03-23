using Google.Cloud.Firestore;
using LostarkLogProject.AbilityStoneLog;
using LostarkLogProject.ControllFuncion;
using Microsoft.Win32;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LostarkLogProject
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
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
            Init();
            StartLogger();
        }

        DashBoardPage dashboard;
        ImageAnalysis imageAnalysis;
        ResourceLoader resourceLoader;
        DetailPage detailPage;
        FirestoreDb firestoreDb;

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

            string path = AppDomain.CurrentDomain.BaseDirectory + @"asl-project-80aca-7ea4b7df82f1.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
            firestoreDb = FirestoreDb.Create("asl-project-80aca");
        }


        private async void StartLogger()
        {

            // 자동시작

            // 버전 확인
            //var version = firestoreDb.Collection("Version").Document("VersionCheck");
            //var snap = await version.GetSnapshotAsync();

            //if (snap.Exists)
            //{
            //    var systemVer = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            //    var firebaseVersion = snap.ToDictionary();
            //    var value = firebaseVersion["Latest"].ToString();

            //    if (!value.Equals(systemVer.ToString()))
            //    {
            //        MessageBox.Show("업데이트가 있습니다. 최신버전을 이용해주세요!");
            //        System.Diagnostics.Process.Start(new ProcessStartInfo("https://github.com/Heinul/AbilityStoneLogProject/releases") { UseShellExecute = true });
            //        return;
            //    }
            //}

            //해상도 확인
            if (Screen.PrimaryScreen.Bounds.Height != 1080 && Screen.PrimaryScreen.Bounds.Height != 1440 && Screen.PrimaryScreen.Bounds.Height != 2160 && Screen.PrimaryScreen.Bounds.Height != 2880)
            {
                MessageBox.Show($"모니터 해상도의 세로기준 1080, 1440, 2160, 2880의 해상도만 지원합니다.\n해당 모니터의 해상도는 세로 {Screen.PrimaryScreen.Bounds.Height} 입니다.");
                ImageAnalysisState1.Text = "Error : 일치하지 않는 해상도입니다!";
            }
            else
            {
                try
                {
                    imageAnalysis = new ImageAnalysis(this, resourceLoader, firestoreDb);
                    ProcessDetector processDetector = new ProcessDetector(this);
                    processDetector.Run();

                    //Test
                    //imageAnalysis = new ImageAnalysis(this, resourceLoader, firestoreDb);
                    //imageAnalysis.Run();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        #region 이벤트 전달 메서드

        public void StartImageAnalysis()
        {
            imageAnalysis.Run();
        }

        public void StopImageAnalysis()
        {
            imageAnalysis.Stop();
        }

        public void AddItemToListBox(string engravingNAme, int percentage, bool success)
        {
            dashboard.AddItemToListBox(engravingNAme, percentage, success);
        }
        #endregion

        #region Form 이벤트 메서드
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        // 폼 이동
        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }

        private void Home_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex != 0)
            {
                dashboard.SetPageState(true);
                dashboard.UpdateDashboard();

                detailPage.SetPageState(false);


                tabControl1.SelectedIndex = 0;
            }
        }

        private void LogDetail_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex != 2)
            {
                detailPage.SetPageState(true);
                detailPage.UpdateDetailPage();

                dashboard.SetPageState(false);

                tabControl1.SelectedIndex = 2;
            }
        }

        private void Tendency_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex != 3)
            {
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

        public void SetImageAnalysisStateText(string str)
        {
            this.Invoke(new Action(delegate ()
            {
                ImageAnalysisState1.Text = str;
                ImageAnalysisState2.Text = str;
            }));
        }

        private void WindowsAutoStartCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.윈도우자동시작 = WindowsAutoStartCheckBox.Checked;
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
            Properties.Settings.Default.종료버튼옵션 = PowerOptionTray.Checked == true ? true : false;
            Properties.Settings.Default.Save();
        }

        private void TrayStart_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.트레이로실행 = TrayStartCheckBox.Checked;
            Properties.Settings.Default.Save();
        }
            private void LoadOption()
        {
            if(Properties.Settings.Default.종료버튼옵션 == true)
            {
                PowerOptionTray.Checked = true;
            }
            else
            {
                PowerOptionOff.Checked = true;
            }

            WindowsAutoStartCheckBox.Checked = Properties.Settings.Default.윈도우자동시작;
            TrayStartCheckBox.Checked = Properties.Settings.Default.트레이로실행;
        }
        #endregion

        #region 트레이아이콘

        private void AutoTrayRun()
        {
            if (TrayStartCheckBox.Checked)
            {
                this.WindowState = FormWindowState.Minimized;
                this.ShowInTaskbar = false;
                this.Visible = false;
                TrayIcon.Visible = true;
                TrayIcon.ContextMenuStrip = TrayMenu;
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
        }

        private void TrayIconMouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.ShowInTaskbar = true;
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;
        }

        #endregion

       
    }
}