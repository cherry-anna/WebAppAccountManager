using System.Threading;
using System.Threading.Tasks;

namespace AccountManager.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken token = default);
    }
}
