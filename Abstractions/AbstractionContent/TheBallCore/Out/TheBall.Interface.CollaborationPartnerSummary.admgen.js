 

var CollaborationPartnerSummary {
	PartnerData: PartnerSummaryItem[];

    constructor() {
					this.PartnerData = ko.observableArray(this.PartnerData);
    }
}

