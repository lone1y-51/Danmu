import axios from 'axios';

export default function getRoomInfo(roomId: string) {
    return axios.get("http://open.douyucdn.cn/api/RoomApi/room/"+roomId)
    // .then(res => {
    //     return res.data;
    // });
}