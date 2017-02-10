 

var AccountMetadata {
	AccountID: string;
	Data: System.Dynamic.ExpandoObject;

    constructor() {
					this.AccountID = ko.observable(this.AccountID);
			this.Data = ko.observable(this.Data);
    }
}

