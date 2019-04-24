using System;
using System.Linq;
using System.Web;
using System.IO;
using NPOI;
using NPOI.SS.Util;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using System.Data;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Common
{
    /*
    导出Excel包含的功能： 
    1.多表头导出最多支持到三行，表头格式说明 
    相邻父列头之间用’#’分隔,父列头与子列头用空格(’ ‘)分隔,相邻子列头用逗号分隔(‘,’) 
    两行：序号#分公司#组别#本日成功签约单数 预警,续约,流失,合计#累计成功签约单数 预警,续约,流失,合计#任务数#完成比例#排名 
    三行：等级#级别#上期结存 件数,重量,比例#本期调入 收购调入 件数,重量,比例#本期发出 车间投料 件数,重量,比例#本期发出 产品外销百分比 件数,重量,比例#平均值 
    三行时请注意：列头要重复 
    2.添加表头标题功能 
    3.添加序号功能 
    4.根据数据设置列宽
    缺陷： 
    数据内容不能合并列合并行
    改进思路： 
    添加一属性：设置要合并的列，为了实现多列合并可以这样设置{“列1,列2”,”列4”}
        */
    /// <summary>
    /// 利用NPOI实现导出Excel
    /// </summary>
    public class ATNPOIHelper
    {

        #region 初始化

        /// <summary>
        /// 声明 HSSFWorkbook 对象
        /// </summary>
        private static HSSFWorkbook _workbook;

        /// <summary>
        /// 声明 HSSFSheet 对象
        /// </summary>
        private static HSSFSheet _sheet;

        #endregion

        #region Excel导出

        /// <summary>
        /// Excel导出
        /// </summary>
        /// <param name="fileName">文件名称 如果为空或NULL，则默认“新建Excel.xls”</param>
        /// <param name="list"></param>
        /// <param name="ColMergeNum">合计：末行合计时，合并的列数</param>
        /// <param name="method">导出方式 1：WEB导出（默认）2：按文件路径导出</param>
        /// <param name="filePath">文件路径 如果WEB导出，则可以为空；如果按文件路径导出，则默认桌面路径</param>
        public static void Export(string fileName, IList<NPOIModel> list, int ColMergeNum, int method = 1, string filePath = null)
        {
            // 文件名称
            if (!string.IsNullOrEmpty(fileName))
            {
                if (fileName.IndexOf('.') == -1)
                {
                    fileName += ".xls";
                }
                else
                {
                    fileName = fileName.Substring(1, fileName.IndexOf('.')) + ".xls";
                }
            }
            else
            {
                fileName = "新建Excel.xls";
            }
            // 文件路径
            if (2 == method && string.IsNullOrEmpty(filePath))
            {
                filePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }
            // 调用导出处理程序
            Export(list, ColMergeNum);
            // WEB导出
            if (1 == method)
            {
                //System.Web.HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
                ////设置下载的Excel文件名\
                //if (System.Web.HttpContext.Current.Request.ServerVariables["http_user_agent"].ToString().IndexOf("Firefox") != -1)
                //{
                //    //火狐浏览器      
                //    System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", "=?UTF-8?B?" + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(fileName)) + "?="));
                //}
                //else
                //{
                //    //IE等浏览器
                //    System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", System.Web.HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8)));
                //}
                //using (MemoryStream ms = new MemoryStream())
                //{
                //    //将工作簿的内容放到内存流中
                //    _workbook.Write(ms);
                //    //将内存流转换成字节数组发送到客户端
                //    System.Web.HttpContext.Current.Response.BinaryWrite(ms.GetBuffer());
                //    System.Web.HttpContext.Current.Response.End();
                //    _sheet = null;
                //    _workbook = null;
                //}
            }
            else if (2 == method)
            {
                using (FileStream fs = File.Open(filePath, FileMode.Append))
                {
                    _workbook.Write(fs);
                    _sheet = null;
                    _workbook = null;
                }
            }
        }

        /// <summary>
        /// 导出方法实现
        /// </summary>
        /// <param name="list"></param>
        private static void Export(IList<NPOIModel> list, int ColMergeNum)
        {

            #region 变量声明

            // 初始化
            _workbook = new HSSFWorkbook();
            // 声明 Row 对象
            IRow _row;
            // 声明 Cell 对象
            ICell _cell;
            // 总列数
            int cols = 0;
            // 总行数
            int rows = 0;
            // 行数计数器
            int rowIndex = 0;
            // 单元格值
            string drValue = null;

            #endregion

            foreach (NPOIModel model in list)
            {
                // 工作薄命名
                if (model.sheetName != null)
                    _sheet = (HSSFSheet)_workbook.CreateSheet(model.sheetName);
                else
                    _sheet = (HSSFSheet)_workbook.CreateSheet();

                // 获取数据源
                DataTable dt = model.dataSource;
                // 初始化
                rowIndex = 0;
                // 获取总行数
                rows = GetRowCount(model.headerName);
                // 获取总列数
                cols = GetColCount(model.headerName);

                //合计：合并表格末行N列,rows为表头行数，dt.Rows.Count为数据行数
                if (ColMergeNum > 1)
                {
                    CellRangeAddress region_Merge = new CellRangeAddress(rows + dt.Rows.Count, rows + dt.Rows.Count, 0, ColMergeNum - 1);
                    _sheet.AddMergedRegion(region_Merge);
                }

                ICellStyle myBodyStyle = bodyStyle;
                ICellStyle myTitleStyle = titleStyle;
                ICellStyle myDateStyle = dateStyle;
                ICellStyle myBodyRightStyle = bodyRightStyle;
                // 循环行数
                foreach (DataRow row in dt.Rows)
                {

                    #region 新建表，填充表头，填充列头，样式

                    if (rowIndex == 65535 || rowIndex == 0)
                    {
                        if (rowIndex != 0)
                            _sheet = (HSSFSheet)_workbook.CreateSheet();

                        // 构建行
                        for (int i = 0; i < rows + model.isTitle; i++)
                        {
                            _row = _sheet.GetRow(i);
                            // 创建行
                            if (_row == null)
                                _row = _sheet.CreateRow(i);

                            for (int j = 0; j < cols; j++)
                                _row.CreateCell(j).CellStyle = myBodyStyle;
                        }

                        // 如果存在表标题
                        if (model.isTitle > 0)
                        {
                            // 获取行
                            _row = _sheet.GetRow(0);
                            // 合并单元格
                            CellRangeAddress region = new CellRangeAddress(0, 0, 0, (cols - 1));
                            _sheet.AddMergedRegion(region);
                            // 填充值
                            _row.CreateCell(0).SetCellValue(model.tableTitle);
                            // 设置样式
                            _row.GetCell(0).CellStyle = myTitleStyle;
                            // 设置行高
                            _row.HeightInPoints = 20;
                        }

                        // 取得上一个实体
                        NPOIHeader lastRow = null;
                        IList<NPOIHeader> hList = GetHeaders(model.headerName, rows, model.isTitle);
                        // 创建表头
                        foreach (NPOIHeader m in hList)
                        {
                            var data = hList.Where(c => c.firstRow == m.firstRow && c.lastCol == m.firstCol - 1);
                            if (data.Count() > 0)
                            {
                                lastRow = data.First();
                                if (m.headerName == lastRow.headerName)
                                    m.firstCol = lastRow.firstCol;
                            }

                            // 获取行
                            _row = _sheet.GetRow(m.firstRow);
                            // 合并单元格
                            CellRangeAddress region = new CellRangeAddress(m.firstRow, m.lastRow, m.firstCol, m.lastCol);

                            _sheet.AddMergedRegion(region);
                            // 填充值
                            _row.CreateCell(m.firstCol).SetCellValue(m.headerName);
                        }
                        // 填充表头样式
                        for (int i = 0; i < rows + model.isTitle; i++)
                        {
                            _row = _sheet.GetRow(i);
                            for (int j = 0; j < cols; j++)
                            {
                                _row.GetCell(j).CellStyle = myBodyStyle;
                                //设置列宽
                                _sheet.SetColumnWidth(j, (model.colWidths[j] + 1) * 450);
                            }
                        }

                        rowIndex = (rows + model.isTitle);
                    }

                    #endregion

                    #region 填充内容

                    // 构建列
                    _row = _sheet.CreateRow(rowIndex);
                    foreach (DataColumn column in dt.Columns)
                    {
                        // 添加序号列
                        if (1 == model.isOrderby && column.Ordinal == 0)
                        {
                            _cell = _row.CreateCell(0);
                            _cell.SetCellValue(rowIndex - rows);
                            _cell.CellStyle = myBodyStyle;
                        }

                        // 创建列
                        _cell = _row.CreateCell(column.Ordinal + model.isOrderby);

                        // 获取值
                        drValue = row[column].ToString();

                        switch (column.DataType.ToString())
                        {
                            case "System.String"://字符串类型
                                _cell.SetCellValue(drValue);
                                _cell.CellStyle = myBodyStyle;
                                break;
                            case "System.DateTime"://日期类型
                                DateTime dateV;
                                DateTime.TryParse(drValue, out dateV);
                                _cell.SetCellValue(dateV);

                                _cell.CellStyle = myDateStyle;//格式化显示
                                break;
                            case "System.Boolean"://布尔型
                                bool boolV = false;
                                bool.TryParse(drValue, out boolV);
                                _cell.SetCellValue(boolV);
                                _cell.CellStyle = myBodyStyle;
                                break;
                            case "System.Int16"://整型
                            case "System.Int32":
                            case "System.Int64":
                            case "System.Byte":
                                int intV = 0;
                                int.TryParse(drValue, out intV);
                                _cell.SetCellValue(intV);
                                _cell.CellStyle = myBodyRightStyle;
                                break;
                            case "System.Decimal"://浮点型
                            case "System.Double":
                                double doubV = 0;
                                double.TryParse(drValue, out doubV);
                                _cell.SetCellValue(doubV.ToString("f2"));
                                _cell.CellStyle = myBodyRightStyle;
                                break;
                            case "System.DBNull"://空值处理
                                _cell.SetCellValue("");
                                break;
                            default:
                                _cell.SetCellValue("");
                                break;
                        }

                    }

                    #endregion

                    rowIndex++;
                }
            }
        }

        #region 辅助方法

        /// <summary>
        /// 表头解析
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="header">表头</param>
        /// <param name="rows">总行数</param>
        /// <param name="addRows">外加行</param>
        /// <param name="addCols">外加列</param>
        /// <returns></returns>
        private static IList<NPOIHeader> GetHeaders(string header, int rows, int addRows)
        {
            // 临时表头数组
            string[] tempHeader;
            string[] tempHeader2;
            // 所跨列数
            int colSpan = 0;
            // 所跨行数
            int rowSpan = 0;
            // 单元格对象
            NPOIHeader model = null;
            // 行数计数器
            int rowIndex = 0;
            // 列数计数器
            int colIndex = 0;
            // 
            IList<NPOIHeader> list = new List<NPOIHeader>();
            // 初步解析
            string[] headers = header.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
            // 表头遍历
            for (int i = 0; i < headers.Length; i++)
            {
                // 行数计数器清零
                rowIndex = 0;
                // 列数计数器清零
                colIndex = 0;
                // 获取所跨行数
                rowSpan = GetRowSpan(headers[i], rows);
                // 获取所跨列数
                colSpan = GetColSpan(headers[i]);

                // 如果所跨行数与总行数相等，则不考虑是否合并单元格问题
                if (rows == rowSpan)
                {
                    colIndex = GetMaxCol(list);
                    model = new NPOIHeader(headers[i],
                        addRows,
                        (rowSpan - 1 + addRows),
                        colIndex,
                        (colSpan - 1 + colIndex),
                        addRows);
                    list.Add(model);
                    rowIndex += (rowSpan - 1) + addRows;
                }
                else
                {
                    // 列索引
                    colIndex = GetMaxCol(list);
                    // 如果所跨行数不相等，则考虑是否包含多行
                    tempHeader = headers[i].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    for (int j = 0; j < tempHeader.Length; j++)
                    {

                        // 如果总行数=数组长度
                        if (1 == GetColSpan(tempHeader[j]))
                        {
                            if (j == tempHeader.Length - 1 && tempHeader.Length < rows)
                            {
                                model = new NPOIHeader(tempHeader[j],
                                    (j + addRows),
                                    (j + addRows) + (rows - tempHeader.Length),
                                    colIndex,
                                    (colIndex + colSpan - 1),
                                    addRows);
                                list.Add(model);
                            }
                            else
                            {
                                model = new NPOIHeader(tempHeader[j],
                                        (j + addRows),
                                        (j + addRows),
                                        colIndex,
                                        (colIndex + colSpan - 1),
                                        addRows);
                                list.Add(model);
                            }
                        }
                        else
                        {
                            // 如果所跨列数不相等，则考虑是否包含多列
                            tempHeader2 = tempHeader[j].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            for (int m = 0; m < tempHeader2.Length; m++)
                            {
                                // 列索引
                                colIndex = GetMaxCol(list) - colSpan + m;
                                if (j == tempHeader.Length - 1 && tempHeader.Length < rows)
                                {
                                    model = new NPOIHeader(tempHeader2[m],
                                        (j + addRows),
                                        (j + addRows) + (rows - tempHeader.Length),
                                        colIndex,
                                        colIndex,
                                        addRows);
                                    list.Add(model);
                                }
                                else
                                {
                                    model = new NPOIHeader(tempHeader2[m],
                                            (j + addRows),
                                            (j + addRows),
                                            colIndex,
                                            colIndex,
                                            addRows);
                                    list.Add(model);
                                }
                            }
                        }
                        rowIndex += j + addRows;
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 获取最大列
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private static int GetMaxCol(IList<NPOIHeader> list)
        {
            int maxCol = 0;
            if (list.Count > 0)
            {
                foreach (NPOIHeader model in list)
                {
                    if (maxCol < model.lastCol)
                        maxCol = model.lastCol;
                }
                maxCol += 1;
            }

            return maxCol;
        }

        /// <summary>
        /// 获取表头行数
        /// </summary>
        /// <param name="newHeaders">表头文字</param>
        /// <returns></returns>
        private static int GetRowCount(string newHeaders)
        {
            string[] ColumnNames = newHeaders.Split(new char[] { '@' });
            int Count = 0;
            if (ColumnNames.Length <= 1)
                ColumnNames = newHeaders.Split(new char[] { '#' });
            foreach (string name in ColumnNames)
            {
                int TempCount = name.Split(new char[] { ' ' }).Length;
                if (TempCount > Count)
                    Count = TempCount;
            }
            return Count;
        }

        /// <summary>
        /// 获取表头列数
        /// </summary>
        /// <param name="newHeaders">表头文字</param>
        /// <returns></returns>
        private static int GetColCount(string newHeaders)
        {
            string[] ColumnNames = newHeaders.Split(new char[] { '@' });
            int Count = 0;
            if (ColumnNames.Length <= 1)
                ColumnNames = newHeaders.Split(new char[] { '#' });
            Count = ColumnNames.Length;
            foreach (string name in ColumnNames)
            {
                int TempCount = name.Split(new char[] { ',' }).Length;
                if (TempCount > 1)
                    Count += TempCount - 1;
            }
            return Count;
        }

        /// <summary>
        /// 列头跨列数
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="newHeaders">表头文字</param>
        /// <returns></returns>
        private static int GetColSpan(string newHeaders)
        {
            return newHeaders.Split(',').Count();
        }

        /// <summary>
        /// 列头跨行数
        /// </summary> 
        /// <remarks>
        /// </remarks>
        /// <param name="newHeaders">列头文本</param>
        /// <param name="rows">表头总行数</param>
        /// <returns></returns>
        private static int GetRowSpan(string newHeaders, int rows)
        {
            int Count = newHeaders.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).Length;
            // 如果总行数与当前表头所拥有行数相等
            if (rows == Count)
                Count = 1;
            else if (Count < rows)
                Count = 1 + (rows - Count);
            else
                throw new Exception("表头格式不正确！");
            return Count;
        }

        #endregion

        #region 单元格样式

        /// <summary>
        /// 数据单元格样式
        /// </summary>
        private static ICellStyle bodyStyle
        {
            get
            {
                ICellStyle style = _workbook.CreateCellStyle();
                style.Alignment = HorizontalAlignment.Center; //居中
                style.VerticalAlignment = VerticalAlignment.Center;//垂直居中 
                style.WrapText = true;//自动换行
                // 边框
                style.BorderBottom = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Thin;
                // 字体
                //IFont font = _workbook.CreateFont();
                //font.FontHeightInPoints = 10;
                //font.FontName = "宋体";
                //style.SetFont(font);

                return style;
            }
        }

        /// <summary>
        /// 数据单元格样式
        /// </summary>
        private static ICellStyle bodyRightStyle
        {
            get
            {
                ICellStyle style = _workbook.CreateCellStyle();
                style.Alignment = HorizontalAlignment.Right; //居中
                style.VerticalAlignment = VerticalAlignment.Center;//垂直居中 
                style.WrapText = true;//自动换行
                // 边框
                style.BorderBottom = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Thin;
                // 字体
                //IFont font = _workbook.CreateFont();
                //font.FontHeightInPoints = 10;
                //font.FontName = "宋体";
                //style.SetFont(font);

                return style;
            }
        }

        /// <summary>
        /// 标题单元格样式
        /// </summary>
        private static ICellStyle titleStyle
        {
            get
            {
                ICellStyle style = _workbook.CreateCellStyle();
                style.Alignment = HorizontalAlignment.Center; //居中
                style.VerticalAlignment = VerticalAlignment.Center;//垂直居中 
                style.WrapText = true;//自动换行 

                //IFont font = _workbook.CreateFont();
                //font.FontHeightInPoints = 14;
                //font.FontName = "宋体";
                //font.Boldweight = (short)FontBoldWeight.BOLD;
                //style.SetFont(font);

                return style;
            }
        }

        /// <summary>
        /// 日期单元格样式
        /// </summary>
        private static ICellStyle dateStyle
        {
            get
            {
                ICellStyle style = _workbook.CreateCellStyle();
                style.Alignment = HorizontalAlignment.Center; //居中
                style.VerticalAlignment = VerticalAlignment.Center;//垂直居中 
                style.WrapText = true;//自动换行
                // 边框
                style.BorderBottom = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Thin;
                // 字体
                //IFont font = _workbook.CreateFont();
                //font.FontHeightInPoints = 10;
                //font.FontName = "宋体";
                //style.SetFont(font);

                IDataFormat format = _workbook.CreateDataFormat();
                style.DataFormat = format.GetFormat("yyyy-MM-dd");
                return style;
            }
        }

        #endregion

        #endregion
    }

   


}