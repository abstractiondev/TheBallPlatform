 


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

