 


import {Injectable} from "@angular/core";
import {TheBallService} from "./theball.service";

@Injectable()
export class ProBrozOnlineTrainingService {

	constructor(private tbService:TheBallService) {
	}

	async SyncPlansAndPaymentOptionsFromStripe() : Promise<any> {
		let result = await this.tbService.ExecuteOperation("ProBroz.OnlineTraining.SyncPlansAndPaymentOptionsFromStripe");
		return result;
	}

	async GetOrInitiateDefaultGym() : Promise<any> {
		let result = await this.tbService.ExecuteOperation("ProBroz.OnlineTraining.GetOrInitiateDefaultGym");
		return result;
	}

	async CreateMember(param:Member) : Promise<any> {
		let result = await this.tbService.ExecuteOperation("ProBroz.OnlineTraining.CreateMember", param);
		return result;
	}

	async SaveMember(param:Member) : Promise<any> {
		let result = await this.tbService.ExecuteOperation("ProBroz.OnlineTraining.SaveMember", param);
		return result;
	}

	async DeleteMember(param:Member) : Promise<any> {
		let result = await this.tbService.ExecuteOperation("ProBroz.OnlineTraining.DeleteMember", param);
		return result;
	}
}

export class Member {
	public ID: string;
	public ETag: string;
	public FirstName: string;
	public LastName: string;
	public MiddleName: string;
	public BirthDay: Date;
	public Email: string;
	public PhoneNumber: string;
	public Address: string;
	public Address2: string;
	public ZipCode: string;
	public PostOffice: string;
	public Country: string;
	public FederationLicense: string;
	public PhotoPermission: boolean;
	public VideoPermission: boolean;
	public constructor(init?:Partial<Member>) {
		Object.assign(this, init);
	}
}

