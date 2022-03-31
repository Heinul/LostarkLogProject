using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostarkLogProject.TripodLog
{
    internal class TripodDashBoard
    {
        MainForm mainForm;
        Label[] tripodSuccessPercentageLabel, tripodCountLabel;
        public TripodDashBoard(MainForm mainForm, Label[] tripodSuccessPercentageLabel, Label[] tripodCountLabel)
        {
            this.mainForm = mainForm;
            this.tripodSuccessPercentageLabel = tripodSuccessPercentageLabel;
            this.tripodCountLabel = tripodCountLabel;
        }

        bool pageState = false;
        public void SetPageState(bool state)
        {
            pageState = state;
        }

        int[] tripodPercentageList = { 5, 10, 15, 30, 30, 60, 100 };
        
        public void UpdateDetailPage()
        {
            new Thread(() => {
                TripodDBManager dBManager = new TripodDBManager();
                while (pageState)
                {
                    for(int i = 0; i < 7; i++)
                    {
                        bool material = i % 2 == 1 ? true : false;
                        var tripodCount = dBManager.Select(tripodPercentageList[i], material).Count;
                        var tripodPercentage = dBManager.Select(tripodPercentageList[i], material, true).Count;
                        var percentage = tripodCount > 0 ? tripodPercentage * 100 / (double)tripodCount : 0;
                        mainForm.Invoke(new Action(delegate ()
                        {
                            tripodSuccessPercentageLabel[i].Text = $"{percentage.ToString("F1")}%";
                            tripodCountLabel[i].Text = $"{tripodCount}번 중";
                        }));
                    }
                    Thread.Sleep(100);
                    
                }
            }).Start();
        }
    }
}
