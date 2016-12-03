 

var ConfirmedLoginInfo {
	ConfirmationCode: string;
	LoginInfo: LoginInfo;

    constructor() {
					this.ConfirmationCode = ko.observable(this.ConfirmationCode);
			this.LoginInfo = ko.observable(this.LoginInfo);
    }
}

