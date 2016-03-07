 

var Member {
	FirstName: string;
	LastName: string;
	MiddleName: string;
	BirthDay: Date;
	Email: string;
	PhoneNumber: string;
	Address: string;
	Address2: string;
	ZipCode: string;
	PostOffice: string;
	Country: string;
	FederationLicense: string;
	PhotoPermission: boolean;
	VideoPermission: boolean;

    constructor() {
					this.FirstName = ko.observable(this.FirstName);
			this.LastName = ko.observable(this.LastName);
			this.MiddleName = ko.observable(this.MiddleName);
			this.BirthDay = ko.observable(this.BirthDay);
			this.Email = ko.observable(this.Email);
			this.PhoneNumber = ko.observable(this.PhoneNumber);
			this.Address = ko.observable(this.Address);
			this.Address2 = ko.observable(this.Address2);
			this.ZipCode = ko.observable(this.ZipCode);
			this.PostOffice = ko.observable(this.PostOffice);
			this.Country = ko.observable(this.Country);
			this.FederationLicense = ko.observable(this.FederationLicense);
			this.PhotoPermission = ko.observable(this.PhotoPermission);
			this.VideoPermission = ko.observable(this.VideoPermission);
    }
}

