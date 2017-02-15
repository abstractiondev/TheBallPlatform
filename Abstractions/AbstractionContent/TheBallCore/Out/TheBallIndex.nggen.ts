 


import {Injectable} from "@angular/core";
import {TheBallService} from "./theball.service";

@Injectable()
export class TheBallIndexService {

	constructor(private tbService:TheBallService) {
	}

	async PerformUserQuery() : Promise<any> {
		let result = await this.tbService.ExecuteOperation("TheBall.Index.PerformUserQuery");
		return result;
	}
}

export class UserQuery {
	public QueryString: string;
	public DefaultFieldName: string;
	public constructor(init?:Partial<UserQuery>) {
		Object.assign(this, init);
	}
}


export class QueryToken {
	public QueryRequestObjectDomainName: string;
	public QueryRequestObjectName: string;
	public QueryRequestObjectID: string;
	public constructor(init?:Partial<QueryToken>) {
		Object.assign(this, init);
	}
}

