namespace GymClass.BusinessLogic.Repositories;

public interface IUnitOfWork
{
    IGymClassRepository GymClassRepository { get; set; }
    Task SaveCompleteAsync();
}