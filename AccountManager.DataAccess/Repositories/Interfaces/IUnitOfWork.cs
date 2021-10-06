using System.Threading;
using System.Threading.Tasks;

namespace AccountManager.DataAccess.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken token = default);
    }
}
