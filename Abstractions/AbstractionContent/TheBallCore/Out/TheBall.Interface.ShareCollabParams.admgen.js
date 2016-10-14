 

var ShareCollabParams {
	Partner: CollaborationPartner;
	FileName: string;

    constructor() {
					this.Partner = ko.observable(this.Partner);
			this.FileName = ko.observable(this.FileName);
    }
}

