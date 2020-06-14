package dm

import (
	"fmt"
	"net/http"

	"github.com/gin-gonic/gin"
)

func AllDM(c *gin.Context){
	fmt.Println("current func: AllDM")
	c.JSON(http.StatusOK, gin.H{"data": "hello world"})
}