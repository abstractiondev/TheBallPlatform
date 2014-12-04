 

var PaymentToken {
	id: string;
	currentproduct: string;
	email: string;

    constructor() {
					this.id = ko.observable(this.id);
			this.currentproduct = ko.observable(this.currentproduct);
			this.email = ko.observable(this.email);
    }
}

