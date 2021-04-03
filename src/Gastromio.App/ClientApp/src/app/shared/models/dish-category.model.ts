import {DishModel} from './dish.model';

export class DishCategoryModel {

  constructor(init?: Partial<DishCategoryModel>) {
    if (init) {
      Object.assign(this, init);
    }
    this.dishes = this.dishes?.map(dish => new DishModel(dish));
  }

  public id: string;

  public name: string;

  public enabled: boolean;

  public dishes: Array<DishModel>;

  public clone(): DishCategoryModel {
    return new DishCategoryModel({
      id: this.id,
      name: this.name,
      enabled: this.enabled,
      dishes: this.dishes?.map(dish => dish?.clone())
    });
  }

}
