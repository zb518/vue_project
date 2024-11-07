// using System.ComponentModel.DataAnnotations;
// using System.ComponentModel.DataAnnotations.Schema;

// namespace DataEntity;

// /// <summary>
// /// 课程目录
// /// </summary>
// [Table("Base_CourseCatalogue")]
// public class Base_CourseCatalogue : BaseDataEntity
// {

//     /// <summary>
//     /// 上级目录
//     /// </summary>
//     /// <value></value>
//     [StringLength(36)]
//     public virtual string ParentId { get; set; } = default!;

//     /// <summary>
//     /// 课程目录
//     /// </summary>
//     /// <value></value>
//     [StringLength(36)]
//     public virtual string CourseId { get; set; } = default!;

//     /// <summary>
//     /// 
//     /// </summary>
//     /// <value></value>
//     [StringLength(128)]
//     public virtual string? Title { get; set; }

//     /// <summary>
//     /// 标题索引值
//     /// </summary>
//     /// <value></value>
//     [StringLength(128)]
//     public virtual string? NormalizedTitle { get; set; }

//     /// <summary>
//     /// 排序
//     /// </summary>
//     /// <value></value>
//     [StringLength(20)]
//     public virtual string? SortCode { get; set; }

//     /// <summary>
//     /// 资源地址
//     /// </summary>
//     /// <value></value>
//     [StringLength(255)]
//     public virtual string? SourceUrl { get; set; }

//     /// <summary>
//     /// 资源地址标准值
//     /// </summary>
//     /// <value></value>
//     [StringLength(255)]
//     public virtual string? NormalizedSourceUrl { get; set; }

//     public override string ToString()
//     {
//         return Title ?? string.Empty;
//     }
// }