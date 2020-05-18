using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightCenter
{
    /// <summary>
    /// This interface defines what every DAO class in this project should have.
    /// All data access classes inherit from it (except the test DAO).
    /// </summary>
    public interface IBasicDB<T> where T : IPoco
    {
        T Get(int id);
        IList<T> GetAll();
        void Add(T t);
        void AddRange(IList<T> list, bool testMode);
        void Remove(int id);
        void Update(T t);
    }
}
