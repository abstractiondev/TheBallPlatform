 

var PartnerSummaryItem {
	Partner: CollaborationPartner;
	ShareInfoSummaryMD5: string;

    constructor() {
					this.Partner = ko.observable(this.Partner);
			this.ShareInfoSummaryMD5 = ko.observable(this.ShareInfoSummaryMD5);
    }
}

