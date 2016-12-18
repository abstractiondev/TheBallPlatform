 

var AccountMembershipData {
	Memberships: AccountMembershipItem[];

    constructor() {
					this.Memberships = ko.observableArray(this.Memberships);
    }
}

