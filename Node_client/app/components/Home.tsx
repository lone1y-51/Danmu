import React, { useEffect, useState, useRef } from 'react';
import routes from '../constants/routes.json';
import styles from './Home.css';
import { Buffer } from 'buffer';
import SingleDM from './SingleDM';
import {Card} from 'antd';
import getRoomInfo from '../api/room';
import SingleGift from './SingleGift';

const wss = new WebSocket("wss://danmuproxy.douyu.com:8504/");
const roomID = "916749";
function Utf8ArrayToStr(array: Uint8Array) {
    var out, i, len, c;
    var char2, char3;
 
    out = "";
    len = array.length;
    i = 0;
    while(i < len) {
        c = array[i++];
        switch(c >> 4)
        { 
            case 0: case 1: case 2: case 3: case 4: case 5: case 6: case 7:
                // 0xxxxxxx
                out += String.fromCharCode(c);
                break;
            case 12: case 13:
                // 110x xxxx   10xx xxxx
                char2 = array[i++];
                out += String.fromCharCode(((c & 0x1F) << 6) | (char2 & 0x3F));
                break;
            case 14:
                // 1110 xxxx  10xx xxxx  10xx xxxx
                char2 = array[i++];
                char3 = array[i++];
                out += String.fromCharCode(((c & 0x0F) << 12) |
                            ((char2 & 0x3F) << 6) |
                            ((char3 & 0x3F) << 0));
                break;
        }
    }
    return out;
}

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
    let message = "type@=loginreq/roomid@="+roomID+"/";
    let buf = genDanmuMessage(message);
    return buf;
}
function joinGroupMessage(){
    let message = "type@=joingroup/rid@="+roomID+"/gid@=-9999/";
    let buf = genDanmuMessage(message);
    return buf;
}
function messageHandle(message: string){
    let msgArray = message.split('/');
    let result = new Map();
    msgArray.forEach(item => {
        let temp = item.split('@=');
        // result[temp[0]] = temp[1];
        result.set(temp[0], temp[1]);
    })
    return result;
}

export default function Home() {
    const [dm, setDm] = useState(Array<Map<any, any> >());
    const [giftInfo, setGiftInfo] = useState(Object());
    const [gift, setGift] = useState(Array<Map<any, any> >());
    wss.onmessage = (evt) => {
        evt.data.stream().getReader().read().then((val: any) => {
            let buf = val.value.slice(12, val.value.length-1);
            let dataText = Utf8ArrayToStr(buf);
            let dataMap = messageHandle(dataText);
            if(dataMap.get("type") === "loginres"){
                wss.send(joinGroupMessage());
                setInterval(function(){wss.send(genDanmuMessage("type@=mrkl/"))}, 45 *1000)
                return;
            }else if(dataMap.get("type") === "chatmsg"){
                // console.log(dm.length);
                dm.length >= 1000 ? dm.pop() : null;
                dm.push(dataMap);
                setDm([...dm]);
            }else if(dataMap.get("type") === "dgb"){
                gift.length >= 1000 ? gift.pop() : null;
                gift.push(dataMap);
                setGift([...gift]);
            }
        });
    }
    let dmBox = document.getElementById("danmuBox");
    let giftBox = document.getElementById("giftBox");
    useEffect(() => {
        wss.onopen = () => {
            console.log("connect to danmu server success");
            wss.send(loginMessage());
        }
        getRoomInfo(roomID).then(res => {
            if(res.status === 200){
                let temp = new Map();
                console.log(res.data.data.gift);
                res.data.data.gift.forEach((val: any) => {
                    temp.set(val.id, val.name) ;   
                });
                temp.set("824", "荧光棒");
                setGiftInfo(temp);
            }
        });
    }, []);
    useEffect(() => {
        if(dmBox){
            dmBox.scrollTop = dmBox.scrollHeight;
        }
        if(giftBox){
            giftBox.scrollTop = giftBox.scrollHeight;
        }
    }, [dm, gift]);
    return (
        <div style={{display: 'flex', justifyContent: 'space-around'}}>
            <div className={styles.danmuContainer} style={{flexBasis: '45%'}}>
                <div id="danmuBox">
                    {
                        dm.map((val: Map<any, any>, index: number) => {
                            return <SingleDM content={val} key={index} />
                        })
                    }
                </div>

            </div>
            <div className={styles.danmuContainer} style={{flexBasis: '45%'}}>
                <div id="giftBox">
                    {
                        gift.map((val: Map<any, any>, index: number) => {
                            return <SingleGift content={val} gift={giftInfo} key={index} />
                        })
                    }
                </div>
            </div>
        </div>
  );
}
