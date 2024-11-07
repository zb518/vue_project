using PPE.DataModel;
using PPE.Model.Shared;
using PPE.ModelDto;

namespace PPE.IDAL;
/// <summary>
/// 单词数据访问操作
/// </summary>
public interface IVocabularyRepository : IBaseRepository<Base_Vocabulary, LearnDbContext>
{

    /// <summary>
    /// 分页查询
    /// </summary>
    /// <param name="parameter">分页查询参数 <see cref=""/>DataTableParameter</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<DataTableResult<VocabularyDetailDto>> FindPageAsync(DataTableParameter parameter, CancellationToken cancellationToken);
}