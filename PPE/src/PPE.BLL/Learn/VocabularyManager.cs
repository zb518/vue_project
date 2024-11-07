using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PPE.DataModel;
using PPE.IDAL;
using PPE.Model.Shared;
using PPE.Utilities;

namespace PPE.BLL;
/// <summary>
/// 单词业务逻辑管理
/// </summary>
public class VocabularyManager : BaseManager<Base_Vocabulary, LearnDbContext>
{
    public VocabularyManager(IServiceProvider service, OperationErrorDescriber describer, IVocabularyRepository repository, ILogger<VocabularyManager> logger) : base(service, describer, repository, logger)
    {
        Store = repository;
    }
    public new IVocabularyRepository Store { get; set; }

    public override async Task<IdentityResult> ValidateAsync(Base_Vocabulary vocabulary)
    {
        var result = await base.ValidateAsync(vocabulary).ConfigureAwait(false);
        if (!result.Succeeded)
        {
            return result;
        }
        if (string.IsNullOrWhiteSpace(vocabulary.Word))
        {
            return IdentityResult.Failed(ErrorDescriber.InvalidError("词条"));
        }
        if (await Store.AnyAsync(x => !x.Id.Equals(vocabulary.Id) && x.NormalizedWord == vocabulary.NormalizedWord, CancellationToken).ConfigureAwait(false))
        {
            return IdentityResult.Failed(ErrorDescriber.DuplicateError($"词条 {vocabulary.Word}"));
        }
        return IdentityResult.Success;
    }

    /// <summary>
    /// 分页查询
    /// </summary>
    /// <param name="parameter">分页查询参数 <see cref=""/>DataTableParameter</param>
    /// <returns></returns>
    public async Task<string> FindPageAsync(DataTableParameter parameter)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(parameter);
        var result = await Store.FindPageAsync(parameter, CancellationToken).ConfigureAwait(false);
        return JsonHelper.ConvertToJson(result);
    }
}