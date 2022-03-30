using Google.Cloud.Firestore;
using LostarkLogProject.AbilityStoneLog;
using LostarkLogProject.TripodLog;
using OpenCvSharp;

namespace LostarkLogProject.ControllFunction
{
    internal class ImageAnalysis
    {
        MainForm mainForm;
        ResourceLoader resourceLoader;
        FirestoreDb firestoreDb;

        public ImageAnalysis(MainForm mainForm, ResourceLoader resourceLoader, FirestoreDb firestoreDb)
        {
            this.mainForm = mainForm;
            this.resourceLoader = resourceLoader;
            this.firestoreDb = firestoreDb;

            for (int i = 0; i < 3; i++)
                previousEngravingSuccessData[i] = new int[10];
        }
        Queue<Mat> displayQueue = new Queue<Mat>();
        Queue<int> tokenQueue = new Queue<int>();

        public void EnqueueDisplayMat(int token, Mat display)
        {
            // Token 0 : 어빌리티스톤, 1 : 트라이포드
            tokenQueue.Enqueue(token);
            displayQueue.Enqueue(display.Clone());
        }

        bool threadState = false;
        public void Run()
        {
            if (!threadState)
            {
                threadState = true;
                new Thread(ImageAnalysisThread).Start();
                new Thread(SaveData).Start();
            }
        }

        public void Stop()
        {
            threadState = false;
        }

        private void ImageAnalysisThread()
        {

            string[] engravingName = new string[3];
            int[][] engravingSuccessData = new int[3][];
            for (int i = 0; i < 3; i++)
                engravingSuccessData[i] = new int[10] { 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 };

            int previousTripodPercentage = 0;
            int previousTripodSuccess = 0; // 0 인식오류, 1 성공, 2 실패

            while (threadState)
            {
                if (tokenQueue.Count > 0 && displayQueue.Count > 0)
                {
                    int percentage = 0;
                    int success = 0;
                    int token = tokenQueue.Dequeue();
                    Mat display = displayQueue.Dequeue();

                    if (token == 0)
                    {
                        // 어빌리티스톤
                        percentage = PercentageCheck(display);

                        for (int i = 0; i < 3; i++)
                        {
                            engravingName[i] = EngravingImageCheck(display, i);
                            engravingSuccessData[i] = EngravingSuccessCheck(display, i);
                        }

                        bool errorCheck = false;
                        for (int i = 0; i < 10; i++)
                        {
                            if (engravingSuccessData[0][i] == 3 || engravingSuccessData[1][i] == 3 || engravingSuccessData[2][i] == 3)
                            {
                                errorCheck = true;
                                break;
                            }
                        }
                        if (!errorCheck)
                            ComparisonData(percentage, engravingName, engravingSuccessData);
                    }
                    else if (token == 1)
                    {
                        //트라이포드
                        int percentageIndex = TripodPercentageCheck(display);
                        bool additionalMeterial = false;
                        if (percentageIndex != 7)
                        {
                            percentage = tripodPercentageList[percentageIndex];
                            additionalMeterial = percentageIndex % 2 == 1 ? true : false;
                        }
                        if (previousTripodPercentage > 0)
                            success = TripodSuccessCheck(display);

                        if (percentage > 0)
                            previousTripodPercentage = percentage;
                        if (success > 0)
                            previousTripodSuccess = success;

                        if (previousTripodSuccess != 0 && previousTripodPercentage != 0)
                        {
                            var successValue = previousTripodSuccess == 1 ? true : false;
                            PushTripodData(successValue, previousTripodPercentage, additionalMeterial);
                            previousTripodPercentage = 0;
                            previousTripodSuccess = 0;
                        }
                    }
                }
                Thread.Sleep(1);
            }
        }




        #region 어빌리티스톤 이미지 처리 함수
        int[] posX1 = { 745, 783, 822, 862, 900, 939, 978, 1018, 1056, 1096 };
        int[] posX2 = { 744, 782, 822, 862, 900, 939, 978, 1017, 1054, 1094 };
        int[] posX_Reduction = { 742, 781, 820, 859, 898, 937, 976, 1015, 1054, 1093 };
        int[] posY = { 388, 481, 608 };
        int[] abilityPercentageList = { 75, 65, 55, 45, 35, 25 };

        private int PercentageCheck(Mat display)
        {
            Mat percentageSerchResult = new Mat();
            Mat percentageArea = display.SubMat(new Rect(1100, 200, 200, 200));
            OpenCvSharp.Point minloc, maxloc;
            double minval, maxval;

            double[] val = new double[6];
            // 퍼센트 확인

            for (int i = 0; i < 6; i++)
            {
                Mat gray = new Mat();
                Cv2.CvtColor(percentageArea, gray, ColorConversionCodes.BGR2GRAY);

                Cv2.MatchTemplate(gray, resourceLoader.GetPercentageGrayImage(i), percentageSerchResult, TemplateMatchModes.CCoeffNormed);
                Cv2.MinMaxLoc(percentageSerchResult, out minval, out maxval, out minloc, out maxloc);
                val[i] = maxval;
            }


            var maxVal = val.Max();
            var maxIndex = val.ToList().IndexOf(maxVal);

            if (maxVal < 0.5)
            {
                return 0;
            }
            else
            {
                return abilityPercentageList[maxIndex];
            }
        }

        private string EngravingImageCheck(Mat image, int num)
        {
            Mat engravingSerchResult = new Mat();
            Mat engravingArea = GetEngravingImageArea(image, num);
            OpenCvSharp.Point minloc, maxloc;
            double minval, maxval;
            if (num != 2)
            {
                for (int i = 0; i < 43; ++i)
                {
                    Mat img = resourceLoader.GetEnhanceImage(i);
                    Cv2.MatchTemplate(engravingArea, img, engravingSerchResult, TemplateMatchModes.CCoeffNormed);
                    Cv2.MinMaxLoc(engravingSerchResult, out minval, out maxval, out minloc, out maxloc);
                    if (maxval > 0.95)
                    {
                        return resourceLoader.GetEnhanceName(i);
                    }
                }
            }
            else
            {
                for (int i = 0; i < 4; ++i)
                {
                    Mat img = resourceLoader.GetReductionImage(i);
                    Cv2.MatchTemplate(engravingArea, img, engravingSerchResult, TemplateMatchModes.CCoeffNormed);
                    Cv2.MinMaxLoc(engravingSerchResult, out minval, out maxval, out minloc, out maxloc);
                    if (maxval > 0.95)
                    {
                        return resourceLoader.GetReductionName(i);
                    }
                }
            }
            return "인식실패";
        }

        private Mat GetEngravingImageArea(Mat display, int num)
        {
            if (num == 0)
            {
                return display.SubMat(new Rect(620, 332, 96, 96));
            }
            else if (num == 1)
            {
                return display.SubMat(new Rect(620, 426, 96, 96));
            }
            else if (num == 2)
            {
                return display.SubMat(new Rect(620, 552, 96, 96));
            }
            else
            {
                return null;
            }

        }

        private int[] EngravingSuccessCheck(Mat display, int num)
        {
            /*
             * 0 : 아직 안누름, 1 : 실패, 2 : 성공, 3 : 인식오류
             */
            int[] data = new int[10];
            int r = 0, g = 0, b = 0;
            if (num == 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    b = GetAveragePixel(display, posY[num], posX1[i], 0);
                    g = GetAveragePixel(display, posY[num], posX1[i], 1);
                    r = GetAveragePixel(display, posY[num], posX1[i], 2);
                    if (r < 50 && g < 50 && b < 50)
                        data[i] = 0;
                    else if (r < 130 && g < 130 && b < 130)
                        data[i] = 1;
                    else if (b > 150)
                        data[i] = 2;
                    else
                        data[i] = 3;
                }
            }
            else if (num == 1)
            {
                for (int i = 0; i < 10; i++)
                {
                    b = GetAveragePixel(display, posY[num], posX2[i], 0);
                    g = GetAveragePixel(display, posY[num], posX2[i], 1);
                    r = GetAveragePixel(display, posY[num], posX2[i], 2);

                    if (r < 50 && g < 50 && b < 50)
                        data[i] = 0;
                    else if (r < 130 && g < 130 && b < 130)
                        data[i] = 1;
                    else if (b > 150)
                        data[i] = 2;
                    else
                        data[i] = 3;
                }
            }
            else if (num == 2)
            {
                for (int i = 0; i < 10; i++)
                {
                    b = GetAveragePixel(display, posY[num], posX_Reduction[i], 0);
                    g = GetAveragePixel(display, posY[num], posX_Reduction[i], 1);
                    r = GetAveragePixel(display, posY[num], posX_Reduction[i], 2);

                    if (r < 50 && g < 50 && b < 50)
                        data[i] = 0;
                    else if (r < 130 && g < 130 && b < 130)
                        data[i] = 1;
                    else if (r > 130)
                        data[i] = 2;
                    else
                        data[i] = 3;
                }
            }

            return data;
        }

        private int GetAveragePixel(Mat display, int y, int x, int bgr)
        {
            int result = (display.At<Vec3b>(y, x)[bgr] +
                        display.At<Vec3b>(y, x)[bgr] +
                        display.At<Vec3b>(y, x)[bgr] +
                        display.At<Vec3b>(y, x)[bgr] +
                        display.At<Vec3b>(y, x)[bgr]) / 5;
            return result;
        }

        private int previousPercentage = 0;
        private string[] previousEngravingName = new string[3] { "", "", "" };
        private int[][] previousEngravingSuccessData = new int[3][];
        private void ComparisonData(int percentage, string[] engravingName, int[][] engravingSuccessData)
        {

            // 첫입력
            if (previousEngravingName[0] == "")
            {
                previousPercentage = percentage;
                for (int i = 0; i < 3; i++)
                {
                    previousEngravingName[i] = engravingName[i];
                    for (int j = 0; j < 10; j++)
                    {
                        previousEngravingSuccessData[i][j] = engravingSuccessData[i][j];
                    }
                }
                return;
            }

            if (engravingName[0] == "인식실패" || engravingName[1] == "인식실패" || engravingName[2] == "인식실패")
            {
                return;
            }

            if (previousPercentage == 0)
            {
                previousPercentage = percentage;
                return;
            }


            var distance1 = GetEngravingDistance(previousEngravingSuccessData, engravingSuccessData, 0);
            var distance2 = GetEngravingDistance(previousEngravingSuccessData, engravingSuccessData, 1);
            var distance3 = GetEngravingDistance(previousEngravingSuccessData, engravingSuccessData, 2);
            int percentageDistace = previousPercentage - percentage;
            // 어빌리티 스톤 변경
            if (!previousEngravingName[0].Equals(engravingName[0]) || !previousEngravingName[1].Equals(engravingName[1]) || !previousEngravingName[2].Equals(engravingName[2]))
            {
                Console.WriteLine("돌변경1");
                //각인이름이 달라진 경우 갱신
                previousPercentage = percentage;
                for (int i = 0; i < 3; i++)
                {
                    previousEngravingName[i] = engravingName[i];
                    for (int j = 0; j < 10; j++)
                    {
                        previousEngravingSuccessData[i][j] = engravingSuccessData[i][j];
                    }
                }
                return;
            }
            else if (distance1[0] == 0 && distance2[0] == 0 && distance3[0] == 0)
            {
                return;
            }
            else if (distance1[0] < 0 || distance1[0] > 2 || distance2[0] < 0 || distance2[0] > 2 || distance3[0] < 0 || distance3[0] > 2)
            {
                // 각인은 같으나 돌을 바꾼경우 (값의 차가 +1~+2가 아닌경우) 갱신
                Console.WriteLine("돌변경2");
                previousPercentage = percentage;
                for (int i = 0; i < 3; i++)
                {
                    previousEngravingName[i] = engravingName[i];
                    for (int j = 0; j < 10; j++)
                    {
                        previousEngravingSuccessData[i][j] = engravingSuccessData[i][j];
                    }
                }
                return;
            }
            else if ((Math.Abs(percentageDistace) > 10 || previousPercentage == percentage) && !(distance1[1] == 10 || distance2[1] == 10 || distance3[1] == 10))
            {
                return;
            }
            else if (distance1[0] == 1 || distance1[0] == 2 || distance2[0] == 1 || distance2[0] == 2 || distance3[0] == 1 || distance3[0] == 2)
            {
                //값이 범위 내로 증가하면 강화를 했다는거니까 저장하고 갱신하면됨
                if (distance1[0] == 1)
                {
                    PushAbilityStoneData(previousPercentage, previousEngravingName[0], false, true, distance1[1]);
                }
                else if (distance1[0] == 2)
                {
                    PushAbilityStoneData(previousPercentage, previousEngravingName[0], true, true, distance1[1]);
                }
                else if (distance2[0] == 1)
                {
                    PushAbilityStoneData(previousPercentage, previousEngravingName[1], false, true, distance2[1]);
                }
                else if (distance2[0] == 2)
                {
                    PushAbilityStoneData(previousPercentage, previousEngravingName[1], true, true, distance2[1]);
                }
                else if (distance3[0] == 1)
                {
                    PushAbilityStoneData(previousPercentage, previousEngravingName[2], false, false, distance3[1]);
                }
                else if (distance3[0] == 2)
                {
                    PushAbilityStoneData(previousPercentage, previousEngravingName[2], true, false, distance3[1]);
                }

                previousPercentage = percentage;
                for (int i = 0; i < 3; i++)
                {
                    previousEngravingName[i] = engravingName[i];
                    for (int j = 0; j < 10; j++)
                    {
                        previousEngravingSuccessData[i][j] = engravingSuccessData[i][j];
                    }
                }
                return;
            }
        }

        private int[] GetEngravingDistance(int[][] previousData, int[][] engravingData, int num)
        {
            int[] result = new int[2];
            var a = ArrayToLong(engravingData[num]);
            var b = ArrayToLong(previousData[num]);
            double distance = a - b;
            double digits = 0;

            if (distance != 0)
            {
                digits = Math.Truncate(Math.Log10(Math.Abs(distance)));
                distance = distance / Math.Pow(10, digits);
            }

            result[0] = (int)Math.Ceiling(distance);
            result[1] = (int)(10 - digits);
            return (int[])result.Clone();
        }

        private long ArrayToLong(int[] data)
        {
            string str = "";
            for (int i = 0; i < data.Length; i++)
            {
                str += data[i].ToString();
            }

            return long.Parse(str);
        }

        Queue<AbilityItem> abilityItemQueue = new Queue<AbilityItem>();
        private void PushAbilityStoneData(int percentage, string engravingName, bool success, bool adjustment, int digit)
        {
            //큐에 데이터 올리고 다른 스레드로 저장 작업 처리
            AbilityItem data = new AbilityItem(percentage, engravingName, success, adjustment, digit, firestoreDb);
            abilityItemQueue.Enqueue(data);
        }
        #endregion

        #region 트라이포드 이미지 처리 함수

        int[] tripodPercentageList = { 5, 10, 15, 30, 30, 60, 100 };
        private int TripodPercentageCheck(Mat display)
        {

            Mat percentageSerchResult = new Mat();
            Mat percentageArea = display.SubMat(new Rect(870, 440, 200, 100));
            OpenCvSharp.Point minloc, maxloc;
            double minval, maxval;

            double[] val = new double[7];
            // 퍼센트 확인
            for (int i = 0; i < 7; i++)
            {
                Cv2.MatchTemplate(percentageArea, resourceLoader.GetTripodPercentageImage(i), percentageSerchResult, TemplateMatchModes.CCoeffNormed);
                Cv2.MinMaxLoc(percentageSerchResult, out minval, out maxval, out minloc, out maxloc);
                val[i] = maxval;
            }
            var maxVal = val.Max();
            var maxIndex = val.ToList().IndexOf(maxVal);

            if (maxVal < 0.5)
            {
                return 7;
            }
            else
            {
                // 5 [10] 15 [30] 30 [60] 100
                return maxIndex;
            }
        }

        private int TripodSuccessCheck(Mat display)
        {
            Mat result = new Mat();
            Mat successArea = display.SubMat(new Rect(850, 220, 200, 75));
            OpenCvSharp.Point minloc, maxloc;
            double minval, maxval;

            // 성공실패 확인
            Cv2.MatchTemplate(successArea, resourceLoader.GetTripodFailImage(), result, TemplateMatchModes.CCoeffNormed);
            Cv2.MinMaxLoc(result, out minval, out maxval, out minloc, out maxloc);

            if (maxval > 0.9)
            {
                return 2;
            }

            Cv2.MatchTemplate(successArea, resourceLoader.GetTripodSuccessImage(), result, TemplateMatchModes.CCoeffNormed);
            Cv2.MinMaxLoc(result, out minval, out maxval, out minloc, out maxloc);
            if (maxval > 0.9)
            {
                return 1;
            }

            return 0;

        }

        Queue<TripodItem> tripodItemQueue = new Queue<TripodItem>();
        private void PushTripodData(bool previousTripodSuccess, int previousTripodPercentage, bool additionalMeterial)
        {
            TripodItem item = new TripodItem(previousTripodSuccess, previousTripodPercentage, additionalMeterial, firestoreDb);
            tripodItemQueue.Enqueue(item);
        }
        #endregion

        private void SaveData()
        {
            //데이터 저장할 때 서버로 데이터 전송
            while (threadState)
            {
                if (!mainForm.TestModeCheck())
                {
                    if (abilityItemQueue.Count > 0)
                    {
                        var item = abilityItemQueue.Dequeue();

                        item.SendData();
                        item.SaveData();
                        mainForm.AddItemToListBox(item.GetEngravingName(), item.GetPercentage(), item.GetSuccess());
                    }

                    if (tripodItemQueue.Count > 0)
                    {
                        var item = tripodItemQueue.Dequeue();
                        item.SendData();
                        item.SaveData();
                    }
                }
                else
                {
                    if (abilityItemQueue.Count > 0)
                    {
                        var item = abilityItemQueue.Dequeue();

                        item.SaveData();
                        mainForm.AddItemToListBox(item.GetEngravingName(), item.GetPercentage(), item.GetSuccess());
                    }

                    if (tripodItemQueue.Count > 0)
                    {
                        var item = tripodItemQueue.Dequeue();
                        item.SaveData();
                    }
                }

                Thread.Sleep(10);
            }
        }
    }
}
