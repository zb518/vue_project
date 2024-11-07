namespace PPE.Model.Shared
{
    /// <summary>
    /// js datatables 结果
    /// </summary>
    public class DataTableResult
    {
        public DataTableResult()
        {
        }
        public DataTableResult(int draw, long total, long filter, object? data)
        {
            this.draw = draw;
            recordsTotal = total;
            recordsFiltered = filter;
            this.data = data;
        }
        public DataTableResult(string error)
        {
            this.error = error;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public int draw { get; set; }
        /// <summary>
        /// 总记录数量
        /// </summary>
        /// <value></value>
        public long recordsTotal { get; set; }
        /// <summary>
        /// 条件筛选后记录数量
        /// </summary>
        /// <value></value>
        public long recordsFiltered { get; set; }
        /// <summary>
        /// 数据对象
        /// </summary>
        /// <value></value>
        public virtual object? data { get; set; }
        /// <summary>
        /// 错误
        /// </summary>
        /// <value></value>
        public string? error { get; set; }
    }

    /// <summary>
    /// js datatables 结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataTableResult<T> where T : new()
    {
        public DataTableResult()
        {
        }

        public DataTableResult(int draw, long total, int filter, IReadOnlyList<T>? data)
        {
            this.draw = draw;
            recordsTotal = total;
            recordsFiltered = filter;
            this.data = data;
        }
        public DataTableResult(string error)
        {
            this.error = error;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public int draw { get; set; }
        /// <summary>
        /// 总记录数量
        /// </summary>
        /// <value></value>
        public long recordsTotal { get; set; }
        /// <summary>
        /// 条件筛选后记录数量
        /// </summary>
        /// <value></value>
        public long recordsFiltered { get; set; }
        /// <summary>
        /// 数据集
        /// </summary>
        /// <value></value>
        public IReadOnlyList<T>? data { get; set; }
        /// <summary>
        /// 错误
        /// </summary>
        /// <value></value>
        public string? error { get; set; }
    }
}