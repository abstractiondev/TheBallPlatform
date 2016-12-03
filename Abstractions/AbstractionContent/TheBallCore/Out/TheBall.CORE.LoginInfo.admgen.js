 

var LoginInfo {
	EmailAddress: string;
	Password: string;

    constructor() {
					this.EmailAddress = ko.observable(this.EmailAddress);
			this.Password = ko.observable(this.Password);
    }
}

