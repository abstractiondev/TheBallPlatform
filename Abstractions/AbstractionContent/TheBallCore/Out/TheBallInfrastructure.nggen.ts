 


import {Injectable} from "@angular/core";
import {TheBallService} from "./theball.service";

@Injectable()
export class TheBallInfrastructureService {

	constructor(private tbService:TheBallService) {
	}

	async UpdateInfraDataInterfaceObjects() : Promise<any> {
		let result = await this.tbService.ExecuteOperation("TheBall.Infrastructure.UpdateInfraDataInterfaceObjects");
		return result;
	}

	async SetRuntimeVersions(param:UpdateConfig) : Promise<any> {
		let result = await this.tbService.ExecuteOperation("TheBall.Infrastructure.SetRuntimeVersions", param);
		return result;
	}

	async CreateCloudDrive() : Promise<any> {
		let result = await this.tbService.ExecuteOperation("TheBall.Infrastructure.CreateCloudDrive");
		return result;
	}

	async MountCloudDrive() : Promise<any> {
		let result = await this.tbService.ExecuteOperation("TheBall.Infrastructure.MountCloudDrive");
		return result;
	}
}

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
	public Status: StatusInfo;
	public constructor(init?:Partial<UpdateConfigItem>) {
		Object.assign(this, init);
	}
}


export class StatusInfo {
	public TestResult: number;
	public TestedAt: Date;
	public InstalledAt: Date;
	public constructor(init?:Partial<StatusInfo>) {
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


export class BaseUIConfigSet {
	public AboutConfig: UpdateConfigItem;
	public AccountConfig: UpdateConfigItem;
	public GroupConfig: UpdateConfigItem;
	public StatusSummary: StatusInfo;
	public constructor(init?:Partial<BaseUIConfigSet>) {
		Object.assign(this, init);
	}
}


export class InstanceUIConfig {
	public DesiredConfig: BaseUIConfigSet;
	public ConfigInTesting: BaseUIConfigSet;
	public EffectiveConfig: BaseUIConfigSet;
	public constructor(init?:Partial<InstanceUIConfig>) {
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

