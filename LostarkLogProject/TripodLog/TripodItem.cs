using Google.Cloud.Firestore;
using LostarkLogProject.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostarkLogProject.TripodLog
{
    internal class TripodItem
    {
        private bool success;
        private int percentage;
        private bool additionalMeterial;
        private FirestoreDb firestoreDb;
        

        TripodDBManager tripodDBManager;

        public TripodItem(bool success, int percentage, bool additionalMeterial, FirestoreDb firestoreDb)
        {
            this.success = success;
            this.percentage = percentage;
            this.firestoreDb = firestoreDb;
            this.additionalMeterial = additionalMeterial;

            tripodDBManager = new TripodDBManager();
        }

        internal void SendData()
        {
            CollectionReference coll = firestoreDb.Collection($"TripodDataBase");
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"Percentage", percentage},
                {"Success", success },
                {"Material", additionalMeterial },
                {"UID", Settings.Default.UID },
                {"Timestamp", Timestamp.FromDateTime(DateTime.UtcNow) }
            };
            coll.AddAsync(data);
            Console.WriteLine("Send To Server With Tripod");
        }

        internal void SaveData()
        {
            // TODO 데이터 저장하면 댐
            tripodDBManager.Insert(percentage, success, additionalMeterial);
        }
    }
}
