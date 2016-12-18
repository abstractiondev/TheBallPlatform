 

var AccountMembershipItem {
	GroupID: string;
	Role: string;
	Details: GroupDetails;

    constructor() {
					this.GroupID = ko.observable(this.GroupID);
			this.Role = ko.observable(this.Role);
			this.Details = ko.observable(this.Details);
    }
}

