 


export class UsersData {
	public AccountInfos: AccountInfo[];
	public constructor(init?:Partial<UsersData>) {
		Object.assign(this, init);
	}
}


export class AccountInfo {
	public AccountID: string;
	public EmailAddress: string;
	public constructor(init?:Partial<AccountInfo>) {
		Object.assign(this, init);
	}
}

