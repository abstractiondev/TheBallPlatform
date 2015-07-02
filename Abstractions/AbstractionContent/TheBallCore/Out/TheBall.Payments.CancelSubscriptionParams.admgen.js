 

var CancelSubscriptionParams {
	PlanName: string;

    constructor() {
					this.PlanName = ko.observable(this.PlanName);
    }
}

