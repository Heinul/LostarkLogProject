using Google.Cloud.Firestore;
using LostarkLogProject.Properties;
using Microsoft.Web.WebView2.WinForms;

namespace LostarkLogProject.TripodLog
{
    internal class TripodItem
    {
        private bool success;
        private int percentage;
        private bool additionalMeterial;

        WebView2 webBrowser;
        MainForm mainForm;

        TripodDBManager tripodDBManager;

        public TripodItem(bool success, int percentage, bool additionalMeterial, WebView2 webBrowser, MainForm mainForm)
        {
            this.success = success;
            this.percentage = percentage;
            this.additionalMeterial = additionalMeterial;
            this.webBrowser = webBrowser;
            this.mainForm = mainForm;
            tripodDBManager = new TripodDBManager();
        }

        internal void SendData()
        {
            string url = "https://lostarklogproject.web.app/SendToServerTripod.html";
            string data = $"?Material={additionalMeterial}&Percentage={percentage}&Success={success}&UID={ Settings.Default.UID}0";
            string str = url + data;

            Console.WriteLine(str);
            mainForm.Invoke(new Action(delegate ()
            {
                webBrowser.Source = new System.Uri(url, System.UriKind.Absolute);
                webBrowser.Source = new System.Uri(str, System.UriKind.Absolute);
            }));

            Console.WriteLine("Send To Server With Tripod");
        }

        internal void SaveData()
        {
            tripodDBManager.Insert(percentage, success, additionalMeterial);
        }
    }
}
