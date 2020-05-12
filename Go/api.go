package main

import (
	"encoding/json"
	"io/ioutil"
	"log"
	"net/http"
)

type roomGift struct {
	ID   string  `json:"id"`
	Name string  `json:"name"`
	Pc   float32 `json:"pc"`
}

type roomInfo struct {
	RoomID string     `json:"room_id"`
	Gift   []roomGift `json:"gift"`
}

type roomRes struct {
	Data roomInfo `json:"data"`
}

func getGiftDic() map[string]string {
	url := "http://open.douyucdn.cn/api/RoomApi/room/" + config.RoomID
	var res roomRes
	result := make(map[string]string)
	resp, err := http.Get(url)
	if err != nil {
		log.Println("query gift info error: ", err)
		return nil
	}
	defer resp.Body.Close()
	body, err := ioutil.ReadAll(resp.Body)
	err = json.Unmarshal(body, &res)
	if err != nil {
		log.Println("result json parse err: ", err)
		return nil
	}
	for _, gift := range res.Data.Gift {
		result[gift.ID] = gift.Name
	}
	result["824"] = "荧光棒"
	return result
}
