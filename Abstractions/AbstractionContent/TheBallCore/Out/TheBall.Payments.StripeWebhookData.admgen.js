 

var StripeWebhookData {
	id: string;
	livemode: boolean;
	type: string;

    constructor() {
					this.id = ko.observable(this.id);
			this.livemode = ko.observable(this.livemode);
			this.type = ko.observable(this.type);
    }
}

