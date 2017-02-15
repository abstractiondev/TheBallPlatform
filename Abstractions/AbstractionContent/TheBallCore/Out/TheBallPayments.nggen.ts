 


import {Injectable} from "@angular/core";
import {TheBallService} from "./theball.service";

@Injectable()
export class TheBallPaymentsService {

	constructor(private tbService:TheBallService) {
	}

	async ProcessStripeWebhook(param:StripeWebhookData) : Promise<any> {
		let result = await this.tbService.ExecuteOperation("TheBall.Payments.ProcessStripeWebhook", param);
		return result;
	}

	async CancelAccountPlan(param:CancelSubscriptionParams) : Promise<any> {
		let result = await this.tbService.ExecuteOperation("TheBall.Payments.CancelAccountPlan", param);
		return result;
	}

	async PurchaseProduct(param:ProductPurchaseInfo) : Promise<any> {
		let result = await this.tbService.ExecuteOperation("TheBall.Payments.PurchaseProduct", param);
		return result;
	}

	async ActivateAccountPlan(param:PaymentToken) : Promise<any> {
		let result = await this.tbService.ExecuteOperation("TheBall.Payments.ActivateAccountPlan", param);
		return result;
	}

	async ActivateAndPayGroupSubscriptionPlan() : Promise<any> {
		let result = await this.tbService.ExecuteOperation("TheBall.Payments.ActivateAndPayGroupSubscriptionPlan");
		return result;
	}

	async CancelGroupSubscriptionPlan() : Promise<any> {
		let result = await this.tbService.ExecuteOperation("TheBall.Payments.CancelGroupSubscriptionPlan");
		return result;
	}

	async ProcessPayment() : Promise<any> {
		let result = await this.tbService.ExecuteOperation("TheBall.Payments.ProcessPayment");
		return result;
	}
}

export class CancelSubscriptionParams {
	public PlanName: string;
	public constructor(init?:Partial<CancelSubscriptionParams>) {
		Object.assign(this, init);
	}
}


export class StripeWebhookData {
	public id: string;
	public livemode: boolean;
	public type: string;
	public constructor(init?:Partial<StripeWebhookData>) {
		Object.assign(this, init);
	}
}


export class ProductPurchaseInfo {
	public currentproduct: string;
	public expectedprice: number;
	public currency: string;
	public isTestMode: boolean;
	public constructor(init?:Partial<ProductPurchaseInfo>) {
		Object.assign(this, init);
	}
}


export class PaymentToken {
	public id: string;
	public currentproduct: string;
	public expectedprice: number;
	public currency: string;
	public email: string;
	public isTestMode: boolean;
	public card: BillingAddress;
	public constructor(init?:Partial<PaymentToken>) {
		Object.assign(this, init);
	}
}


export class BillingAddress {
	public name: string;
	public address_city: string;
	public address_country: string;
	public address_line1: string;
	public address_line1_check: string;
	public address_zip: string;
	public address_zip_check: string;
	public cvc_check: string;
	public exp_month: string;
	public exp_year: string;
	public last4: string;
	public constructor(init?:Partial<BillingAddress>) {
		Object.assign(this, init);
	}
}


export class CustomerActivePlans {
	public PlanStatuses: PlanStatus[];
	public constructor(init?:Partial<CustomerActivePlans>) {
		Object.assign(this, init);
	}
}


export class PlanStatus {
	public name: string;
	public validuntil: Date;
	public cancelatperiodend: boolean;
	public constructor(init?:Partial<PlanStatus>) {
		Object.assign(this, init);
	}
}

