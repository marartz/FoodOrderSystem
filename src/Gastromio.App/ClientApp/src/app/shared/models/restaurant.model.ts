import {PaymentMethodModel} from './payment-method.model';
import {UserModel} from './user.model';
import {CuisineModel} from './cuisine.model';
import {DateModel} from "./date.model";
import * as moment from "moment";

export class RestaurantModel {

  constructor(init?: Partial<RestaurantModel>) {
    if (init) {
      Object.assign(this, init);
    }
    this.address = new AddressModel(this.address);
    this.contactInfo = new ContactInfoModel(this.contactInfo);
    this.regularOpeningDays = this.regularOpeningDays?.map(day => new RegularOpeningDayModel(day)) ?? new Array<RegularOpeningDayModel>();
    this.deviatingOpeningDays = this.deviatingOpeningDays?.map(day => new DeviatingOpeningDayModel(day)) ?? new Array<DeviatingOpeningDayModel>();
    this.pickupInfo = new PickupInfoModel(this.pickupInfo);
    this.deliveryInfo = new DeliveryInfoModel(this.deliveryInfo);
    this.reservationInfo = new ReservationInfoModel(this.reservationInfo);
    this.externalMenus = this.externalMenus?.map(menu => new ExternalMenu(menu)) ?? new Array<ExternalMenu>();
    this.paymentMethods = this.paymentMethods?.map(paymentMethod => new PaymentMethodModel(paymentMethod))
      .sort((a, b) => {
        if (a.name < b.name)
          return -1
        else if (a.name > b.name)
          return 1;
        return 0;
      });

    try {
      const createdOnMoment = moment.utc(this.createdOn);
      this.createdOnDate = createdOnMoment.local().toDate();
    } catch (e) {}

    try {
      const updatedOnMoment = moment.utc(this.updatedOn);
      this.updatedOnDate = updatedOnMoment.local().toDate();
    } catch (e) {}
  }


  public id: string;

  public name: string;

  public importId: string;

  public alias: string;

  public address: AddressModel;

  public contactInfo: ContactInfoModel;

  public imageTypes: string[];

  public regularOpeningDays: Array<RegularOpeningDayModel>;

  public deviatingOpeningDays: Array<DeviatingOpeningDayModel>;

  public regularOpeningHoursText: string;

  public deviatingOpeningHoursText: string;

  public openingHoursTodayText: string;

  public pickupInfo: PickupInfoModel;

  public deliveryInfo: DeliveryInfoModel;

  public reservationInfo: ReservationInfoModel;

  public hygienicHandling: string;

  public cuisines: CuisineModel[];

  public cuisinesText: string;

  public paymentMethods: PaymentMethodModel[];

  public administrators: UserModel[];

  public isActive: boolean;

  public needsSupport: boolean;

  public supportedOrderMode: string;

  public externalMenus: ExternalMenu[];

  public createdOn: string;

  public createdOnDate: Date;

  public createdBy: UserModel;

  public updatedOn: string;

  public updatedOnDate: Date;

  public updatedBy: UserModel;

  public clone(): RestaurantModel {
    return new RestaurantModel({
      id: this.id,
      name: this.name,
      importId: this.importId,
      alias: this.alias,
      address: this.address?.clone(),
      contactInfo: this.contactInfo?.clone(),
      imageTypes: this.imageTypes?.map(imageType => imageType),
      regularOpeningDays: this.regularOpeningDays?.map(day => day?.clone()),
      deviatingOpeningDays: this.deviatingOpeningDays?.map(day => day?.clone()),
      regularOpeningHoursText: this.regularOpeningHoursText,
      deviatingOpeningHoursText: this.deviatingOpeningHoursText,
      openingHoursTodayText: this.openingHoursTodayText,
      pickupInfo: this.pickupInfo?.clone(),
      deliveryInfo: this.deliveryInfo?.clone(),
      reservationInfo: this.reservationInfo?.clone(),
      hygienicHandling: this.hygienicHandling,
      cuisines: this.cuisines?.map(cuisine => cuisine?.clone()),
      cuisinesText: this.cuisinesText,
      paymentMethods: this.paymentMethods?.map(paymentMethod => paymentMethod?.clone()),
      administrators: this.administrators?.map(administrator => administrator?.clone()),
      isActive: this.isActive,
      needsSupport: this.needsSupport,
      supportedOrderMode: this.supportedOrderMode,
      externalMenus: this.externalMenus?.map(menu => menu?.clone()),
      createdOn: this.createdOn,
      createdOnDate: this.createdOnDate,
      createdBy: this.createdBy.clone(),
      updatedOn: this.updatedOn,
      updatedOnDate: this.updatedOnDate,
      updatedBy: this.updatedBy.clone()
    });
  }

  public isOpen(dateTime: Date): boolean {
    if (dateTime === undefined || dateTime < new Date())
      dateTime = new Date();
    return this.findOpeningPeriod(dateTime) !== undefined;
  }

  public getRestaurantClosedReason(dateTime: Date): string {
    if (dateTime === undefined || dateTime < new Date())
      dateTime = new Date();
    const date = new DateModel(dateTime.getFullYear(), dateTime.getMonth() + 1, dateTime.getDate());
    const deviatingOpeningDay = this.deviatingOpeningDays?.find(en => DateModel.isEqual(en.date, date));
    if (!deviatingOpeningDay)
      return "geschlossen";

    if (deviatingOpeningDay.status === "fully-booked") {
      return "ausgebucht";
    } else {
      return "geschlossen";
    }
  }

  public isOrderPossibleAt(orderDateTime: Date): boolean {
    const now = new Date();

    if (orderDateTime === undefined || orderDateTime < now)
      orderDateTime = new Date();

    const today = new Date();
    today.setHours(0, 0, 0, 0);

    const orderDate = new Date(orderDateTime.getFullYear(), orderDateTime.getMonth(), orderDateTime.getDate());

    if (this.supportedOrderMode === 'phone') {
      return false;
    }

    const openingPeriodOfOrderDateTime = this.findOpeningPeriod(orderDateTime);
    if (openingPeriodOfOrderDateTime === undefined) {
      return false;
    }

    if (this.supportedOrderMode !== 'shift') {
      return true;
    }

    if (orderDate > today) {
      return true;
    }

    const openingPeriodOfNow = this.findOpeningPeriod(now);
    if (openingPeriodOfNow === undefined) {
      return true;
    }

    return openingPeriodOfNow !== openingPeriodOfOrderDateTime;
  }

  private findOpeningPeriod(dateTime: Date): OpeningPeriodModel {
    let dayOfWeek = (dateTime.getDay() - 1) % 7; // DayOfWeek starts with Sunday
    if (dayOfWeek < 0) {
      dayOfWeek += 7;
    }
    let time = dateTime.getHours() * 60 + dateTime.getMinutes();
    if (dateTime.getHours() < 4) {
      dayOfWeek = (dayOfWeek - 1) % 7;
      if (dayOfWeek < 0) {
        dayOfWeek += 7;
      }
      time += 24 * 60;
    }

    if (this.deviatingOpeningDays) {
      const date = new DateModel(dateTime.getFullYear(), dateTime.getMonth() + 1, dateTime.getDate());
      const deviatingOpeningDay = this.deviatingOpeningDays.find(en => DateModel.isEqual(en.date, date));
      if (deviatingOpeningDay) {
        return deviatingOpeningDay?.openingPeriods.find(en => en.start <= time && time <= en.end);
      }
    }

    if (this.regularOpeningDays) {
      const regularOpeningDay = this.regularOpeningDays.find(en => en.dayOfWeek === dayOfWeek);
      if (regularOpeningDay) {
        return regularOpeningDay?.openingPeriods.find(en => en.start <= time && time <= en.end);
      }
    }

    return undefined;
  }

}

export class AddressModel {

  constructor(init?: Partial<AddressModel>) {
    if (init) {
      Object.assign(this, init);
    }
  }

  public street: string;

  public zipCode: string;

  public city: string;

  public clone(): AddressModel {
    return new AddressModel({
      street: this.street,
      zipCode: this.zipCode,
      city: this.city
    });
  }

}

export class ContactInfoModel {

  constructor(init?: Partial<ContactInfoModel>) {
    if (init) {
      Object.assign(this, init);
    }
  }

  public phone: string;

  public fax: string;

  public webSite: string;

  public responsiblePerson: string;

  public emailAddress: string;

  public mobile: string;

  public orderNotificationByMobile: boolean;

  public clone(): ContactInfoModel {
    return new ContactInfoModel({
      phone: this.phone,
      fax: this.fax,
      webSite: this.webSite,
      responsiblePerson: this.responsiblePerson,
      emailAddress: this.emailAddress,
      mobile: this.mobile,
      orderNotificationByMobile: this.orderNotificationByMobile
    });
  }

}

export class OpeningPeriodModel {

  constructor(init?: Partial<OpeningPeriodModel>) {
    if (init) {
      Object.assign(this, init);
    }
  }

  public start: number;

  public end: number;

  public clone(): OpeningPeriodModel {
    return new OpeningPeriodModel({
      start: this.start,
      end: this.end
    });
  }

}

export class RegularOpeningDayModel {

  constructor(init?: Partial<RegularOpeningDayModel>) {
    if (init) {
      Object.assign(this, init);
    }
    this.openingPeriods = this.openingPeriods?.map(period => new OpeningPeriodModel(period)) ?? new Array<OpeningPeriodModel>();
  }

  public dayOfWeek: number;

  public openingPeriods: Array<OpeningPeriodModel>;

  public clone(): RegularOpeningDayModel {
    return new RegularOpeningDayModel({
      dayOfWeek: this.dayOfWeek,
      openingPeriods: this.openingPeriods?.map(period => period?.clone())
    })
  }

}

export class DeviatingOpeningDayModel {

  constructor(init?: Partial<DeviatingOpeningDayModel>) {
    if (init) {
      Object.assign(this, init);
    }
    this.openingPeriods = this.openingPeriods?.map(period => new OpeningPeriodModel(period)) ?? new Array<OpeningPeriodModel>();
  }

  public date: DateModel;

  public status: string;

  public openingPeriods: Array<OpeningPeriodModel>;

  public clone(): DeviatingOpeningDayModel {
    return new DeviatingOpeningDayModel({
      date: this.date?.clone(),
      status: this.status,
      openingPeriods: this.openingPeriods?.map(period => period?.clone())
    });
  }

}

export class PickupInfoModel {

  constructor(init?: Partial<PickupInfoModel>) {
    if (init) {
      Object.assign(this, init);
    }
  }

  public enabled: boolean;

  public averageTime: number;

  public minimumOrderValue: number;

  public minimumOrderValueText: string;

  public maximumOrderValue: number;

  public maximumOrderValueText: string;

  public clone(): PickupInfoModel {
    return new PickupInfoModel({
      enabled: this.enabled,
      averageTime: this.averageTime,
      minimumOrderValue: this.minimumOrderValue,
      minimumOrderValueText: this.minimumOrderValueText,
      maximumOrderValue: this.maximumOrderValue,
      maximumOrderValueText: this.maximumOrderValueText
    });
  }

}

export class DeliveryInfoModel {

  constructor(init?: Partial<DeliveryInfoModel>) {
    if (init) {
      Object.assign(this, init);
    }
  }

  public enabled: boolean;

  public averageTime: number;

  public minimumOrderValue: number;

  public minimumOrderValueText: string;

  public maximumOrderValue: number;

  public maximumOrderValueText: string;

  public costs: number;

  public clone(): DeliveryInfoModel {
    return new DeliveryInfoModel({
      enabled: this.enabled,
      averageTime: this.averageTime,
      minimumOrderValue: this.minimumOrderValue,
      minimumOrderValueText: this.minimumOrderValueText,
      maximumOrderValue: this.maximumOrderValue,
      maximumOrderValueText: this.maximumOrderValueText
    });
  }

  public getCostsText(): string {
    const val = this.costs;
    if (val === undefined) {
      return undefined;
    } else if (val > 0) {
      return 'Min. ' + val.toLocaleString('de', {minimumFractionDigits: 2}) + ' €';
    } else {
      return 'Gratis';
    }
  }

}

export class ReservationInfoModel {

  constructor(init?: Partial<ReservationInfoModel>) {
    if (init) {
      Object.assign(this, init);
    }
  }

  public enabled: boolean;

  public reservationSystemUrl: string;

  public clone(): ReservationInfoModel {
    return new ReservationInfoModel({
      enabled: this.enabled,
      reservationSystemUrl: this.reservationSystemUrl
    });
  }

}

export class ServiceInfoModel {

  constructor(init?: Partial<ServiceInfoModel>) {
    if (init) {
      Object.assign(this, init);
    }
  }
  public pickupEnabled: boolean;

  public pickupAverageTime: number;

  public pickupMinimumOrderValue: number;

  public pickupMaximumOrderValue: number;

  public deliveryEnabled: boolean;

  public deliveryAverageTime: number;

  public deliveryMinimumOrderValue: number;

  public deliveryMaximumOrderValue: number;

  public deliveryCosts: number;

  public reservationEnabled: boolean;

  public reservationSystemUrl: string;

  public hygienicHandling: string;

  public clone(): ServiceInfoModel {
    return new ServiceInfoModel({
      pickupEnabled: this.pickupEnabled,
      pickupAverageTime: this.pickupAverageTime,
      pickupMinimumOrderValue: this.pickupMinimumOrderValue,
      pickupMaximumOrderValue: this.pickupMaximumOrderValue,
      deliveryEnabled: this.deliveryEnabled,
      deliveryAverageTime: this.deliveryAverageTime,
      deliveryMinimumOrderValue: this.deliveryMinimumOrderValue,
      deliveryMaximumOrderValue: this.deliveryMaximumOrderValue,
      deliveryCosts: this.deliveryCosts,
      reservationEnabled: this.reservationEnabled,
      reservationSystemUrl: this.reservationSystemUrl,
      hygienicHandling: this.hygienicHandling
    });
  }

}

export class ExternalMenu {

  constructor(init?: Partial<ExternalMenu>) {
    if (init) {
      Object.assign(this, init);
    }
  }

  public id: string;

  public name: string;

  public description: string;

  public url: string;

  public clone(): ExternalMenu {
    return new ExternalMenu({
      id: this.id,
      name: this.name,
      description: this.description,
      url: this.url
    });
  }

}
