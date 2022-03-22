using Google.Cloud.Firestore;

namespace LostarkLogProject.AbilityStoneLog
{
    internal class AbilityItem
    {
        private int percentage = 0;
        private string engravingName = "";
        private bool success = false;
        private bool adjustment = false; //true 강화, false 감소
        private int digit = 0;
        AbilityStoneDBManager database = null;
        FirestoreDb firestoreDb = null;

        public AbilityItem(int percentage, string engravingName, bool success, bool adjustment, int digit, FirestoreDb firestoreDb)
        {
            this.firestoreDb = firestoreDb;
            this.database = new AbilityStoneDBManager();

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
            CollectionReference coll = firestoreDb.Collection($"EngravingDataBase_{DateTime.Now.ToString("yyMMdd")}_{GetMacAddress()}");
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"Percentage", percentage},
                {"EngravingName", engravingName},
                {"Success", success },
                {"Adjusment", adjustment},
                {"Digit", digit},
                {"MAC", GetMacAddress() },
                {"Timestamp", Timestamp.FromDateTime(DateTime.UtcNow) }
            };
            coll.AddAsync(data);
            Console.WriteLine("Send To Server");
        }

        private string GetMacAddress()
        {
            return System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()[0].GetPhysicalAddress().ToString();
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
