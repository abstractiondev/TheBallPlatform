 


export class ShareCollabParams {
	public Partner: CollaborationPartner;
	public FileName: string;
	public constructor(init?:Partial<ShareCollabParams>) {
		Object.assign(this, init);
	}
}


export class CollaborationPartner {
	public PartnerType: string;
	public PartnerID: string;
	public constructor(init?:Partial<CollaborationPartner>) {
		Object.assign(this, init);
	}
}


export class InterfaceJSONData {
	public Name: string;
	public Data: System.Dynamic.ExpandoObject;
	public constructor(init?:Partial<InterfaceJSONData>) {
		Object.assign(this, init);
	}
}


export class CollaborationPartnerSummary {
	public PartnerData: PartnerSummaryItem[];
	public constructor(init?:Partial<CollaborationPartnerSummary>) {
		Object.assign(this, init);
	}
}


export class PartnerSummaryItem {
	public Partner: CollaborationPartner;
	public ShareInfoSummaryMD5: string;
	public constructor(init?:Partial<PartnerSummaryItem>) {
		Object.assign(this, init);
	}
}


export class ShareInfoSummary {
	public SharedByMe: ShareInfo[];
	public SharedForMe: ShareInfo[];
	public constructor(init?:Partial<ShareInfoSummary>) {
		Object.assign(this, init);
	}
}


export class ShareInfo {
	public ItemName: string;
	public ContentMD5: string;
	public Modified: Date;
	public Length: number;
	public constructor(init?:Partial<ShareInfo>) {
		Object.assign(this, init);
	}
}


export class ConnectionCommunicationData {
	public ActiveSideConnectionID: string;
	public ReceivingSideConnectionID: string;
	public ProcessRequest: string;
	public ProcessParametersString: string;
	public ProcessResultString: string;
	public ProcessResultArray: string[];
	public CategoryCollectionData: CategoryInfo[];
	public LinkItems: CategoryLinkItem[];
	public constructor(init?:Partial<ConnectionCommunicationData>) {
		Object.assign(this, init);
	}
}


export class CategoryInfo {
	public CategoryID: string;
	public NativeCategoryID: string;
	public NativeCategoryDomainName: string;
	public NativeCategoryObjectName: string;
	public NativeCategoryTitle: string;
	public IdentifyingCategoryName: string;
	public ParentCategoryID: string;
	public constructor(init?:Partial<CategoryInfo>) {
		Object.assign(this, init);
	}
}


export class CategoryLinkParameters {
	public ConnectionID: string;
	public LinkItems: CategoryLinkItem[];
	public constructor(init?:Partial<CategoryLinkParameters>) {
		Object.assign(this, init);
	}
}


export class CategoryLinkItem {
	public SourceCategoryID: string;
	public TargetCategoryID: string;
	public LinkingType: string;
	public constructor(init?:Partial<CategoryLinkItem>) {
		Object.assign(this, init);
	}
}


export class AccountMembershipData {
	public Memberships: AccountMembershipItem[];
	public constructor(init?:Partial<AccountMembershipData>) {
		Object.assign(this, init);
	}
}


export class AccountDetails {
	public EmailAddress: string;
	public constructor(init?:Partial<AccountDetails>) {
		Object.assign(this, init);
	}
}


export class AccountMembershipItem {
	public GroupID: string;
	public Role: string;
	public Details: GroupDetails;
	public constructor(init?:Partial<AccountMembershipItem>) {
		Object.assign(this, init);
	}
}


export class GroupMembershipData {
	public Memberships: GroupMembershipItem[];
	public constructor(init?:Partial<GroupMembershipData>) {
		Object.assign(this, init);
	}
}


export class GroupDetails {
	public GroupName: string;
	public constructor(init?:Partial<GroupDetails>) {
		Object.assign(this, init);
	}
}


export class GroupMembershipItem {
	public AccountID: string;
	public Role: string;
	public Details: AccountDetails;
	public constructor(init?:Partial<GroupMembershipItem>) {
		Object.assign(this, init);
	}
}


export class EmailPackage {
	public RecipientAccountIDs: string[];
	public Subject: string;
	public BodyText: string;
	public BodyHtml: string;
	public Attachments: EmailAttachment[];
	public constructor(init?:Partial<EmailPackage>) {
		Object.assign(this, init);
	}
}


export class EmailAttachment {
	public FileName: string;
	public InterfaceDataName: string;
	public TextDataContent: string;
	public Base64Content: string;
	public constructor(init?:Partial<EmailAttachment>) {
		Object.assign(this, init);
	}
}

