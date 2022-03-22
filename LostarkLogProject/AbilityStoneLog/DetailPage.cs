using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostarkLogProject.AbilityStoneLog
{
    internal class DetailPage
    {
        private MainForm mainForm;
        private bool pageState = false;
        private DateTimePicker startDateTimePicker, endDateTimePicker;

        Label[] detailPercentage;

        PictureBox graph;
        public DetailPage(MainForm mainForm, DateTimePicker startDateTimePicker, DateTimePicker endDateTimePicker, Label[] detailPercentage, PictureBox graph)
        {
            this.mainForm = mainForm;

            this.startDateTimePicker = startDateTimePicker;
            endDateTimePicker.Value = DateTime.Now;
            this.endDateTimePicker = endDateTimePicker;

            this.detailPercentage = detailPercentage;
            this.graph = graph;

            UpdateDetailPage();
        }

        public void SetPageState(bool state)
        {
            pageState = state;
        }

        int[] dotPosition = { 475, 389, 305, 221, 137, 47 }, ePosition = { 505, 419, 335, 251, 167, 77 }, rPosition = { 537, 451, 367, 283, 199, 110 };

        public void UpdateDetailPage()
        {
            new Thread(() => {
                AbilityStoneDBManager db = new AbilityStoneDBManager();
                while (pageState)
                {
                    mainForm.Invoke(new Action(delegate ()
                    {
                        endDateTimePicker.Value = DateTime.Now;
                    }));
                    try
                    {
                        int[] totalDot = new int[6];
                        int[] heightEData = new int[6];
                        int[] heightRData = new int[6];
                        for (int i = 0; i < 6; i++)
                        {
                            var perECount = db.Select(startDateTimePicker.Value, endDateTimePicker.Value, 25 + (10 * i), true).Count;
                            var scsECount = db.Select(startDateTimePicker.Value, endDateTimePicker.Value, 25 + (10 * i), true, true).Count;

                            var perRCount = db.Select(startDateTimePicker.Value, endDateTimePicker.Value, 25 + (10 * i), false).Count;
                            var scsRCount = db.Select(startDateTimePicker.Value, endDateTimePicker.Value, 25 + (10 * i), false, true).Count;

                            var totalTryCount = db.Select(startDateTimePicker.Value, endDateTimePicker.Value, 25 + (10 * i)).Count;
                            var scsTCount = scsECount + scsRCount;

                            totalDot[i] = totalTryCount > 0 ? (400 * scsTCount / totalTryCount) : 0;
                            heightEData[i] = perECount > 0 ? (400 * scsECount / perECount) : 0;
                            heightRData[i] = perRCount > 0 ? (400 * scsRCount / perRCount) : 0;

                            mainForm.Invoke(new Action(delegate ()
                            {
                                detailPercentage[i].Text = totalTryCount != 0 ? (100 * scsTCount / (double)totalTryCount).ToString("0.0") + "%" : "0%";
                            }));

                        }

                        Mat grpImage = new Mat(new OpenCvSharp.Size(530, 415), MatType.CV_8UC3);
                        Cv2.Rectangle(grpImage, new Rect(0, 0, 530, 415), Scalar.White, -1, LineTypes.AntiAlias);
                        for (int i = 0; i < 6; ++i)
                        {
                            Cv2.Rectangle(grpImage, new Rect(ePosition[i] - 56, 415 - heightEData[i], 20, heightEData[i]), Scalar.CornflowerBlue, -1, LineTypes.AntiAlias);
                            Cv2.Rectangle(grpImage, new Rect(rPosition[i] - 56, 415 - heightRData[i], 20, heightRData[i]), Scalar.Tomato, -1, LineTypes.AntiAlias);
                            Cv2.Rectangle(grpImage, new Rect(dotPosition[i], 415 - totalDot[i], 5, 5), Scalar.MediumOrchid, -1, LineTypes.AntiAlias);
                            if (i < 5)
                                Cv2.Line(grpImage, new OpenCvSharp.Point(dotPosition[i], 415 - totalDot[i]), new OpenCvSharp.Point(dotPosition[i + 1], 415 - totalDot[i + 1]), Scalar.MediumOrchid, 1);
                        }

                        mainForm.Invoke(new Action(delegate ()
                        {
                            graph.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(grpImage);
                        }));

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
    }
}
