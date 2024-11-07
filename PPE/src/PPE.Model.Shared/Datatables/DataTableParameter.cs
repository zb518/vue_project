using System.ComponentModel.DataAnnotations;

namespace PPE.Model.Shared
{
    /// <summary>
    /// js datatables 查询参数
    /// </summary>
    public class DataTableParameter
    {
        public DataTableParameter()
        {
            Columns = new List<DataTableColumn>();
        }

        public virtual int Draw { get; set; }

        /// <summary>
        /// 记录开始位置
        /// </summary>
        /// <value></value>
        public virtual int Start { get; set; }

        /// <summary>
        /// 每页记录长度
        /// </summary>
        /// <value></value>
        public virtual int Length { get; set; } = 10;

        /// <summary>
        /// 列信息
        /// </summary>
        /// <value></value>
        public virtual List<DataTableColumn> Columns { get; set; } = default!;

        /// <summary>
        /// 排序信息
        /// </summary>
        /// <value></value>
        public virtual List<DataTableOrder>? Order { get; set; }

        /// <summary>
        /// 搜索内容
        /// </summary>
        /// <value></value>
        public virtual DataTableSearch? Search { get; set; }

        /// <summary>
        /// 获取单个排序
        /// </summary>
        /// <value></value>
        public virtual string OrderBy
        {
            get { return Columns?.Count > 0 && Order?.Count > 0 ? Columns[Order[0].Column].Data ?? string.Empty : string.Empty; }
        }

        /// <summary>
        /// 排序方式
        /// </summary>
        /// <value></value>
        public virtual OrderDirection OrderDirection
        {
            get { return Order?.Count > 0 ? Order[0].Dir : OrderDirection.Asc; }
        }
    }

    /// <summary>
    /// js datatables 列参数
    /// </summary>
    public class DataTableColumn
    {
        /// <summary>
        /// 列序号
        /// </summary>
        /// <value></value>
        public virtual int Target { get; set; }

        /// <summary>
        /// 列名称
        /// </summary>
        /// <value></value>
        public virtual string? Name { get; set; }

        /// <summary>
        /// 列数据名称
        /// </summary>
        /// <value></value>
        public virtual string? Data { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        /// <value></value>
        public virtual string? Title { get; set; }

        /// <summary>
        /// 是否可搜索
        /// </summary>
        /// <value></value>
        public virtual bool Searchable { get; set; }

        /// <summary>
        /// 是否可排序
        /// </summary>
        /// <value></value>
        public virtual bool Orderable { get; set; }

        /// <summary>
        /// 搜索内容
        /// </summary>
        /// <value></value>
        public virtual DataTableSearch? Search { get; set; }
    }

    /// <summary>
    /// js datatables 搜索内容
    /// </summary>
    public class DataTableSearch
    {
        /// <summary>
        /// 搜索内容
        /// </summary>
        /// <value></value>
        public virtual string? Value { get; set; }

        /// <summary>
        /// 是否为正则式
        /// </summary>
        /// <value></value>
        public bool Regex { get; set; }
    }

    /// <summary>
    /// js datatables 排序
    /// </summary>
    public class DataTableOrder
    {

        /// <summary>
        /// 列序号，从0开始
        /// </summary>
        /// <value></value>
        public virtual int Column { get; set; }

        /// <summary>
        /// 排序方式，升序或降序
        /// </summary>
        /// <value></value>
        public OrderDirection Dir { get; set; }
    }

    /// <summary>
    /// 排序方式，Asc - 升序，Desc -  降序
    /// </summary>
    public enum OrderDirection
    {
        /// <summary>
        /// 升序
        /// </summary>
        [Display(Name = "升序")]
        Asc,
        /// <summary>
        /// 降序
        /// </summary>
        [Display(Name = "降序")]
        Desc
    }
}