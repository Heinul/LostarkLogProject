using LostarkLogProject.ControllFuncion;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostarkLogProject.TripodLog
{
    internal class TripodImageAnalysis
    {
        DisplayCapture displayCapture;
        MainForm mainForm;
        ResourceLoader resourceLoader;

        public TripodImageAnalysis(MainForm mainForm, ResourceLoader resourceLoader)
        {
            this.mainForm = mainForm;
            this.resourceLoader = resourceLoader;
            displayCapture = new DisplayCapture();
        }

        bool threadState = false;
        public void Run()
        {
            if (!threadState)
            {
                threadState = true;

                new Thread(CaptureDisplay).Start();
                //new Thread(ImageAnalysisThread).Start();
                //new Thread(SaveData).Start();
            }
        }

        public void Stop()
        {
            threadState = false;
            displayQueue.Clear();
        }

        bool tripodWindowState = false;
        Queue<Mat> displayQueue = new Queue<Mat>();
        private void CaptureDisplay()
        {
            while (threadState)
            {
                Mat display = displayCapture.GetMatCapture();
                SerchTripodText(display);

                if (tripodWindowState)
                {
                    displayQueue.Enqueue(display);
                    mainForm.SetImageAnalysisStateText("트라이포드 부여 기록중");
                    mainForm.SetStateImage(2);
                }
                else
                {
                    mainForm.SetImageAnalysisStateText("Error : 트라이포드화면 인식 불가");
                    mainForm.SetStateImage(4);
                }
                Thread.Sleep(1);
            }
        }

        private void SerchTripodText(Mat display)
        {
            Mat result = new Mat();

            Cv2.MatchTemplate(display, resourceLoader.GetSuccessTextImage(), result, TemplateMatchModes.CCoeffNormed);
            OpenCvSharp.Point minloc, maxloc;
            double minval, maxval;

            Cv2.MinMaxLoc(result, out minval, out maxval, out minloc, out maxloc);

            if (maxval > 0.8)
            {
                tripodWindowState = true;
            }
            else
            {
                tripodWindowState = false;
            }
        }

        public void EnqueueDisplayData(Mat display)
        {

        }
    }
}
