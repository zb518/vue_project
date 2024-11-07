namespace PPE.ModelDto;

public class RequestDto : RequestDto<string>
{

}

public class RequestDto<TKey> where TKey : IEquatable<TKey>
{
    public virtual TKey Id { get; set; } = default!;
}