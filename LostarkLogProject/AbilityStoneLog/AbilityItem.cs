using Google.Cloud.Firestore;
using LostarkLogProject.Properties;
using Microsoft.Web.WebView2.WinForms;

namespace LostarkLogProject.AbilityStoneLog
{
    internal class AbilityItem
    {
        private int percentage = 0;
        private string engravingName = "";
        private bool success = false;
        private bool adjustment = false; //true 강화, false 감소
        private int digit = 0;
        MainForm mainForm;
        WebView2 webBrowser;
        AbilityStoneDBManager database = null;

        public AbilityItem(int percentage, string engravingName, bool success, bool adjustment, int digit, MainForm mainForm, WebView2 webBrowser)
        {
            this.database = new AbilityStoneDBManager();
            this.mainForm = mainForm;
            this.webBrowser = webBrowser;

            this.percentage = percentage;
            this.engravingName = engravingName;
            this.success = success;
            this.adjustment = adjustment;
            this.digit = digit;
        }

        public void SaveData()
        {
            database.Insert(percentage, engravingName, success, adjustment, digit);
        }

        public void SendData()
        {
            string url = "https://lostarklogproject.web.app/SendToServerTripod.html";
            string data = $"?Adjustment={adjustment}&Digit={digit}&EngravingName={engravingName}&Percentage={percentage}&UID={ Settings.Default.UID}";
            string str = url + data;

            mainForm.Invoke(new Action(delegate ()
            {
                webBrowser.Source = new System.Uri(str, System.UriKind.Absolute);
                webBrowser.Source = new System.Uri("https://lostarklogproject.web.app/");
            }));

            //CollectionReference coll = firestoreDb.Collection($"AbilityStoneDataBase");
            //Dictionary<string, object> data = new Dictionary<string, object>()
            //{
            //    {"Percentage", percentage},
            //    {"EngravingName", engravingName},
            //    {"Success", success },
            //    {"Adjustment", adjustment},
            //    {"Digit", digit},
            //    {"UID", Settings.Default.UID },
            //    {"Timestamp", Timestamp.FromDateTime(DateTime.UtcNow) }
            //};
            //coll.AddAsync(data);
            Console.WriteLine("Send To Server With AbilityStone");
        }

        public string GetEngravingName()
        {
            return engravingName;
        }

        public int GetPercentage()
        {
            return percentage;
        }

        public bool GetSuccess()
        {
            return success;
        }
    }
}
