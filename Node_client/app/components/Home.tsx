import React, { useEffect } from 'react';
import routes from '../constants/routes.json';
import styles from './Home.css';
import { Buffer } from 'buffer';

function genDanmuMessage(message: string) {
	console.log("send message: ", message)
	let dataLen = message.length + 9
  let byte = new ArrayBuffer(dataLen + 4);
  let data = new DataView(byte);
  let messageBuf = new Buffer(message);
  data.setUint32(0, dataLen, true);
  data.setUint32(4, dataLen, true);
  data.setUint8(8, 0xb1);
  data.setUint8(9, 0x02);
  data.setUint8(10, 0x00);
  data.setUint8(11, 0x00);
  messageBuf.forEach((ele: number, index: number) => {
    data.setUint8(12+index, ele);
  })
  data.setUint8(dataLen + 4 -1, 0x00);
	return data.buffer;
}

function loginMessage(){
  let message = "type@=loginreq/roomid@=623499/";
  let buf = genDanmuMessage(message);
  return buf;
}
function joinGroupMessage(){
  let message = "type@=joingroup/rid@=623499/gid@=-9999/";
  let buf = genDanmuMessage(message);
  return buf;
}

export default function Home() {
  const wss = new WebSocket("wss://danmuproxy.douyu.com:8504/");
  wss.onmessage = (evt) => {
    evt.data.text().then((val: string) => {
      console.log(val);
      let temp = val.split("type@=");
      if(temp.length < 2){
        return;
      }
      let messageType = temp[1].split('/')[0];
      if(messageType == 'loginres'){
        console.log("login resp");
        wss.send(joinGroupMessage());
      }
      if(messageType == 'chatmsg'){
        console.log("danmu data");
      }
    });
  }
  useEffect(() => {
    console.log("into effect");
    wss.onopen = () => {
      console.log("connect to danmu server success");
      wss.send(loginMessage());
    }
  });
  return (
    <div className={styles.container} data-tid="container">
      <h2>Home</h2>
      {/* <Link to={routes.COUNTER}>to Counter</Link> */}
    </div>
  );
}
