using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostarkLogProject.ControllFuncion
{
    internal class ProcessDetector
    {
        Thread thread;
        bool lostarkState = false;
        MainForm mainForm;
        public ProcessDetector(MainForm mainForm)
        {
            this.mainForm = mainForm;
            thread = new Thread(ProcessDetection);
        }

        private void ProcessDetection()
        {
            while (true)
            {
                Process[] processList = Process.GetProcessesByName("LostArk");
                if (processList.Length > 0)
                {
                    if (lostarkState == false)
                    {
                        lostarkState = true;
                        mainForm.SetImageAnalysisStateText("이미지탐지 대기중");
                        mainForm.StartImageAnalysis();
                    }

                }
                else
                {
                    if (lostarkState == true)
                    {
                        lostarkState = false;
                        mainForm.SetImageAnalysisStateText("로스트아크 실행 대기중");
                        mainForm.StopImageAnalysis();
                    }
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
                Thread.Sleep(5000);
            }
        }

        public void Run()
        {
            Thread thread = new Thread(ProcessDetection);
            thread.Start();
        }
    }
}
