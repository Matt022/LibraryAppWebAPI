using LibraryAppWebAPI.Base;

namespace LibraryAppWebAPI.Repository.Interfaces;

public interface IRepository<T> where T : EntityBase
{
    IEnumerable<T> GetAll();

    T Create(T entity);

    T Delete(int id);

    void Update(T entity);

    T GetById(int id);
}
