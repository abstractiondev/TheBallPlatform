 

var JSONDataContainer {
	Data: System.Dynamic.ExpandoObject;

    constructor() {
					this.Data = ko.observable(this.Data);
    }
}

