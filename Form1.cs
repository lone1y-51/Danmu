using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebSocketSharp;

namespace Danmu
{
    public partial class Form1 : Form
    {
        private delegate void messageDelegate(string text);
        private messageDelegate displayResult;
        private static byte[] headerByte = { 0xb1, 0x02, 0x00, 0x00 };
        private static byte[] endByte = { 0x00 };
        private static string keyValueSplitString = "@=";
        private static string contentSplitString = "/";
        private static string roomId = "916749";
        private WebSocket ws = null;
        public Form1()
        {
            InitializeComponent();
            this.displayResult += new messageDelegate(messageUIHandle);
            ws = new WebSocket("wss://danmuproxy.douyu.com:8504/");
            ws.OnMessage += this.messageRecv;
            ws.OnOpen += this.socketOpen;
            ws.OnError += this.socketError;
            ws.Connect();
            ws.Send(this.genDataByte("type@=loginreq/roomid@="+roomId+"/"));
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
            string text = "";
            if (!result.ContainsKey("type"))
            {
                return;
            }
            if (result["type"] == "loginres")
            {
                ws.Send(genDataByte("type@=joingroup/rid@="+roomId+"/gid@=-9999/"));
                return;
            }
            if (result["type"] == "chatmsg")
            {
                if(result["bl"] != "0")
                {
                    text += string.Format("[{0} {1}] ", result["bl"], result["bnn"]);
                }
                text += string.Format("Lv {0} {1}: {2}", result["level"], result["nn"], result["txt"]);
                this.Invoke(displayResult, text);
            }
        }
        private void messageUIHandle(string text)
        {
            this.dmTB.Text += text + "\r\n";
        }

        private void dmTB_TextChanged(object sender, EventArgs e)
        {
            this.dmTB.SelectionStart = this.dmTB.Text.Length;
            this.dmTB.ScrollToCaret();
        }
    }
}
