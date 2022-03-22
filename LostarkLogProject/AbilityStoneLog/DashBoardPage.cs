using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostarkLogProject.AbilityStoneLog
{
    internal class DashBoardPage
    {
        private bool pageState = true;

        private MainForm mainForm;
        private PictureBox[] enhanceGraph, reductionGraph;
        private Label TryLabel, SuccessLabel, FailLabel, CoinLabel;
        private ResourceLoader resourceLoader;

        private string[] tooltipEText = new string[6];
        private string[] tooltipRText = new string[6];

        private PictureBox[] itemImages;
        private Label[] imageNames;
        private Label[] successText;
        private int itemCount = 0;

        public DashBoardPage(MainForm mainForm, ResourceLoader resourceLoader, PictureBox[] enhanceGraph, PictureBox[] reductionGraph, Label TryLabel, Label SuccessLabel, Label FailLabel, Label CoinLabel, PictureBox[] itemImages, Label[] imageNames, Label[] successText)
        {
            this.mainForm = mainForm;
            this.enhanceGraph = enhanceGraph;
            this.reductionGraph = reductionGraph;
            this.TryLabel = TryLabel;
            this.SuccessLabel = SuccessLabel;
            this.FailLabel = FailLabel;
            this.CoinLabel = CoinLabel;
            this.resourceLoader = resourceLoader;
            this.itemImages = itemImages;
            this.imageNames = imageNames;
            this.successText = successText;

            UpdateDashboard();
        }

        public void SetPageState(bool state)
        {
            pageState = state;
        }

        public void UpdateDashboard()
        {
            new Thread(() => {
                AbilityStoneDBManager db = new AbilityStoneDBManager();
                while (pageState)
                {
                    try
                    {
                        var tryCount = db.SelectAll().Count;
                        var successCount = db.Select(true).Count;
                        mainForm.Invoke(new Action(delegate ()
                        {
                            TryLabel.Text = (tryCount > 1000) ? String.Format("{0:#,0.##}", ((double)tryCount / 1000)) + "K" : String.Format("{0:#,0}", tryCount);
                            SuccessLabel.Text = (successCount > 1000) ? String.Format("{0:#,0.##}", ((double)successCount / 1000)) + "K" : String.Format("{0:#,0}", successCount);
                            FailLabel.Text = (tryCount - successCount > 1000) ? String.Format("{0:#,0.##}", ((double)tryCount - successCount / 1000)) : String.Format("{0:#,0}", (tryCount - successCount));
                            CoinLabel.Text = (tryCount * 1.68 > 1000) ? String.Format("{0:##,##0}", tryCount * 1.68) + "K" : String.Format("{0:##,##0.00}", tryCount * 1.68) + "K";
                        }));

                        for (int i = 0; i < 6; i++)
                        {
                            if (db.Select(25 + (10 * i), true).Count != 0)
                            {
                                var perCount = db.Select(25 + (10 * i), true).Count;
                                var scsCount = db.Select(25 + (10 * i), true, true).Count;
                                var height = 250 * scsCount / perCount;

                                mainForm.Invoke(new Action(delegate ()
                                {
                                    enhanceGraph[i].Height = height;
                                    enhanceGraph[i].Location = new Point(enhanceGraph[i].Location.X, 486 - height);
                                    tooltipEText[i] = ($"시행횟수 : {perCount}\n성공횟수 : {scsCount}\n실패횟수 : {perCount - scsCount}");
                                }));
                            }

                            if (db.Select(25 + (10 * i), false).Count != 0)
                            {
                                var height = 250 * db.Select(25 + (10 * i), false, true).Count / db.Select(25 + (10 * i), false).Count;

                                mainForm.Invoke(new Action(delegate ()
                                {
                                    var perCount = db.Select(25 + (10 * i), false).Count;
                                    var scsCount = db.Select(25 + (10 * i), false, true).Count;
                                    reductionGraph[i].Height = height;
                                    reductionGraph[i].Location = new Point(reductionGraph[i].Location.X, 486 - height);
                                    tooltipRText[i] = ($"시행횟수 : {perCount}\n성공횟수 : {scsCount}\n실패횟수 : {perCount - scsCount}");
                                }));
                            }
                        }
                        Thread.Sleep(1000);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        pageState = false;
                    }
                }
            }).Start();
        }

        public void GraphMouseMove(object sender, EventArgs e, ToolTip DashboardGraphToolTip)
        {
            if (((PictureBox)sender).Name == "EnhanceGraph75")
                SetTooltip(5, true, sender, DashboardGraphToolTip);
            else if (((PictureBox)sender).Name == "EnhanceGraph65")
                SetTooltip(4, true, sender, DashboardGraphToolTip);
            else if (((PictureBox)sender).Name == "EnhanceGraph55")
                SetTooltip(3, true, sender, DashboardGraphToolTip);
            else if (((PictureBox)sender).Name == "EnhanceGraph45")
                SetTooltip(2, true, sender, DashboardGraphToolTip);
            else if (((PictureBox)sender).Name == "EnhanceGraph35")
                SetTooltip(1, true, sender, DashboardGraphToolTip);
            else if (((PictureBox)sender).Name == "EnhanceGraph25")
                SetTooltip(0, true, sender, DashboardGraphToolTip);
            else if (((PictureBox)sender).Name == "ReductionGraph75")
                SetTooltip(5, false, sender, DashboardGraphToolTip);
            else if (((PictureBox)sender).Name == "ReductionGraph65")
                SetTooltip(4, false, sender, DashboardGraphToolTip);
            else if (((PictureBox)sender).Name == "ReductionGraph55")
                SetTooltip(3, false, sender, DashboardGraphToolTip);
            else if (((PictureBox)sender).Name == "ReductionGraph45")
                SetTooltip(2, false, sender, DashboardGraphToolTip);
            else if (((PictureBox)sender).Name == "ReductionGraph35")
                SetTooltip(1, false, sender, DashboardGraphToolTip);
            else if (((PictureBox)sender).Name == "ReductionGraph25")
                SetTooltip(0, false, sender, DashboardGraphToolTip);
        }

        private void SetTooltip(int num, bool adjustment, object sender, ToolTip DashboardGraphToolTip)
        {
            DashboardGraphToolTip.ToolTipTitle = "상세정보";
            if (adjustment)
                DashboardGraphToolTip.SetToolTip((PictureBox)sender, $"{tooltipEText[num]}");
            else
                DashboardGraphToolTip.SetToolTip((PictureBox)sender, $"{tooltipRText[num]}");
        }

        public void AddItemToListBox(string engravingName, int percentage, bool success)
        {
            mainForm.Invoke(new Action(delegate ()
            {
                var image = resourceLoader.GetImageToName(engravingName);
                image = image.Resize(new OpenCvSharp.Size(image.Width / 2, image.Height / 2));
                Bitmap bitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(image);

                if (itemCount < 7)
                    itemCount += 1;

                for (int i = itemCount; i > 1; --i)
                {
                    itemImages[i - 1].Image = itemImages[i - 2].Image;
                    imageNames[i - 1].Text = imageNames[i - 2].Text;
                    successText[i - 1].Text = successText[i - 2].Text;
                }

                string successString = success ? "성공" : "실패";

                itemImages[0].Image = bitmap;
                imageNames[0].Text = engravingName;
                successText[0].Text = $"{percentage}% {successString}";

            }));

        }
    }
}
