using WitchHutSearch.Searcher;

namespace WitchHutSearch.Writers;

public interface IHutWriter : IDisposable
{
    Task BeginAsync();
    
    Task<bool> WriteAsync(HutCentre centre);

    Task CompleteAsync(int count);
}