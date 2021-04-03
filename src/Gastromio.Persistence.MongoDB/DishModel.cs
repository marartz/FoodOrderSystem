using System;
using System.Collections.Generic;

namespace Gastromio.Persistence.MongoDB
{
    public class DishModel
    {
        public Guid Id { get; set; }

        public Guid RestaurantId { get; set; }

        public Guid CategoryId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ProductInfo { get; set; }

        public int OrderNo { get; set; }

        public IList<DishVariantModel> Variants { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime UpdatedOn { get; set; }

        public Guid UpdatedBy { get; set; }
    }
}
