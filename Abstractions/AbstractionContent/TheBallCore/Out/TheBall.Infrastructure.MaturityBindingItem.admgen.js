 

var MaturityBindingItem {
	MaturityLevel: string;
	Instances: string[];

    constructor() {
					this.MaturityLevel = ko.observable(this.MaturityLevel);
			this.Instances = ko.observableArray(this.Instances);
    }
}

