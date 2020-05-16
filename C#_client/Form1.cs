using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebSocketSharp;
using System.Timers;

namespace Danmu
{
    public partial class Form1 : Form
    {
        private delegate void messageDelegate(string text, int type);
        private messageDelegate displayResult;
        private static byte[] headerByte = { 0xb1, 0x02, 0x00, 0x00 };
        private static byte[] endByte = { 0x00 };
        private static string keyValueSplitString = "@=";
        private static string contentSplitString = "/";
        private static string roomId = "916749";
        private WebSocket ws = null;
        private Dictionary<string, string> nobleDic = new Dictionary<string, string>();
        private string roomInfoUrlPre = "http://open.douyucdn.cn/api/RoomApi/room/";
        private Dictionary<string, string> giftDic = new Dictionary<string, string>();
        private System.Timers.Timer keepTimer = new System.Timers.Timer(45 * 1000);
        public Form1()
        {
            InitializeComponent();
            getGiftInfo();
            this.displayResult += new messageDelegate(messageUIHandle);
            ws = new WebSocket("wss://danmuproxy.douyu.com:8504/");
            ws.OnMessage += this.messageRecv;
            ws.OnOpen += this.socketOpen;
            ws.OnError += this.socketError;
            ws.Connect();
            ws.Send(this.genDataByte("type@=loginreq/roomid@="+roomId+"/"));

            nobleDic.Add("1", "骑");
            nobleDic.Add("2", "子");
            nobleDic.Add("3", "伯");
            nobleDic.Add("4", "公");
            nobleDic.Add("5", "国");
            nobleDic.Add("6", "皇");
            nobleDic.Add("7", "游");

            keepTimer.Elapsed += new System.Timers.ElapsedEventHandler(keepTimerTick);
            keepTimer.AutoReset =  true;
        }

        private void keepTimerTick(Object sender, EventArgs args)
        {
            ws.Send(genDataByte("type@=mrkl/"));
        }

        private void getGiftInfo()
        {
            string url = roomInfoUrlPre + roomId;
            // HttpWebRequests request = HttpWebRequest.Create(url) as HttpWebRequest;
            HttpClient cli = new HttpClient();
            HttpResponseMessage response =  cli.GetAsync(url).Result;
            string result = response.Content.ReadAsStringAsync().Result;
            JObject jo = JObject.Parse(result);
            JArray giftJO = (JArray)jo["data"]["gift"];
            foreach(var gift in giftJO)
            {
                giftDic.Add(gift["id"].ToString(), gift["name"].ToString());
            }
            giftDic.Add("824", "荧光棒");
        }

        private void messageRecv(Object sender, MessageEventArgs e)
        {
            // string result = Encoding.UTF8.GetString(e.RawData);
            int start = 0;
            while (true)
            {
                byte[] lenByte = e.RawData.SubArray(start, 4);
                int len = (int)BitConverter.ToUInt32(lenByte, 0);
                string result = Encoding.UTF8.GetString(e.RawData.SubArray(start + 4 * 3, len - 4 * 2));
                string[] contentList = result.Split(contentSplitString.ToCharArray());
                Dictionary<string, string> contentDic = this.genKeyMap(contentList);
                this.messageHandle(contentDic);
                start = start + len + 4;
                if (start >= e.RawData.Length)
                {
                    break;
                }
            }
        }

        private Dictionary<string, string> genKeyMap(string[] contentList)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (string content in contentList)
            {
                if (!content.Contains(keyValueSplitString))
                {
                    continue;
                }
                string[] tmp = content.Split(keyValueSplitString.ToCharArray());
                if (result.ContainsKey(tmp[0]))
                {
                    result.Remove(tmp[0]);
                }
                result.Add(tmp[0], tmp[tmp.Length - 1]);
            }
            return result;
        }

        private void socketOpen(Object sender, EventArgs e)
        {
            Console.WriteLine("socket is open!");
        }

        private void socketError(Object sender, EventArgs e)
        {
            Console.WriteLine("socket error");
        }

        private byte[] genDataByte(string data)
        {
            UInt32 dataLen = (UInt32)(data.Length + 9);
            byte[] messageByte = Encoding.UTF8.GetBytes(data);
            byte[] lenByte = BitConverter.GetBytes(dataLen);
            dataLen = BitConverter.ToUInt32(lenByte, 0);
            lenByte = BitConverter.GetBytes(dataLen);
            byte[] result = lenByte.Concat(lenByte).Concat(headerByte).Concat(messageByte).Concat(endByte).ToArray();
            return result;
        }

        private void messageHandle(Dictionary<string, string> result)
        {
            int type = 0;
            string text = "";
            if (!result.ContainsKey("type"))
            {
                return;
            }
            if (result["type"] == "loginres")
            {
                ws.Send(genDataByte("type@=joingroup/rid@="+roomId+"/gid@=-9999/"));
                keepTimer.Enabled = true;
                return;
            }
            else if (result["type"] == "chatmsg")
            {
                if(result.ContainsKey("rg") && result["rg"] == "4")
                {
                    type = 1;
                    text += string.Format("[房]");
                }
                if(result.ContainsKey("nl") && result["nl"] == "0")
                {
                    type = 1;
                    text += string.Format("[{0}]", nobleDic[result["nl"]]);
                }
                if(result["bl"] != "0")
                {
                    text += string.Format("[{0} {1}] ", result["bl"], result["bnn"]);
                }
                text += string.Format("Lv {0} {1}: {2}", result["level"], result["nn"], result["txt"]);
            }
            else if(result["type"] == "dgb")
            {
                if (giftDic.ContainsKey(result["gfid"]))
                {
                    type = 2;
                    text += string.Format("Lv {0} {1}: 赠送给主播 {2}X{3} {4}连击",
                        result["level"], result["nn"], giftDic[result["gfid"]], result["gfcnt"], result["hits"]);
                }
                else
                {
                    return;
                }
            }
            else if(result["type"] == "uenter")
            {
                text += string.Format("欢迎Lv{0} {1} 进入直播间", result["level"], result["nn"]);
            }
            if (string.IsNullOrEmpty(text))
                return;
            this.Invoke(displayResult, text, type);
        }
        private void messageUIHandle(string text, int type)
        {
            if(type == 2)
            {
                this.giftTB.Text += text + "\r\n";
            }
            else
            {
                this.normalDMTB.Text += text + "\r\n";
                if(type == 1)
                {
                    this.highDMTB.Text += text + "\r\n";
                }
            }
        }

        private void highDMTB_TextChanged(object sender, EventArgs e)
        {
            this.highDMTB.SelectionStart = this.highDMTB.Text.Length;
            this.highDMTB.ScrollToCaret();
        }

        private void normalDMTB_TextChanged(object sender, EventArgs e)
        {
            this.normalDMTB.SelectionStart = this.normalDMTB.Text.Length;
            this.normalDMTB.ScrollToCaret();
        }

        private void giftTB_TextChanged(object sender, EventArgs e)
        {
            this.giftTB.SelectionStart = this.giftTB.Text.Length;
            this.giftTB.ScrollToCaret();
        }
    }
}
