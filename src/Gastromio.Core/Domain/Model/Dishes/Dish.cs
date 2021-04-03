using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.DishCategories;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;

namespace Gastromio.Core.Domain.Model.Dishes
{
    public class Dish
    {
        private IList<DishVariant> variants;

        public Dish(
            DishId id,
            RestaurantId restaurantId,
            DishCategoryId categoryId,
            string name,
            string description,
            string productInfo,
            int orderNo,
            IList<DishVariant> variants,
            DateTimeOffset createdOn,
            UserId createdBy,
            DateTimeOffset updatedOn,
            UserId updatedBy
        )
        {
            Id = id;
            RestaurantId = restaurantId;
            CategoryId = categoryId;
            Name = name;
            Description = description;
            ProductInfo = productInfo;
            OrderNo = orderNo;
            CreatedOn = createdOn;
            CreatedBy = createdBy;
            UpdatedOn = updatedOn;
            UpdatedBy = updatedBy;
            this.variants = variants;
        }

        public DishId Id { get; }

        public RestaurantId RestaurantId { get; }

        public DishCategoryId CategoryId { get; }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public string ProductInfo { get; private set; }

        public int OrderNo { get; private set; }

        public IReadOnlyList<DishVariant> Variants => new ReadOnlyCollection<DishVariant>(variants);

        public DateTimeOffset CreatedOn { get; }

        public UserId CreatedBy { get; }

        public DateTimeOffset UpdatedOn { get; private set; }

        public UserId UpdatedBy { get; private set; }

        public Result<bool> ChangeName(string name, UserId changedBy)
        {
            if (string.IsNullOrEmpty(name))
                return FailureResult<bool>.Create(FailureResultCode.DishNameRequired);
            if (name.Length > 40)
                return FailureResult<bool>.Create(FailureResultCode.DishNameTooLong, 40);

            Name = name;
            UpdatedOn = DateTimeOffset.UtcNow;
            UpdatedBy = changedBy;

            return SuccessResult<bool>.Create(true);
        }

        public Result<bool> ChangeDescription(string description, UserId changedBy)
        {
            if (description != null && description.Length > 200)
                return FailureResult<bool>.Create(FailureResultCode.DishDescriptionTooLong,  200);

            Description = description;
            UpdatedOn = DateTimeOffset.UtcNow;
            UpdatedBy = changedBy;

            return SuccessResult<bool>.Create(true);
        }

        public Result<bool> ChangeProductInfo(string productInfo, UserId changedBy)
        {
            if (productInfo != null && productInfo.Length > 200)
                return FailureResult<bool>.Create(FailureResultCode.DishProductInfoTooLong,  200);

            ProductInfo = productInfo;
            UpdatedOn = DateTimeOffset.UtcNow;
            UpdatedBy = changedBy;

            return SuccessResult<bool>.Create(true);
        }

        public Result<bool> ChangeOrderNo(int orderNo, UserId changedBy)
        {
            if (orderNo < 0)
                return FailureResult<bool>.Create(FailureResultCode.DishInvalidOrderNo);

            OrderNo = orderNo;
            UpdatedOn = DateTimeOffset.UtcNow;
            UpdatedBy = changedBy;

            return SuccessResult<bool>.Create(true);
        }

        public Result<bool> AddVariant(Guid variantId, string name, decimal price, UserId changedBy)
        {
            var result = AddVariant(variants, variantId, name, price);
            if (result.IsFailure)
                return result;

            UpdatedOn = DateTimeOffset.UtcNow;
            UpdatedBy = changedBy;

            return SuccessResult<bool>.Create(true);
        }

        public Result<bool> RemoveVariant(Guid variantId, UserId changedBy)
        {
            var variant = variants.FirstOrDefault(en => en.VariantId == variantId);
            if (variant == null)
                throw new InvalidOperationException("variant does not exist");

            variants.Remove(variant);
            UpdatedOn = DateTimeOffset.UtcNow;
            UpdatedBy = changedBy;

            return SuccessResult<bool>.Create(true);
        }

        public Result<bool> ReplaceVariants(IEnumerable<DishVariant> newVariants, UserId changedBy)
        {
            if (newVariants == null)
            {
                variants = new List<DishVariant>();
                UpdatedOn = DateTimeOffset.UtcNow;
                UpdatedBy = changedBy;
                return SuccessResult<bool>.Create(true);
            }

            var tempVariants = new List<DishVariant>();

            foreach (var variant in newVariants)
            {
                var tempResult = AddVariant(tempVariants, variant.VariantId, variant.Name, variant.Price);
                if (tempResult.IsFailure)
                    return tempResult;
            }

            variants = tempVariants;
            UpdatedOn = DateTimeOffset.UtcNow;
            UpdatedBy = changedBy;

            return SuccessResult<bool>.Create(true);
        }

        private static Result<bool> AddVariant(IList<DishVariant> variants, Guid variantId, string name, decimal price)
        {
            if (variants == null)
                throw new ArgumentNullException(nameof(variants));

            if (variantId == Guid.Empty)
                throw new InvalidOperationException("variant has no id");
            if (variants.Any(en => en.VariantId == variantId))
                throw new InvalidOperationException("variant already exists");

            if (name != null && name.Length > 40)
                return FailureResult<bool>.Create(FailureResultCode.DishVariantNameTooLong, 40);

            if (!(price > 0))
                return FailureResult<bool>.Create(FailureResultCode.DishVariantPriceIsNegativeOrZero);
            if (price > 200)
                return FailureResult<bool>.Create(FailureResultCode.DishVariantPriceIsTooBig);

            variants.Add(new DishVariant(variantId, name, price));
            return SuccessResult<bool>.Create(true);
        }
    }
}
