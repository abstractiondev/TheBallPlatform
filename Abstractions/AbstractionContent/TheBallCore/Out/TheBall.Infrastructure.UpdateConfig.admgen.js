 

var UpdateConfig {
	PackageData: UpdateConfigItem[];

    constructor() {
					this.PackageData = ko.observableArray(this.PackageData);
    }
}

