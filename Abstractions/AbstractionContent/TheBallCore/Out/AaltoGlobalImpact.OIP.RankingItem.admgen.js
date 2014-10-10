 

var RankingItem {
	ContentID: string;
	ContentSemanticType: string;
	RankName: string;
	RankValue: string;

    constructor() {
					this.ContentID = ko.observable(this.ContentID);
			this.ContentSemanticType = ko.observable(this.ContentSemanticType);
			this.RankName = ko.observable(this.RankName);
			this.RankValue = ko.observable(this.RankValue);
    }
}

