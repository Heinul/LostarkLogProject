using Google.Cloud.Firestore;
using LostarkLogProject.AbilityStoneLog;
using LostarkLogProject.ControllFunction;
using LostarkLogProject.TripodLog;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace LostarkLogProject.ControllFuncion
{
    internal class DisplayCapture
    {
        ResourceLoader resourceLoader;
        AbilityStoneImageAnalysis abilityStoneImageAnalysis;
        TripodImageAnalysis tripodImageAnalysis;
        ImageAnalysis imageAnalysis;

        public DisplayCapture()
        {

        }
        public DisplayCapture(/*MainForm mainform,*/ ResourceLoader resourceLoader/*, FirestoreDb firestoreDb*/)
        {
            this.resourceLoader = resourceLoader;
            imageAnalysis = new ImageAnalysis(resourceLoader, null);
            
            //abilityStoneImageAnalysis = new AbilityStoneImageAnalysis(mainform, resourceLoader, firestoreDb);
            //tripodImageAnalysis = new TripodImageAnalysis(mainform, resourceLoader);
        }

        public Mat GetMatCapture()
        {
            /* 
             * 세로크기 기준으로 16:9를 생성 했을 때 가로 길이의 차이를 계산
             * (Screen.PrimaryScreen.Bounds.Width - (Screen.PrimaryScreen.Bounds.Height / 9 * 16));
             */
            // 16:9로 크기 정규화
            int normalization = (Screen.PrimaryScreen.Bounds.Width - Screen.PrimaryScreen.Bounds.Height / 9 * 16) / 2;
            Bitmap bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Graphics gr = Graphics.FromImage(bmp);
            gr.CopyFromScreen(0, 0, 0, 0, bmp.Size);
            Mat display = bmp.ToMat();
            display = display.SubMat(new Rect(normalization, 0, display.Height * 16 / 9, display.Height));
            display = display.Resize(new OpenCvSharp.Size(1920, 1080));

            GC.Collect();
            GC.WaitForPendingFinalizers();

            return display;
        }

        public Bitmap GetBitmapCapture()
        {
            int normalization = (Screen.PrimaryScreen.Bounds.Width - Screen.PrimaryScreen.Bounds.Height / 9 * 16) / 2;
            Bitmap bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Height / 9 * 16, Screen.PrimaryScreen.Bounds.Height);
            Graphics gr = Graphics.FromImage(bmp);
            gr.CopyFromScreen(normalization, 0, normalization, 0, bmp.Size);
            bmp = new Bitmap(bmp, new System.Drawing.Size(1920, 1080));
            GC.Collect();
            GC.WaitForPendingFinalizers();
            return bmp;
        }

        bool threadState = false;
        public void StartDisplayCapture()
        {
            threadState = true;
            new Thread(ClassificationWorkState).Start();
            imageAnalysis.Run();
        }

        public void StopDisplayCapture()
        {
            threadState = false;
            imageAnalysis.Stop();
        }

        private void ClassificationWorkState()
        {
            while (threadState)
            {
                Mat display = GetMatCapture();

                OpenCvSharp.Point minloc, maxloc;
                double minval, maxval;

                Mat resultAbility = new Mat();
                Cv2.MatchTemplate(display, resourceLoader.GetSuccessTextImage(), resultAbility, TemplateMatchModes.CCoeffNormed);
                Cv2.MinMaxLoc(resultAbility, out minval, out maxval, out minloc, out maxloc);
                if (maxval > 0.8)
                {
                    imageAnalysis.EnqueueDisplayMat(0, display);
                    //mainForm.SetImageAnalysisStateText("어빌리티스톤 세공 기록중");
                    //mainForm.SetStateImage(2);
                    continue;
                }
                resultAbility.Dispose();

                
                Mat resultTripod = new Mat();
                Cv2.MatchTemplate(display, resourceLoader.GetTripodTextImage(), resultTripod, TemplateMatchModes.CCoeffNormed);
                Cv2.MinMaxLoc(resultTripod, out minval, out maxval, out minloc, out maxloc);
                if (maxval > 0.8)
                {
                    imageAnalysis.EnqueueDisplayMat(1, display);
                    //mainForm.SetImageAnalysisStateText("트라이포드 부여 기록중");
                    //mainForm.SetStateImage(3);
                    continue;
                }
                resultTripod.Dispose();
                Thread.Sleep(1);
            }
        }

    }
}
