using System;
using Gastromio.Core.Domain.Model.Cuisines;

namespace Gastromio.Core.Application.DTOs
{
    public class CuisineDTO
    {
        public CuisineDTO(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        internal CuisineDTO(Cuisine cuisine)
        {
            Id = cuisine.Id.Value;
            Name = cuisine.Name;
        }

        public Guid Id { get; }

        public string Name { get; }
    }
}
