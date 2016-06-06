 

var PlanStatus {
	name: string;
	validuntil: Date;

    constructor() {
					this.name = ko.observable(this.name);
			this.validuntil = ko.observable(this.validuntil);
    }
}

