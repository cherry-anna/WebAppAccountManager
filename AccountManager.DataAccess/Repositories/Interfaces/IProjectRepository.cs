using AccountManager.Domain.Interfaces;
using AccountManager.Domain.Models;

namespace AccountManager.DataAccess.Repositories.Interfaces
{
    public interface IProjectRepository: IRepository<Project, int>
    {
    }
}
