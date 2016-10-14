 

var CollaborationPartner {
	PartnerType: string;
	PartnerID: string;

    constructor() {
					this.PartnerType = ko.observable(this.PartnerType);
			this.PartnerID = ko.observable(this.PartnerID);
    }
}

