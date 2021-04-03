using System;

namespace Gastromio.Persistence.MongoDB
{
    public class CuisineModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime UpdatedOn { get; set; }

        public Guid UpdatedBy { get; set; }
    }
}
