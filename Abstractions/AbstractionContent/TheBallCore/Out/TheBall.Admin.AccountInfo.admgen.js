 

var AccountInfo {
	AccountID: string;
	EmailAddress: string;

    constructor() {
					this.AccountID = ko.observable(this.AccountID);
			this.EmailAddress = ko.observable(this.EmailAddress);
    }
}

