using OpenCvSharp;
using LostarkLogProject.Properties;
using OpenCvSharp.Extensions;

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

        //어빌리티스톤
        private Mat[] abilityPercentageImage = new Mat[6];
        private Mat[] enhance = new Mat[43];
        private Mat[] reduction = new Mat[4];
        private Mat abilityStoneTextImage = new Mat();
        private Mat SuccessTextImage = new Mat();

        //트라이포드
        private Mat[] tripodPercentageImage = new Mat[7];
        private Mat tripodTextImage = new Mat();
        private Mat tripodSuccessImage = new Mat();
        private Mat tripodFailImage = new Mat();

        public ResourceLoader()
        {


            #region 어빌리티스톤 확률
            abilityStoneTextImage = Resources.Ability_Stone_Text.ToMat();
            SuccessTextImage = Resources.SuccessText.ToMat();

            
            abilityPercentageImage[0] = Resources._75p.ToMat();
            abilityPercentageImage[1] = Resources._65p.ToMat();
            abilityPercentageImage[2] = Resources._55p.ToMat();
            abilityPercentageImage[3] = Resources._45p.ToMat();
            abilityPercentageImage[4] = Resources._35p.ToMat();
            abilityPercentageImage[5] = Resources._25p.ToMat();
            #endregion

            #region 각인 이미지
            enhance[0] = Resources.각성.ToMat();
            enhance[1] = Resources.강령술.ToMat();
            enhance[2] = Resources.강화방패.ToMat();
            enhance[3] = Resources.결투의대가.ToMat();
            enhance[4] = Resources.구슬동자.ToMat();
            enhance[5] = Resources.굳은의지.ToMat();
            enhance[6] = Resources.급소타격.ToMat();
            enhance[7] = Resources.기습의대가.ToMat();
            enhance[8] = Resources.긴급구조.ToMat();
            enhance[9] = Resources.달인의저력.ToMat();
            enhance[10] = Resources.돌격대장.ToMat();
            enhance[11] = Resources.마나의흐름.ToMat();
            enhance[12] = Resources.마나효율증가.ToMat();
            enhance[13] = Resources.바리케이드.ToMat();
            enhance[14] = Resources.번개의분노.ToMat();
            enhance[15] = Resources.부러진뼈.ToMat();
            enhance[16] = Resources.분쇄의주먹.ToMat();
            enhance[17] = Resources.불굴.ToMat();
            enhance[18] = Resources.선수필승.ToMat();
            enhance[19] = Resources.속전속결.ToMat();
            enhance[20] = Resources.슈퍼차지.ToMat();
            enhance[21] = Resources.승부사.ToMat();
            enhance[22] = Resources.시선집중.ToMat();
            enhance[23] = Resources.실드관통.ToMat();
            enhance[24] = Resources.아드레날린.ToMat();
            enhance[25] = Resources.안정된상태.ToMat();
            enhance[26] = Resources.약자무시.ToMat();
            enhance[27] = Resources.에테르포식자.ToMat();
            enhance[28] = Resources.여신의가호.ToMat();
            enhance[29] = Resources.예리한둔기.ToMat();
            enhance[30] = Resources.원한.ToMat();
            enhance[31] = Resources.위기모면.ToMat();
            enhance[32] = Resources.저주받은인형.ToMat();
            enhance[33] = Resources.전문의.ToMat();
            enhance[34] = Resources.정기흡수.ToMat();
            enhance[35] = Resources.정밀단도.ToMat();
            enhance[36] = Resources.중갑착용.ToMat();
            enhance[37] = Resources.질량증가.ToMat();
            enhance[38] = Resources.최대마나증가.ToMat();
            enhance[39] = Resources.추진력.ToMat();
            enhance[40] = Resources.타격의대가.ToMat();
            enhance[41] = Resources.탈출의명수.ToMat();
            enhance[42] = Resources.폭팔물전문가.ToMat();

            reduction[0] = Resources.공격력감소.ToMat();
            reduction[1] = Resources.공격속도감소.ToMat();
            reduction[2] = Resources.방어력감소.ToMat();
            reduction[3] = Resources.이동속도감소.ToMat();
            #endregion

            #region 트라이포드 확률이미지
            tripodTextImage = Resources.Tripod_Text.ToMat();
            tripodSuccessImage = Resources.Tripod_success.ToMat();
            tripodFailImage = Resources.Tripod_fail.ToMat();
            
            tripodPercentageImage[0] = Resources.Tripod_5_.ToMat();
            tripodPercentageImage[1] = Resources.Tripod_up10_.ToMat();
            tripodPercentageImage[2] = Resources.Tripod_15_.ToMat();
            tripodPercentageImage[3] = Resources.Tripod_up30_.ToMat();
            tripodPercentageImage[4] = Resources.Tripod_30_.ToMat();
            tripodPercentageImage[5] = Resources.Tripod_up60_.ToMat();
            tripodPercentageImage[6] = Resources.Tripod_100_.ToMat();
            #endregion
        }

        public Mat GetTripodPercentageImage(int num)
        {
            return tripodPercentageImage[num];
        }

        public Mat GetTripodSuccessImage()
        {
            return tripodSuccessImage;
        }

        public Mat GetTripodFailImage()
        {
            return tripodFailImage;
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

        public Mat GetTripodTextImage()
        {
            return tripodTextImage;
        }

        public Mat GetPercentageImage(int num)
        {
            return abilityPercentageImage[num];
        }

        public Mat GetPercentageGrayImage(int num)
        {
            Mat gray = new Mat();
            Cv2.CvtColor(abilityPercentageImage[num], gray, ColorConversionCodes.BGR2GRAY);
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
