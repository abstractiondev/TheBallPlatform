 

var InterfaceJSONData {
	Name: string;
	Data: System.Dynamic.ExpandoObject;

    constructor() {
					this.Name = ko.observable(this.Name);
			this.Data = ko.observable(this.Data);
    }
}

