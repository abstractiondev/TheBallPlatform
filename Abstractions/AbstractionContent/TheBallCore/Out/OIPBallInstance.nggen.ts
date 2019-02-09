 


import {Injectable} from "@angular/core";
import {TheBallService} from "./theball.service";

@Injectable()
export class AaltoGlobalImpactOIPService {

	constructor(private tbService:TheBallService) {
	}

	async SetCategoryContentRanking() : Promise<any> {
		let result = await this.tbService.ExecuteOperation("AaltoGlobalImpact.OIP.SetCategoryContentRanking");
		return result;
	}

	async SetCategoryHierarchyAndOrderInNodeSummary() : Promise<any> {
		let result = await this.tbService.ExecuteOperation("AaltoGlobalImpact.OIP.SetCategoryHierarchyAndOrderInNodeSummary");
		return result;
	}

	async ClearDefaultGroupFromAccount() : Promise<any> {
		let result = await this.tbService.ExecuteOperation("AaltoGlobalImpact.OIP.ClearDefaultGroupFromAccount");
		return result;
	}
}

export class CategoryChildRanking {
	public CategoryID: string;
	public RankingItems: RankingItem[];
	public constructor(init?:Partial<CategoryChildRanking>) {
		Object.assign(this, init);
	}
}


export class RankingItem {
	public ContentID: string;
	public ContentSemanticType: string;
	public RankName: string;
	public RankValue: string;
	public constructor(init?:Partial<RankingItem>) {
		Object.assign(this, init);
	}
}


export class ParentToChildren {
	public id: string;
	public children: ParentToChildren[];
	public constructor(init?:Partial<ParentToChildren>) {
		Object.assign(this, init);
	}
}

