using OpenCvSharp;
using LostarkLogProject.Properties;
namespace LostarkLogProject
{
    internal class ResourceLoader
    {
        private string[] enhanceList =
            {
            "각성",
            "강령술",
            "강화방패",
            "결투의대가",
            "구슬동자",
            "굳은의지",
            "급소타격",
            "기습의대가",
            "긴급구조",
            "달인의저력",
            "돌격대장",
            "마나의흐름",
            "마나효율증가",
            "바리케이드",
            "번개의분노",
            "부러진뼈",
            "분쇄의주먹",
            "불굴",
            "선수필승",
            "속전속결",
            "슈퍼차지",
            "승부사",
            "시선집중",
            "실드관통",
            "아드레날린",
            "안정된상태",
            "약자무시",
            "에테르포식자",
            "여신의가호",
            "예리한둔기",
            "원한",
            "위기모면",
            "저주받은인형",
            "전문의",
            "정기흡수",
            "정밀단도",
            "중갑착용",
            "질량증가",
            "최대마나증가",
            "추진력",
            "타격의대가",
            "탈출의명수",
            "폭팔물전문가"
        };
        private string[] reductionList = { "공격력감소", "공격속도감소", "방어력감소", "이동속도감소" };

        private Mat[] perImage = new Mat[6];
        private Mat[] enhance = new Mat[43];
        private Mat[] reduction = new Mat[4];
        private Mat abilityStoneTextImage = new Mat();
        private Mat SuccessTextImage = new Mat();

        public ResourceLoader()
        {
            abilityStoneTextImage = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.Ability_Stone_Text);
            SuccessTextImage = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.SuccessText);

            perImage[0] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources._75p);
            perImage[1] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources._65p);
            perImage[2] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources._55p);
            perImage[3] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources._45p);
            perImage[4] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources._35p);
            perImage[5] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources._25p);

            enhance[0] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.각성);
            enhance[1] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.강령술);
            enhance[2] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.강화방패);
            enhance[3] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.결투의대가);
            enhance[4] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.구슬동자);
            enhance[5] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.굳은의지);
            enhance[6] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.급소타격);
            enhance[7] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.기습의대가);
            enhance[8] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.긴급구조);
            enhance[9] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.달인의저력);
            enhance[10] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.돌격대장);
            enhance[11] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.마나의흐름);
            enhance[12] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.마나효율증가);
            enhance[13] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.바리케이드);
            enhance[14] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.번개의분노);
            enhance[15] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.부러진뼈);
            enhance[16] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.분쇄의주먹);
            enhance[17] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.불굴);
            enhance[18] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.선수필승);
            enhance[19] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.속전속결);
            enhance[20] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.슈퍼차지);
            enhance[21] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.승부사);
            enhance[22] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.시선집중);
            enhance[23] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.실드관통);
            enhance[24] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.아드레날린);
            enhance[25] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.안정된상태);
            enhance[26] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.약자무시);
            enhance[27] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.에테르포식자);
            enhance[28] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.여신의가호);
            enhance[29] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.예리한둔기);
            enhance[30] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.원한);
            enhance[31] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.위기모면);
            enhance[32] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.저주받은인형);
            enhance[33] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.전문의);
            enhance[34] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.정기흡수);
            enhance[35] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.정밀단도);
            enhance[36] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.중갑착용);
            enhance[37] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.질량증가);
            enhance[38] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.최대마나증가);
            enhance[39] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.추진력);
            enhance[40] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.타격의대가);
            enhance[41] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.탈출의명수);
            enhance[42] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.폭팔물전문가);

            reduction[0] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.공격력감소);
            reduction[1] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.공격속도감소);
            reduction[2] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.방어력감소);
            reduction[3] = OpenCvSharp.Extensions.BitmapConverter.ToMat(Resources.이동속도감소);
        }

        public Mat GetImageToName(string name)
        {
            int num = GetReductionImageCode(name);
            if (num == -1)
            {
                num = GetEnhanceImageCode(name);
                return GetEnhanceImage(num);
            }
            return GetReductionImage(num);

        }

        public int GetReductionImageCode(string name)
        {
            for (int i = 0; i < reductionList.Length; i++)
            {
                if (name == reductionList[i])
                {
                    return i;
                }
            }
            return -1;
        }

        public int GetEnhanceImageCode(string name)
        {
            for (int i = 0; i < enhanceList.Length; i++)
            {
                if (name == enhanceList[i])
                {
                    return i;
                }
            }
            return -1;
        }

        public Mat GetEnhanceImage(int num)
        {
            return enhance[num];
        }

        public Mat GetReductionImage(int num)
        {
            return reduction[num];
        }

        public Mat GetAbilityStoneText()
        {
            return abilityStoneTextImage;
        }

        public Mat GetSuccessTextImage()
        {
            return SuccessTextImage;
        }
        public Mat GetPercentageImage(int num)
        {
            return perImage[num];
        }

        public Mat GetPercentageGrayImage(int num)
        {
            Mat gray = new Mat();
            Cv2.CvtColor(perImage[num], gray, ColorConversionCodes.BGR2GRAY);
            return gray;
        }

        public string GetEnhanceName(int num)
        {
            return enhanceList[num];
        }

        public string GetReductionName(int num)
        {
            return reductionList[num];
        }
    }
}
