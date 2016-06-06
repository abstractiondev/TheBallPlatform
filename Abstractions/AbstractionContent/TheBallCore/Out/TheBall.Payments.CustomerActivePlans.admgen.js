 

var CustomerActivePlans {
	PlanStatuses: PlanStatus[];

    constructor() {
					this.PlanStatuses = ko.observableArray(this.PlanStatuses);
    }
}

