import {OrderedDishInfoModel} from './ordered-dish-info.model';

export class CartInfoModel {

  constructor(init?: Partial<CartInfoModel>) {
    if (init) {
      Object.assign(this, init);
    }
    this.orderedDishes = this.orderedDishes?.map(dish => new OrderedDishInfoModel(dish));
  }

  public orderType: number;

  public restaurantId: string;

  public restaurantInfo: string;

  public orderedDishes: OrderedDishInfoModel[];

  public clone(): CartInfoModel {
    return new CartInfoModel({
      orderType: this.orderType,
      restaurantId: this.restaurantId,
      restaurantInfo: this.restaurantInfo,
      orderedDishes: this.orderedDishes?.map(dish => dish?.clone())
    });
  }

}
