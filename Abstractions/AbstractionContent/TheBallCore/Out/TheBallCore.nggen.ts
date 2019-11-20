 


import {Injectable} from "@angular/core";
import {TheBallService} from "./theball.service";

@Injectable()
export class TheBallCoreService {

	constructor(private tbService:TheBallService) {
	}

	async SetAccountClientMetadata(param:AccountMetadata) : Promise<any> {
		let result = await this.tbService.ExecuteOperation("TheBall.Core.SetAccountClientMetadata", param);
		return result;
	}

	async SetAccountServerMetadata(param:AccountMetadata) : Promise<any> {
		let result = await this.tbService.ExecuteOperation("TheBall.Core.SetAccountServerMetadata", param);
		return result;
	}
}

export class LoginInfo {
	public EmailAddress: string;
	public Password: string;
	public constructor(init?:Partial<LoginInfo>) {
		Object.assign(this, init);
	}
}


export class ConfirmedLoginInfo {
	public ConfirmationCode: string;
	public LoginInfo: LoginInfo;
	public constructor(init?:Partial<ConfirmedLoginInfo>) {
		Object.assign(this, init);
	}
}


export class LoginRegistrationResult {
	public Success: boolean;
	public constructor(init?:Partial<LoginRegistrationResult>) {
		Object.assign(this, init);
	}
}


export class AccountMetadata {
	public AccountID: string;
	public Data: any;
	public constructor(init?:Partial<AccountMetadata>) {
		Object.assign(this, init);
	}
}


export class DeviceOperationData {
	public OperationRequestString: string;
	public OperationParameters: string[];
	public OperationReturnValues: string[];
	public OperationResult: boolean;
	public OperationSpecificContentData: ContentItemLocationWithMD5[];
	public constructor(init?:Partial<DeviceOperationData>) {
		Object.assign(this, init);
	}
}


export class ContentItemLocationWithMD5 {
	public ContentLocation: string;
	public ContentMD5: string;
	public ItemDatas: ItemData[];
	public constructor(init?:Partial<ContentItemLocationWithMD5>) {
		Object.assign(this, init);
	}
}


export class ItemData {
	public DataName: string;
	public ItemTextData: string;
	public constructor(init?:Partial<ItemData>) {
		Object.assign(this, init);
	}
}

