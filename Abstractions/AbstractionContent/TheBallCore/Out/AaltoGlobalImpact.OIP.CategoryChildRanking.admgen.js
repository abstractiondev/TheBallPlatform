 

var CategoryChildRanking {
	CategoryID: string;
	RankingItems: RankingItem[];

    constructor() {
					this.CategoryID = ko.observable(this.CategoryID);
			this.RankingItems = ko.observableArray(this.RankingItems);
    }
}

