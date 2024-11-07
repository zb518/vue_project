using AutoMapper;

namespace PPE.Core;

public class MapperHelper
{

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <typeparam name="D"></typeparam>
    /// <typeparam name="S"></typeparam>
    /// <returns></returns>
    public static D Mapper<D, S>(S source)
    {
        var config = new MapperConfiguration(cfg => cfg.CreateMap<S, D>());
        return config.CreateMapper().Map<D>(source);
    }
}