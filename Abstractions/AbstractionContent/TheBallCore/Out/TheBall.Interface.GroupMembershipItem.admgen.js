 

var GroupMembershipItem {
	AccountID: string;
	Role: string;
	Details: AccountDetails;

    constructor() {
					this.AccountID = ko.observable(this.AccountID);
			this.Role = ko.observable(this.Role);
			this.Details = ko.observable(this.Details);
    }
}

