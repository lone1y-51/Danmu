package db

import (
	"database/sql"
	"log"

	//mysql used
	_ "github.com/go-sql-driver/mysql"
)

var db, err = sql.Open("mysql", "lone1y:123456@/danmu")

//InsertToDB is a function
func InsertToDB(content map[string]string) {
	defer func() {
		if err := recover(); err != nil {
			log.Println(err)
		}
	}()
	tx, e := db.Begin()
	if e != nil {
		log.Println(e)
		return
	}
	if content["bl"] == "0" {
		sqlFormat := "insert into danmu (id, uid, name, bl, bnn, level, text, cst) VALUES (NULL, ?, ?, NULL, NULL, ?, ?, ?);"
		db.Exec(sqlFormat, content["uid"], content["nn"], content["level"], content["txt"], content["cst"])
	} else {
		sqlFormat := "insert into danmu (id, uid, name, bl, bnn, level, text, cst) VALUES(NULL, ?, ?, ?, ?, ?, ?, ?);"
		db.Exec(sqlFormat, content["uid"], content["nn"], content["bl"], content["bnn"], content["level"], content["txt"], content["cst"])
	}
	tx.Commit()
}
