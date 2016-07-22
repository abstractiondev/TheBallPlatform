 

var PlanStatus {
	name: string;
	validuntil: Date;
	cancelatperiodend: boolean;

    constructor() {
					this.name = ko.observable(this.name);
			this.validuntil = ko.observable(this.validuntil);
			this.cancelatperiodend = ko.observable(this.cancelatperiodend);
    }
}

