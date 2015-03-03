 

var BillingAddress {
	name: string;
	address_city: string;
	address_country: string;
	address_line1: string;
	address_line1_check: string;
	address_zip: string;
	address_zip_check: string;
	cvc_check: string;
	exp_month: string;
	exp_year: string;
	last4: string;

    constructor() {
					this.name = ko.observable(this.name);
			this.address_city = ko.observable(this.address_city);
			this.address_country = ko.observable(this.address_country);
			this.address_line1 = ko.observable(this.address_line1);
			this.address_line1_check = ko.observable(this.address_line1_check);
			this.address_zip = ko.observable(this.address_zip);
			this.address_zip_check = ko.observable(this.address_zip_check);
			this.cvc_check = ko.observable(this.cvc_check);
			this.exp_month = ko.observable(this.exp_month);
			this.exp_year = ko.observable(this.exp_year);
			this.last4 = ko.observable(this.last4);
    }
}

