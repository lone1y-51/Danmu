package main

import (
	"bytes"
	"encoding/binary"
	"fmt"
	"log"
	"net/url"
	"strings"
	"time"

	"github.com/gorilla/websocket"
)

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
	log.Println("gen bytes: ", data)
	return data
}

func commonSend(c *websocket.Conn, message string) {
	err := c.WriteMessage(websocket.TextMessage, genDanmuMessage(message))
	if err != nil {
		log.Println(err)
	}
}

func loginServer(c *websocket.Conn) {
	message := "type@=loginreq/roomid@=156277/"
	commonSend(c, message)
	// err := c.WriteMessage(websocket.TextMessage, genDanmuMessage(message))
	// if err != nil {
	// 	return
	// }
	readMessage(c)
}

func joinGroup(c *websocket.Conn) {
	message := "type@=joingroup/rid@=156277/gid@=-9999/"
	commonSend(c, message)
	// err := c.WriteMessage(websocket.TextMessage, genDanmuMessage(message))
	// if err != nil {
	// 	return
	// }
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
		if message["bl"] != "0" {
			outText += fmt.Sprintf("[%s %s] ", message["bl"], message["bnn"])
		}
		outText += fmt.Sprintf("Lv %s %s: %s", message["level"], message["nn"], message["txt"])
		log.Println(outText)
	}
}

func readMessage(c *websocket.Conn) {
	_, res, err := c.ReadMessage()
	if err != nil {
		log.Println("read err", err)
		return
	}
	start := 0
	for {
		lenBytes := res[start : start+4]
		dataLen := int(binary.LittleEndian.Uint32(lenBytes))
		textData := string(res[start+4*3 : start+4+dataLen])
		handleResult := messageHandle(textData)
		printText(handleResult)
		start = start + 4 + dataLen
		if start >= len(res) {
			break
		}
	}
	// log.Println(string(res))
}

func main() {
	log.SetFlags(0)
	u := url.URL{Scheme: "wss", Host: "danmuproxy.douyu.com:8504", Path: "/"}
	log.Println("connect to ", u.String())
	c, _, err := websocket.DefaultDialer.Dial(u.String(), nil)
	if err != nil {
		log.Fatalln("dial:", err)
	}
	defer c.Close()
	loginServer(c)
	joinGroup(c)
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
