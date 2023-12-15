namespace GymClass.Core.Repositories;

public interface IUnitOfWork
{
    IGymClassRepository GymClassRepository { get; set; }
    Task SaveCompleteAsync();
}