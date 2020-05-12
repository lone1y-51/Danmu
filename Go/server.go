package main

import (
	"bytes"
	"encoding/binary"
	"fmt"
	"io/ioutil"
	"log"
	"strings"
	"time"

	"danmu/api"
	"danmu/db"

	"github.com/gorilla/websocket"
	"gopkg.in/yaml.v2"
)

var giftDic map[string]string

//Conf config struct
type Conf struct {
	RoomID string `yaml:"roomId"`
	URL    string `yaml:"url"`
}

var config Conf

func loadConf() {
	yamlFile, err := ioutil.ReadFile("conf.yaml")
	if err != nil {
		log.Println("load file error: ", err)
		return
	}
	err = yaml.Unmarshal(yamlFile, &config)
	if err != nil {
		log.Println("parse conf error: ", err)
		return
	}
	log.Println("load config info: ", config)
}

func genDanmuMessage(message string) []byte {
	log.Println("send message: ", message)
	dataLen := len(message) + 9
	lenByte := make([]byte, 4)
	binary.LittleEndian.PutUint32(lenByte, uint32(dataLen))
	headerBytes := []byte{0xb1, 0x02, 0x00, 0x00}
	var buffer bytes.Buffer
	buffer.Write(lenByte)
	buffer.Write(lenByte)
	buffer.Write(headerBytes)
	buffer.Write([]byte(message))
	buffer.WriteByte(0x00)
	data := buffer.Bytes()
	return data
}

func commonSend(c *websocket.Conn, message string) {
	err := c.WriteMessage(websocket.TextMessage, genDanmuMessage(message))
	if err != nil {
		log.Println(err)
	}
}

func loginServer(c *websocket.Conn) {
	message := "type@=loginreq/roomid@=" + config.RoomID + "/"
	commonSend(c, message)
	readMessage(c)
}

func joinGroup(c *websocket.Conn) {
	message := "type@=joingroup/rid@=" + config.RoomID + "/gid@=-9999/"
	commonSend(c, message)
	readMessage(c)
}

func keepAlive(c *websocket.Conn) {
	message := "type@=mrkl/"
	commonSend(c, message)
}

func messageHandle(text string) map[string]string {
	contentList := strings.Split(text, "/")
	contentDic := make(map[string]string)
	for _, content := range contentList {
		tmp := strings.Split(content, "@=")
		if len(tmp) < 2 {
			continue
		}
		contentDic[tmp[0]] = tmp[len(tmp)-1]
	}
	return contentDic
}

func printText(message map[string]string) {
	outText := ""
	if _, ok := message["type"]; !ok {
		return
	}
	if message["type"] == "chatmsg" {
		if rg, ok := message["rg"]; ok && rg != "1" {
			outText += "[房] "
		}
		if message["bl"] != "0" {
			outText += fmt.Sprintf("[%s %s] ", message["bl"], message["bnn"])
		}
		outText += fmt.Sprintf("Lv %s %s: %s", message["level"], message["nn"], message["txt"])
		log.Println(outText)
		db.InsertToDB(message)
	}
	if message["type"] == "dgb" {
		if _, ok := giftDic[message["gfid"]]; ok {
			outText += fmt.Sprintf("Lv %s %s: 赠送给主播 %sX%s %s连击",
				message["level"], message["nn"], giftDic[message["gfid"]], message["gfcnt"], message["hits"])
			log.Println(outText)
		}
	}
}

func readMessage(c *websocket.Conn) {
	_, res, err := c.ReadMessage()
	if err != nil {
		log.Println("read err", err)
		if restartSocket(c) < 0 {
			return
		}
	}
	start := 0
	for {
		lenBytes := res[start : start+4]
		dataLen := int(binary.LittleEndian.Uint32(lenBytes))
		if start+4+dataLen-1 >= len(res) {
			break
		}
		textData := string(res[start+4*3 : start+4+dataLen])
		handleResult := messageHandle(textData)
		printText(handleResult)
		start = start + 4 + dataLen
		if start >= len(res) {
			break
		}
	}
}

func createSocket() *websocket.Conn {
	c, _, err := websocket.DefaultDialer.Dial(config.URL, nil)
	if err != nil {
		log.Fatalln("dial:", err)
		return nil
	}
	return c
}

func startSocket(c *websocket.Conn) {
	loginServer(c)
	joinGroup(c)
}

func restartSocket(c *websocket.Conn) int {
	c.Close()
	c = createSocket()
	if c == nil {
		log.Println("socket restart err")
		return -1
	}
	startSocket(c)
	return 0
}

func main() {
	loadConf()
	giftDic = api.GetGiftDic(config.RoomID)
	log.Println(giftDic)
	// log.SetFlags(0)
	log.Println("connect to ", config.URL)
	c := createSocket()
	if c == nil {
		log.Println("create socket err")
		return
	}
	defer c.Close()
	startSocket(c)
	t := time.NewTicker(time.Duration(45 * time.Second))
	for {
		select {
		case <-t.C:
			keepAlive(c)
		default:
			readMessage(c)
		}
	}
}
