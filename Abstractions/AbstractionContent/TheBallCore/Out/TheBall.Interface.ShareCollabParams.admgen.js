 

var ShareCollabParams {
	ColTargetType: string;
	ColTargetID: string;
	FileName: string;

    constructor() {
					this.ColTargetType = ko.observable(this.ColTargetType);
			this.ColTargetID = ko.observable(this.ColTargetID);
			this.FileName = ko.observable(this.FileName);
    }
}

