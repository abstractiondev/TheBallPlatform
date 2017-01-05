 

var UsersData {
	AccountInfos: AccountInfo[];

    constructor() {
					this.AccountInfos = ko.observableArray(this.AccountInfos);
    }
}

