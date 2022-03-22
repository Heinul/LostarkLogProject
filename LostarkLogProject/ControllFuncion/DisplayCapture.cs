using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace LostarkLogProject.ControllFuncion
{
    internal class DisplayCapture
    {
        public Mat GetMatCapture()
        {
            /* 
             * 세로크기 기준으로 16:9를 생성 했을 때 가로 길이의 차이를 계산
             * (Screen.PrimaryScreen.Bounds.Width - (Screen.PrimaryScreen.Bounds.Height / 9 * 16));
             */
            // 16:9로 크기 정규화
            int normalization = (Screen.PrimaryScreen.Bounds.Width - Screen.PrimaryScreen.Bounds.Height / 9 * 16) / 2;
            Bitmap bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Height / 9 * 16, Screen.PrimaryScreen.Bounds.Height);
            Graphics gr = Graphics.FromImage(bmp);
            gr.CopyFromScreen(normalization, 0, 0, 0, bmp.Size);
            Mat display = bmp.ToMat();
            display.Resize(new OpenCvSharp.Size(1920, 1080));

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
    }
}
