 

var AccessInfo {
	AccountName: string;
	ShareName: string;
	SASToken: string;

    constructor() {
					this.AccountName = ko.observable(this.AccountName);
			this.ShareName = ko.observable(this.ShareName);
			this.SASToken = ko.observable(this.SASToken);
    }
}

