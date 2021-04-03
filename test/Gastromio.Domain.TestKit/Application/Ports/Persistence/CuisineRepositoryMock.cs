using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Application.Ports.Persistence;
using Gastromio.Core.Domain.Model.Cuisines;
using Moq;
using Moq.Language.Flow;

namespace Gastromio.Domain.TestKit.Application.Ports.Persistence
{
    public class CuisineRepositoryMock : Mock<ICuisineRepository>
    {
        public CuisineRepositoryMock(MockBehavior behavior) : base(behavior)
        {
        }

        public ISetup<ICuisineRepository, Task<IEnumerable<Cuisine>>> SetupFindAllAsync()
        {
            return Setup(m => m.FindAllAsync(It.IsAny<CancellationToken>()));
        }

        public ISetup<ICuisineRepository, Task<Cuisine>> SetupFindByNameAsync(string name)
        {
            return Setup(m => m.FindByNameAsync(name, It.IsAny<CancellationToken>()));
        }

        public ISetup<ICuisineRepository, Task<Cuisine>> SetupFindByCuisineIdAsync(CuisineId cuisineId)
        {
            return Setup(m => m.FindByCuisineIdAsync(cuisineId, It.IsAny<CancellationToken>()));
        }

        public ISetup<ICuisineRepository, Task> SetupStoreAsync(Cuisine cuisine)
        {
            return Setup(m => m.StoreAsync(cuisine, It.IsAny<CancellationToken>()));
        }

        public void VerifyStoreAsync(Cuisine cuisine, Func<Times> times)
        {
            Verify(m => m.StoreAsync(cuisine, It.IsAny<CancellationToken>()), times);
        }

        public ISetup<ICuisineRepository, Task> SetupRemoveAsync(CuisineId cuisineId)
        {
            return Setup(m => m.RemoveAsync(cuisineId, It.IsAny<CancellationToken>()));
        }

        public void VerifyRemoveAsync(CuisineId cuisineId, Func<Times> times)
        {
            Verify(m => m.RemoveAsync(cuisineId, It.IsAny<CancellationToken>()), times);
        }
    }
}
