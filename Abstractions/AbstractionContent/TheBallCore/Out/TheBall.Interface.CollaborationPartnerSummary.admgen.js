 

var CollaborationPartnerSummary {
	Partners: CollaborationPartner[];

    constructor() {
					this.Partners = ko.observableArray(this.Partners);
    }
}

