 

var ShareInfoSummary {
	SharedByMe: ShareInfo[];
	SharedForMe: ShareInfo[];

    constructor() {
					this.SharedByMe = ko.observableArray(this.SharedByMe);
			this.SharedForMe = ko.observableArray(this.SharedForMe);
    }
}

