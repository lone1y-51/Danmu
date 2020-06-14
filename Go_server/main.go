package main

import (
	"net/http"

	dm "danmu/dm"

	"github.com/gin-gonic/gin"
)

func main() {
	router := gin.Default()
	dmGroup := router.Group("dm")
	dm.DmRouterRegister(dmGroup)
	router.GET("/", WebRoot)
	router.Run()
}

func WebRoot(context *gin.Context) {
	context.JSON(http.StatusOK, gin.H{"msg": "hello world"})
}
