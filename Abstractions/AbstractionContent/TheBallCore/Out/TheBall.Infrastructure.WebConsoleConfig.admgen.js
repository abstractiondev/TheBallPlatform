 

var WebConsoleConfig {
	PollingIntervalSeconds: number;
	PackageData: UpdateConfigItem[];
	InstanceBindings: MaturityBindingItem[];
	WwwSitesMaturityLevel: string;
	WwwSiteHostNames: string[];

    constructor() {
					this.PollingIntervalSeconds = ko.observable(this.PollingIntervalSeconds);
			this.PackageData = ko.observableArray(this.PackageData);
			this.InstanceBindings = ko.observableArray(this.InstanceBindings);
			this.WwwSitesMaturityLevel = ko.observable(this.WwwSitesMaturityLevel);
			this.WwwSiteHostNames = ko.observableArray(this.WwwSiteHostNames);
    }
}

