 

var GroupMembershipData {
	Memberships: GroupMembershipItem[];

    constructor() {
					this.Memberships = ko.observableArray(this.Memberships);
    }
}

