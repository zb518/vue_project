using PPE.Core;
using PPE.DataModel;
using PPE.IDAL;
using PPE.Model.Shared;
using PPE.ModelDto;
using System.Net;

namespace PPE.DAL;
/// <summary>
/// 单词数据访问操作
/// </summary>
public class VocabularyRepository : BaseRepository<Base_Vocabulary, LearnDbContext>, IVocabularyRepository
{
    public VocabularyRepository(LearnDbContext context, OperationErrorDescriber describer, IdentityFactory identityFactory, IOperationLogRepository logRepository) : base(context, describer, identityFactory, logRepository)
    {
    }



    /// <summary>
    /// 分页查询
    /// </summary>
    /// <param name="parameter">分页查询参数 <see cref=""/>DataTableParameter</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<DataTableResult<VocabularyDetailDto>> FindPageAsync(DataTableParameter parameter, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(parameter);
        var query = from word in Entities
                    select new VocabularyDetailDto
                    {
                        Id = word.Id,
                        Word = word.Word,
                        NormalizedWord = word.NormalizedWord,
                        PhoneticSymbol = word.PhoneticSymbol,
                        IsDeleted = word.IsDeleted,
                        Description = word.Description,
                        CreateUserName = word.CreateUserName,
                        CreateRealName = word.CreateRealName,
                        CreateDate = word.CreateDate,
                        UpdateUserName = word.UpdateUserName,
                        UpdateRealName = word.UpdateRealName,
                        UpdateDate = word.UpdateDate,
                        Expressions = word.Expressions != null ? WebUtility.HtmlDecode(word.Expressions) : null,
                    };
        return ExpressionExtensions.FindPageAsync(query, parameter, null, cancellationToken);
    }
}