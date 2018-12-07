using System;
using System.Text;

namespace GSMediator.StressTest.Generator
{
    public class RandomStringGenerator
    {
        private static readonly string[] CODE_DICT =
        {"A","B","C","D","E","F","G","H","J",
            "K","M","N","P","Q","R","S","T","U",
            "V","W","X","Y","Z",
            "a","b","c","d","e","f","g","h","j",
            "k","m","n","p","q","r","s","t","u",
            "v","w","x","y","z",
            "2","3","4","5","6","7","8","9" };

        private static readonly string[] NAME_CODE_DICT =
        {"冰","火","水","綠","藍","澄","橙","紅","紫",
            "黑","白","月","玥","凌","鈴","翎","零","綾",
            "蒼","碧","空","樂","遙",
            "泉","伶","貓","櫻","雪","霜","雨","羽","夏",
            "秋","冬","春","冷","凜","芙","緋","夜","華",
            "花","風","楓","律","幻",
            "夢","闇","音","茵","千","葉","草","銀" };
        private Random _random = new Random();

        /// <summary>
        /// 產生隨機的帳號或密碼
        /// </summary>
        /// <returns></returns>
        public string RandomAccountOrPassword()
        {
            return RandomString(8, CODE_DICT);
        }

        /// <summary>
        /// 產生隨機的角色名稱
        /// </summary>
        /// <returns></returns>
        public string RandomCreateCharacterName()
        {
            return RandomString(8, NAME_CODE_DICT);
        }

        /// <summary>
        /// Randoms the string.
        /// </summary>
        /// <param name="stringLength">Length of the string.</param>
        /// <param name="codeDict">The code dictionary.</param>
        /// <returns></returns>
        private string RandomString(int stringLength, string[] codeDict)
        {
            StringBuilder randomCharacterName = new StringBuilder();
            for (int i = 0; i < stringLength; i++)
            {
                // 使用亂數隨機取得字典中的值
                int randomNum = _random.Next(0, codeDict.Length);
                randomCharacterName.Append(codeDict[randomNum]);
            }
            return randomCharacterName.ToString();
        }
    }
}
