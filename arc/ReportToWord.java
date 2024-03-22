package com.spring.pay;

import com.alibaba.fastjson.JSONObject;
import org.apache.poi.xwpf.usermodel.XWPFDocument;
import org.apache.poi.xwpf.usermodel.XWPFParagraph;
import org.apache.poi.xwpf.usermodel.XWPFRun;
import org.apache.poi.xwpf.usermodel.XWPFTable;

import java.io.FileOutputStream;
import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.text.SimpleDateFormat;
import java.util.*;

public class ReportToWord {

//    private final String DRIVER = "com.mysql.jdbc.Driver";
    private final String DRIVER = "com.mysql.cj.jdbc.Driver";
//    private final String URL = "jdbc:mysql://10.18.51.148:3306/wy_item?useUnicode=true&characterEncoding=UTF-8&useSSL=false&allowMultiQueries=true&serverTimezone=GMT%2B8";
//    private final String USER_NAME = "root";
//    private final String PASS_WORD = "Farben2019@";
//    private final String database = "wy_item";

//    private final String URL = "jdbc:mysql://118.126.91.251:3306/scenic_sys?serverTimezone=GMT%2b8&useUnicode=true&characterEncoding=UTF-8&useSSL=false&zeroDateTimeBehavior=convertToNull&autoReconnect=true&failOverReadOnly=false";
//    private final String USER_NAME = "root";
//    private final String PASS_WORD = "Db@dbtest@2018";
//    private final String database = "scenic_sys";

//    private final String URL = "jdbc:mysql://192.168.3.41:3306/mid_trade_settlement_dev?useUnicode=true&characterEncoding=UTF-8&useSSL=false&serverTimezone=GMT%2B8";
//    private final String USER_NAME = "root";
//    private final String PASS_WORD = "Farben2019@";
//    private final String database = "mid_trade_settlement_dev";
//
//    private final String URL = "jdbc:mysql://192.168.3.41:3306/mid_trade_merchant_dev?useUnicode=true&characterEncoding=UTF-8&useSSL=false&serverTimezone=GMT%2B8&zeroDateTimeBehavior=convertToNull";
//    private final String USER_NAME = "root";
//    private final String PASS_WORD = "Farben2019@";
//    private final String database = "mid_trade_merchant_dev";
//
//    private final String URL = "jdbc:mysql://192.168.3.41:3306/mid_trade_order_dev?allowMultiQueries=true&useUnicode=true&characterEncoding=UTF-8&useSSL=false&serverTimezone=GMT%2B8";
//    private final String USER_NAME = "root";
//    private final String PASS_WORD = "Farben2019@";
//    private final String database = "mid_trade_order_dev";

//    private final String URL = "jdbc:mysql://192.168.3.41:3306/mid_trade_payment_dev?useUnicode=true&characterEncoding=UTF-8&useSSL=false&serverTimezone=GMT%2B8&zeroDateTimeBehavior=convertToNull";
//    private final String USER_NAME = "root";
//    private final String PASS_WORD = "Farben2019@";
//    private final String database = "mid_trade_payment_dev";

    private final String URL="jdbc:mysql://rm-wz9gsao21946zjyw1qo.mysql.rds.aliyuncs.com:3306/pds?zeroDateTimeBehavior=convertToNull&characterEncoding=UTF-8&allowMultiQueries=true";
    private final String USER_NAME="ywuser";
    private final String PASS_WORD="xsYwUsert2o19";
    private final String database="pds";

    private final String reportPath = "C:";

    // 启动方法
    public static void main(String[] args) {

        try {
            ReportToWord rd = new ReportToWord();
            rd.report();
        }catch (Exception e){
            System.out.println("异常：自行处理或者联系我都阔以.");
            e.printStackTrace();
        }

    }


    Connection conn = null;
    PreparedStatement pst = null;
    ResultSet rs = null;

    // 获取查询数据
    public Map<String,List<TableColumn>> getData() throws Exception{

        System.out.println("数据生成中，请稍等...");
        Map<String,List<TableColumn>> map = new HashMap<>();

        List<Table> tables = getTables(database);

        for (Table table : tables) {
            List<TableColumn> columns = getColumns(database,table.getTableName());
            map.put(table.getTableName(),columns);
        }

        return map;

    }


    // 获取表字段信息
    public List<TableColumn>  getColumns(String database,String tableName) throws Exception{

        String sql = "select column_name,column_comment,column_type,is_nullable, column_key from information_schema.columns  where  table_schema=? and table_name=?  group by column_name";
        ResultSet rs = getConn(database,tableName,sql);

        List<TableColumn> tableColumns = new ArrayList<>();

        while (rs.next()){

            TableColumn tc = new TableColumn();
            tc.setTableName(tableName);
            tc.setColumnName(rs.getString("column_name"));
            tc.setColumnType(rs.getString("column_type"));
            tc.setColumnKey(rs.getString("column_key"));
            tc.setIsNullable(rs.getString("is_nullable"));
            tc.setColumnComment(rs.getString("column_comment"));
            tableColumns.add(tc);

        }

        releaseConn();

        return tableColumns;

    }


    // 获取所有表
    public List<Table> getTables(String database) throws Exception{

        String  sql = "select table_name,table_comment from information_schema.tables where table_schema=?";
        ResultSet rs = getConn(database, "",sql);

        List<Table> tables = new ArrayList<>();
        while(rs.next()){
            Table table = new Table();
            table.setTableName(rs.getString( "table_name"));
            table.setTableCommont(rs.getString("table_comment"));
            tables.add(table);
        }

        releaseConn();
        return  tables;

    }

    // 连接数据库
    private ResultSet getConn(String dataBase,String tableName,String sql){

        try{

            Class.forName(DRIVER);
            conn = DriverManager.getConnection(URL,USER_NAME,PASS_WORD);
            pst = conn.prepareStatement(sql);
            pst.setString(1,dataBase);
            if(!"".equals(tableName)){
                pst.setString(2,tableName);
            }
            rs = pst.executeQuery();
            return  rs;

        }catch (Exception e){
            e.printStackTrace();
        }

        return null;

    }

    // 释放连接
    private void  releaseConn(){
        try{
            if(rs != null ){
                rs.close();
            }
            if(pst != null){
                pst.close();
            }
            if(conn != null){
                conn.close();
            }
        }catch (Exception e){
            e.printStackTrace();
        }
    }


    // 导出数据
    public void report()  throws  Exception{

        Map<String, List<TableColumn>> data = this.getData();       // 表名：表体
        List<Table> tables = this.getTables(this.database);         // 表体(列名、类型、注释)
        Map<String,String> tableMap = new HashMap<>();              // 表名:中文名

        JSONObject json = new JSONObject((HashMap)data);

        for (Table table : tables) {
            tableMap.put(table.getTableName(),table.getTableCommont());
        }

        // 构建表格数据
        XWPFDocument document = new XWPFDocument();

        Integer i = 1;
        for (String tableName : data.keySet()) {

            XWPFParagraph paragraph = document.createParagraph();                // 创建标题对象
            XWPFRun run = paragraph.createRun();                                 // 创建文本对象
//            run.setText((i+"、"+tableName+"    "+tableMap.get(tableName)));      // 标题名称
            run.setText(("表"+tableName+"    "+tableMap.get(tableName)));      // 标题名称
            run.setFontSize(14);                                                 // 字体大小
            run.setBold(true);                                                   // 字体加粗

            int j = 0;
            XWPFTable table = document.createTable(data.get(tableName).size()+1,5);
            // 第一行
            table.setCellMargins(10,50,10,200);
            table.getRow(j).getCell(0).setText("字段名称");
            table.getRow(j).getCell(1).setText("字段类型");
            table.getRow(j).getCell(2).setText("约束");
            table.getRow(j).getCell(3).setText("为空");
            table.getRow(j).getCell(4).setText("字段含义");
            j++;

            for (TableColumn tableColumn : data.get(tableName)) {

                table.getRow(j).getCell(0).setText(tableColumn.getColumnName());
                table.getRow(j).getCell(1).setText(tableColumn.getColumnType());
                table.getRow(j).getCell(2).setText(tableColumn.getColumnKey());
                table.getRow(j).getCell(3).setText(tableColumn.getIsNullable());
                table.getRow(j).getCell(4).setText(tableColumn.getColumnComment());
                j++;

            }
            i++;
        }

        // 文档输出
        FileOutputStream out = new FileOutputStream(reportPath + new SimpleDateFormat("yyyyMMddHHmmss").format(new Date()).toString()+"_"+database +".docx");
        document.write(out);
        out.close();
        System.out.println("Word生成完成!!!");

    }

    // 表
   class Table{

       private String tableName;

       private String tableCommont;

       public String getTableName() {
           return tableName;
       }

       public void setTableName(String tableName) {
           this.tableName = tableName;
       }

       public String getTableCommont() {
           return tableCommont;
       }

       public void setTableCommont(String tableCommont) {
           this.tableCommont = tableCommont;
       }

   }

   // 表列信息
   class TableColumn{
       // 表名
       private String tableName;
       // 字段名
       private String columnName;
       // 字段类型
       private String columnType;
       // 字段注释
       private String columnComment;
       // 可否为空
       private String isNullable;
       // 约束
       private String columnKey;

       public String getTableName() {
           return tableName;
       }

       public void setTableName(String tableName) {
           this.tableName = tableName;
       }

       public String getColumnName() {
           return columnName;
       }

       public void setColumnName(String columnName) {
           this.columnName = columnName;
       }

       public String getColumnType() {
           return columnType;
       }

       public void setColumnType(String columnType) {
           this.columnType = columnType;
       }

       public String getColumnComment() {
           return columnComment;
       }

       public void setColumnComment(String columnComment) {
           this.columnComment = columnComment;
       }

       public String getIsNullable() {
           return isNullable;
       }

       public void setIsNullable(String isNullable) {
           this.isNullable = isNullable;
       }

       public String getColumnKey() {
           return columnKey;
       }

       public void setColumnKey(String columnKey) {
           this.columnKey = columnKey;
       }

   }


}


