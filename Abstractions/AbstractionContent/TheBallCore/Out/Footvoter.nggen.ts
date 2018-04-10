 


import {Injectable} from "@angular/core";
import {TheBallService} from "./theball.service";

@Injectable()
export class FootvoterServicesService {

	constructor(private tbService:TheBallService) {
	}

	async UpdateUserProfile(param:UserProfile) : Promise<any> {
		let result = await this.tbService.ExecuteOperation("Footvoter.Services.UpdateUserProfile", param);
		return result;
	}

	async DoVote(param:VoteData) : Promise<any> {
		let result = await this.tbService.ExecuteOperation("Footvoter.Services.DoVote", param);
		return result;
	}

	async SetCompanyFollow(param:CompanyFollowData) : Promise<any> {
		let result = await this.tbService.ExecuteOperation("Footvoter.Services.SetCompanyFollow", param);
		return result;
	}

	async GetCompanies(param:CompanySearchCriteria) : Promise<any> {
		let result = await this.tbService.ExecuteOperation("Footvoter.Services.GetCompanies", param);
		return result;
	}
}

export class UserProfile {
	public firstName: string;
	public lastName: string;
	public description: string;
	public dateOfBirth: Date;
	public constructor(init?:Partial<UserProfile>) {
		Object.assign(this, init);
	}
}


export class CompanyFollowData {
	public FollowDataItems: FollowDataItem[];
	public constructor(init?:Partial<CompanyFollowData>) {
		Object.assign(this, init);
	}
}


export class FollowDataItem {
	public IDToFollow: string;
	public FollowingLevel: number;
	public constructor(init?:Partial<FollowDataItem>) {
		Object.assign(this, init);
	}
}


export class VoteData {
	public Votes: VoteItem[];
	public constructor(init?:Partial<VoteData>) {
		Object.assign(this, init);
	}
}


export class VoteItem {
	public companyID: string;
	public voteValue: boolean;
	public constructor(init?:Partial<VoteItem>) {
		Object.assign(this, init);
	}
}


export class VotedEntry {
	public VotedForID: string;
	public VoteTime: Date;
	public constructor(init?:Partial<VotedEntry>) {
		Object.assign(this, init);
	}
}


export class VotingSummary {
	public VotedEntries: VotedEntry[];
	public constructor(init?:Partial<VotingSummary>) {
		Object.assign(this, init);
	}
}


export class CompanySearchCriteria {
	public namePart: string;
	public gpsLocation: GpsLocation;
	public constructor(init?:Partial<CompanySearchCriteria>) {
		Object.assign(this, init);
	}
}


export class GpsLocation {
	public latitude: number;
	public longitude: number;
	public constructor(init?:Partial<GpsLocation>) {
		Object.assign(this, init);
	}
}

