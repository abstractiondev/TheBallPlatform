 

var PaymentToken {
	id: string;
	currentproduct: string;
	email: string;
	card: BillingAddress;

    constructor() {
					this.id = ko.observable(this.id);
			this.currentproduct = ko.observable(this.currentproduct);
			this.email = ko.observable(this.email);
			this.card = ko.observable(this.card);
    }
}

