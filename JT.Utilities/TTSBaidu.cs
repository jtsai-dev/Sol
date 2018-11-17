using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace JT.Infrastructure
{
    public class TTSBaidu
    {
        #region params
        // reference: http://yuyin.baidu.com/docs/tts/136

        /// <summary>
        /// 语言选择,填写zh
        /// </summary>
        private const string Lan = "zh";
        /// <summary>
        /// 客户端类型选择，web端填写1
        /// </summary>
        public const int Ctp = 1;

        ///// <summary>
        ///// 合成的文本，使用UTF-8编码，请注意文本长度必须小于1024字节
        ///// </summary>
        //public string Tex { get; set; }

        private int spd = 5;
        /// <summary>
        /// 语速，取值0-9，默认为5中语速
        /// </summary>
        public int Spd
        {
            get { return spd; }
            set { spd = value >= 0 && value <= 9 ? value : spd; }
        }

        private int pit = 5;
        /// <summary>
        /// 音调，取值0-9，默认为5中语调
        /// </summary>
        public int Pit
        {
            get { return pit; }
            set { pit = value >= 0 && value <= 9 ? value : pit; }
        }

        private int vol = 5;
        /// <summary>
        /// 音量，取值0-9，默认为5中音量
        /// </summary>
        public int Vol
        {
            get { return vol; }
            set { vol = value >= 0 && value <= 9 ? value : vol; }
        }

        private int per = 0;
        /// <summary>
        /// 发音人选择，取值0-1, 0为女声，1为男声，默认为女声
        /// </summary>
        public int Per
        {
            get { return per; }
            set { per = value >= 0 && value <= 1 ? value : per; }
        }

        /// <summary>
        /// 用户唯一标识，用来区分用户，填写机器 MAC 地址或 IMEI 码，长度为60以内
        /// </summary>
        private string Cuid { get; set; }

        private string ApiKey;
        private string SecretKey;
        #endregion

        private readonly string url = "http://tsn.baidu.com/text2audio";
        private readonly string restFormat = "tex={0}&lan={1}&tok={2}&ctp={3}&cuid={4}&spd={5}&pit={6}&vol={7}&per={8}";
        private readonly string fileNameFormat = "{0}.mp3";

        private readonly string tokenUrlFormat = "https://openapi.baidu.com/oauth/2.0/token?grant_type=client_credentials&client_id={0}&client_secret={1}";
        private string TokenKey = "TTSBaidu_TOKEN";


        public TTSBaidu(string apiKey, string secretKey)
        {
            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(secretKey))
            {
                throw new ArgumentNullException("apiKey || string secretKey", "apiKey || string secretKey could not be null or empty");
            }

            this.ApiKey = apiKey;
            this.SecretKey = secretKey;
        }


        /// <summary>
        /// translate text to audio(mp3)
        /// </summary>
        /// <param name="text"></param>
        /// <param name="savePath"></param>
        /// <returns>saved fileName</returns>
        public string Text2Audio(string text, string savePath)
        {
            #region valid params
            string Tok = GetToken();
            if (string.IsNullOrEmpty(Tok)) throw new ArgumentNullException("Tok");
            if (string.IsNullOrEmpty(text)) throw new ArgumentNullException("text");
            #endregion

            //Cuid = Guid.NewGuid().ToString("N");
            string fileName = string.Format(fileNameFormat, DateTime.Now.Ticks);//Cuid);
            string fullPath = string.Format("{0}/{1}", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, savePath), fileName);

            byte[] responseData = new byte[] { };
            var texs = SplitByLen(text);
            foreach (var item in texs)
            {
                responseData.Concat(Text2AudioBytes(item, Tok));
            }

            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            File.WriteAllBytes(fullPath, responseData);
            return fileName;
        }


        private string GetToken()
        {
            var token = JT.Infrastructure.AppCommon.CacheHelper.Get(TokenKey);
            if (token != null)
            {
                return token.ToString();
            }

            string requestUrl = string.Format(tokenUrlFormat, this.ApiKey, this.SecretKey);
            WebClient webClient = new WebClient();

            try
            {
                var content = webClient.DownloadData(requestUrl);
                var jsonStr = System.Text.Encoding.UTF8.GetString(content);

                JavaScriptSerializer js = new JavaScriptSerializer();
                var result = js.Deserialize<Dictionary<string, string>>(jsonStr);
                var access_token = result["access_token"];

                JT.Infrastructure.AppCommon.CacheHelper.Add(TokenKey, access_token, new DateTimeOffset(DateTime.Now.AddDays(25)));

                return access_token;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<string> SplitByLen(string str, int perLength = 340)
        {
            var strs = new List<string>();
            if (string.IsNullOrEmpty(str) || str.Length <= perLength)
            {
                return strs;
            }

            var i = 0;
            while (i * perLength < str.Length)
            {
                var perStr = str.Skip(i * perLength).Take(perLength);
                strs.Add(string.Join("", perStr));
                i++;
            }

            return strs;
        }

        private byte[] Text2AudioBytes(string text, string tok)
        {
            string strUpdateData = string.Format(restFormat, text, Lan, tok, Ctp, Cuid, Spd, Pit, Vol, Per);
            try
            {
                WebClient webClient = new WebClient();
                webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                byte[] responseData = webClient.UploadData(url, "POST", Encoding.UTF8.GetBytes(strUpdateData));

                string srcString = Encoding.UTF8.GetString(responseData);
                if (srcString.IndexOf("err_no") > -1)
                    throw new Exception(srcString);

                return responseData;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
