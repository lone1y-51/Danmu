package dm

import "github.com/gin-gonic/gin"

func DmRouterRegister(g *gin.RouterGroup) {
	g.GET("allDM", AllDM)
}
