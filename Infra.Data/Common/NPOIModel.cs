using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Infrastructure.Common
{
    /// <summary>
    /// 实体类
    /// </summary>
    public class NPOIModel
    {
        /// <summary>
        /// 数据源
        /// </summary>
        public DataTable dataSource { get; private set; }
        /// <summary>
        /// 要导出的数据列数组
        /// </summary>
        public string[] fileds { get; private set; }
        /// <summary>
        /// 工作薄名称数组
        /// </summary>
        public string sheetName { get; private set; }
        /// <summary>
        /// 表标题
        /// </summary>
        public string tableTitle { get; private set; }
        /// <summary>
        /// 表标题是否存在 1：存在 0：不存在
        /// </summary>
        public int isTitle { get; private set; }
        /// <summary>
        /// 是否添加序号
        /// </summary>
        public int isOrderby { get; private set; }
        /// <summary>
        /// 表头
        /// </summary>
        public string headerName { get; private set; }
        /// <summary>
        /// 取得列宽
        /// </summary>
        public int[] colWidths { get; private set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="dataSource">数据来源 DataTable</param>
        /// <param name="filed">要导出的字段，如果为空或NULL，则默认全部</param> 
        /// <param name="sheetName">工作薄名称</param>
        /// <param name="headerName">表头名称 如果为空或NULL，则默认数据列字段
        /// 相邻父列头之间用'#'分隔,父列头与子列头用空格(' ')分隔,相邻子列头用逗号分隔(',')
        /// 两行：序号#分公司#组别#本日成功签约单数 预警,续约,流失,合计#累计成功签约单数 预警,续约,流失,合计#任务数#完成比例#排名 
        /// 三行：等级#级别#上期结存 件数,重量,比例#本期调入 收购调入 件数,重量,比例#本期发出 车间投料 件数,重量,比例#本期发出 产品外销百分比 件数,重量,比例#平均值 
        /// 三行时请注意：列头要重复
        /// </param>
        /// <param name="tableTitle">表标题</param> 
        /// <param name="isOrderby">是否添加序号 0：不添加 1：添加</param>
        public NPOIModel(DataTable dataSource, string filed, string sheetName, string headerName, string tableTitle = null, int isOrderby = 0)
        {
            if (!string.IsNullOrEmpty(filed))
            {
                this.fileds = filed.ToUpper().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                // 移除多余数据列
                for (int i = dataSource.Columns.Count - 1; i >= 0; i--)
                {
                    DataColumn dc = dataSource.Columns[i];
                    if (!this.fileds.Contains(dataSource.Columns[i].Caption.ToUpper()))
                    {
                        dataSource.Columns.Remove(dataSource.Columns[i]);
                    }
                }

                // 列索引
                int colIndex = 0;
                // 循环排序
                for (int i = 0; i < dataSource.Columns.Count; i++)
                {
                    // 获取索引
                    colIndex = GetColIndex(dataSource.Columns[i].Caption.ToUpper());
                    // 设置下标
                    dataSource.Columns[i].SetOrdinal(colIndex);
                }
            }
            else
            {
                this.fileds = new string[dataSource.Columns.Count];
                for (int i = 0; i < dataSource.Columns.Count; i++)
                {
                    this.fileds[i] = dataSource.Columns[i].ColumnName;
                }
            }
            this.dataSource = dataSource;

            if (!string.IsNullOrEmpty(sheetName))
            {
                this.sheetName = sheetName;
            }
            if (!string.IsNullOrEmpty(headerName))
            {
                this.headerName = headerName;
            }
            else
            {
                this.headerName = string.Join("#", this.fileds);
            }
            if (!string.IsNullOrEmpty(tableTitle))
            {
                this.tableTitle = tableTitle;
                this.isTitle = 1;
            }
            // 取得数据列宽 数据列宽可以和表头列宽比较，采取最长宽度  
            colWidths = new int[this.dataSource.Columns.Count];
            foreach (DataColumn item in this.dataSource.Columns)
            {
                colWidths[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName.ToString()).Length;
            }
            // 循环比较最大宽度
            for (int i = 0; i < this.dataSource.Rows.Count; i++)
            {
                for (int j = 0; j < this.dataSource.Columns.Count; j++)
                {
                    int intTemp = Encoding.GetEncoding(936).GetBytes(this.dataSource.Rows[i][j].ToString()).Length;
                    if (intTemp > colWidths[j])
                    {
                        colWidths[j] = intTemp;
                    }
                }
            }
            if (isOrderby > 0)
            {
                this.isOrderby = isOrderby;
                this.headerName = "序号#" + this.headerName;
            }
        }

        /// <summary>
        /// 获取列名下标
        /// </summary>
        /// <param name="colName">列名称</param>
        /// <returns></returns>
        private int GetColIndex(string colName)
        {
            for (int i = 0; i < this.fileds.Length; i++)
            {
                if (colName == this.fileds[i])
                    return i;
            }
            return 0;
        }
    }

    /// <summary>
    /// 表头构建类
    /// </summary>
    public class NPOIHeader
    {
        /// <summary>
        /// 表头
        /// </summary>
        public string headerName { get; set; }
        /// <summary>
        /// 起始行
        /// </summary>
        public int firstRow { get; set; }
        /// <summary>
        /// 结束行
        /// </summary>
        public int lastRow { get; set; }
        /// <summary>
        /// 起始列
        /// </summary>
        public int firstCol { get; set; }
        /// <summary>
        /// 结束列
        /// </summary>
        public int lastCol { get; set; }
        /// <summary>
        /// 是否跨行
        /// </summary>
        public int isRowSpan { get; private set; }
        /// <summary>
        /// 是否跨列
        /// </summary>
        public int isColSpan { get; private set; }
        /// <summary>
        /// 外加行
        /// </summary>
        public int rows { get; set; }

        public NPOIHeader() { }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="headerName">表头</param>
        /// <param name="firstRow">起始行</param>
        /// <param name="lastRow">结束行</param>
        /// <param name="firstCol">起始列</param>
        /// <param name="lastCol">结束列</param>
        /// <param name="rows">外加行</param>
        /// <param name="cols">外加列</param>
        public NPOIHeader(string headerName, int firstRow, int lastRow, int firstCol, int lastCol, int rows = 0)
        {
            this.headerName = headerName;
            this.firstRow = firstRow;
            this.lastRow = lastRow;
            this.firstCol = firstCol;
            this.lastCol = lastCol;
            // 是否跨行判断
            if (firstRow != lastRow)
                isRowSpan = 1;
            if (firstCol != lastCol)
                isColSpan = 1;

            this.rows = rows;
        }
    }
}
