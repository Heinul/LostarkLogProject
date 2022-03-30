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
        bool lostarkState = false;
        MainForm mainForm;
        DisplayCapture displayCapture;
        public ProcessDetector(MainForm mainForm, ResourceLoader resourceLoader)
        {
            this.mainForm = mainForm;
            displayCapture = new DisplayCapture(resourceLoader);
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
                        //mainForm.SetStateImage(1);
                        //mainForm.SetImageAnalysisStateText("강화작업 대기중");
                        //mainForm.StartImageAnalysis();

                        displayCapture.StartDisplayCapture();
                    }
                }
                else
                {
                    if (lostarkState == true)
                    {
                        lostarkState = false;
                        //mainForm.SetStateImage(0);
                        //mainForm.SetImageAnalysisStateText("로스트아크 실행 대기중");
                        //mainForm.StopImageAnalysis();

                        displayCapture.StopDisplayCapture();
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

        public void TestRun()
        {
            displayCapture.StartDisplayCapture();
        }
    }
}
