using GymClass.Core.Repositories;
using GymClass.Data.Data;

namespace GymClass.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public IGymClassRepository GymClassRepository { get; set; }
        private readonly ApplicationDbContext context;

        public UnitOfWork(ApplicationDbContext context, IGymClassRepository gymClassRepository)
        {
            this.context = context;
            GymClassRepository = gymClassRepository;
        }

        public async Task SaveCompleteAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
