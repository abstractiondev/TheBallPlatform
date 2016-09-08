 

var ShareInfo {
	ItemName: string;
	ContentMD5: string;
	Modified: Date;
	Length: number;

    constructor() {
					this.ItemName = ko.observable(this.ItemName);
			this.ContentMD5 = ko.observable(this.ContentMD5);
			this.Modified = ko.observable(this.Modified);
			this.Length = ko.observable(this.Length);
    }
}

