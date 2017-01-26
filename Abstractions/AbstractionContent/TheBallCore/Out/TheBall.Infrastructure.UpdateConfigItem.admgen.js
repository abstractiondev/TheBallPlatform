 

var UpdateConfigItem {
	AccessInfo: AccessInfo;
	Name: string;
	MaturityLevel: string;
	BuildNumber: string;
	Commit: string;

    constructor() {
					this.AccessInfo = ko.observable(this.AccessInfo);
			this.Name = ko.observable(this.Name);
			this.MaturityLevel = ko.observable(this.MaturityLevel);
			this.BuildNumber = ko.observable(this.BuildNumber);
			this.Commit = ko.observable(this.Commit);
    }
}

