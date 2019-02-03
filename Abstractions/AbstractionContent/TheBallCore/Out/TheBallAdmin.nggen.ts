 


import {Injectable} from "@angular/core";
import {TheBallService} from "./theball.service";

@Injectable()
export class TheBallAdminService {

	constructor(private tbService:TheBallService) {
	}

	async UpdateUsersData() : Promise<any> {
		let result = await this.tbService.ExecuteOperation("TheBall.Admin.UpdateUsersData");
		return result;
	}
}

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

