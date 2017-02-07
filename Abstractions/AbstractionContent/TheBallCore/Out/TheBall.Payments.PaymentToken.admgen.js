 

var PaymentToken {
	id: string;
	currentproduct: string;
	expectedprice: number;
	email: string;
	isTestMode: boolean;
	card: BillingAddress;

    constructor() {
					this.id = ko.observable(this.id);
			this.currentproduct = ko.observable(this.currentproduct);
			this.expectedprice = ko.observable(this.expectedprice);
			this.email = ko.observable(this.email);
			this.isTestMode = ko.observable(this.isTestMode);
			this.card = ko.observable(this.card);
    }
}

