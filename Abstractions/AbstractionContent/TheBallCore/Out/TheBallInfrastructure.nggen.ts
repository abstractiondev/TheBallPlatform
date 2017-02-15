 


export class UpdateConfig {
	public PackageData: UpdateConfigItem[];
	public constructor(init?:Partial<UpdateConfig>) {
		Object.assign(this, init);
	}
}


export class UpdateConfigItem {
	public AccessInfo: AccessInfo;
	public Name: string;
	public MaturityLevel: string;
	public BuildNumber: string;
	public Commit: string;
	public constructor(init?:Partial<UpdateConfigItem>) {
		Object.assign(this, init);
	}
}


export class AccessInfo {
	public AccountName: string;
	public ShareName: string;
	public SASToken: string;
	public constructor(init?:Partial<AccessInfo>) {
		Object.assign(this, init);
	}
}


export class WebConsoleConfig {
	public PollingIntervalSeconds: number;
	public PackageData: UpdateConfigItem[];
	public InstanceBindings: MaturityBindingItem[];
	public WwwSitesMaturityLevel: string;
	public WwwSiteHostNames: string[];
	public constructor(init?:Partial<WebConsoleConfig>) {
		Object.assign(this, init);
	}
}


export class MaturityBindingItem {
	public MaturityLevel: string;
	public Instances: string[];
	public constructor(init?:Partial<MaturityBindingItem>) {
		Object.assign(this, init);
	}
}


export class DeploymentPackages {
	public PackageData: UpdateConfigItem[];
	public constructor(init?:Partial<DeploymentPackages>) {
		Object.assign(this, init);
	}
}

