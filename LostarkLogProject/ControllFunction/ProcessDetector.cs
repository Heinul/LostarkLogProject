using Google.Cloud.Firestore;
using Microsoft.Web.WebView2.WinForms;
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
        bool threadState = false;
        MainForm mainForm;
        DisplayCapture displayCapture;

        public ProcessDetector(MainForm mainForm, ResourceLoader resourceLoader, WebView2 webView21)
        {
            this.mainForm = mainForm;
            displayCapture = new DisplayCapture(mainForm, resourceLoader, webView21);
        }

        private void ProcessDetection()
        {
            while (threadState)
            {
                Process[] processList = Process.GetProcessesByName("LostArk");
                if (processList.Length > 0)
                {
                    if (lostarkState == false)
                    {
                        lostarkState = true;
                        mainForm.SetStateImage(1);
                        displayCapture.StartDisplayCapture();
                    }
                }
                else
                {
                    if (lostarkState == true)
                    {
                        lostarkState = false;
                        mainForm.SetStateImage(0);
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
            threadState = true;
            Thread thread = new Thread(ProcessDetection);
            thread.Start();
        }

        public void Stop()
        {
            threadState = false;
        }

        public void TestRun()
        {
            displayCapture.StartDisplayCapture();
        }
    }
}
