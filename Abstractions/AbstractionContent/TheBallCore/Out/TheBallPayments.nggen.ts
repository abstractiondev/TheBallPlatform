 


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

