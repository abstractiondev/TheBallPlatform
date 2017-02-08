 

var ProductPurchaseInfo {
	currentproduct: string;
	expectedprice: number;
	currency: string;
	isTestMode: boolean;

    constructor() {
					this.currentproduct = ko.observable(this.currentproduct);
			this.expectedprice = ko.observable(this.expectedprice);
			this.currency = ko.observable(this.currency);
			this.isTestMode = ko.observable(this.isTestMode);
    }
}

