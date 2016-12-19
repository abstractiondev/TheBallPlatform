 

var EmailAttachment {
	FileName: string;
	InterfaceDataName: string;
	TextDataContent: string;
	Base64Content: string;

    constructor() {
					this.FileName = ko.observable(this.FileName);
			this.InterfaceDataName = ko.observable(this.InterfaceDataName);
			this.TextDataContent = ko.observable(this.TextDataContent);
			this.Base64Content = ko.observable(this.Base64Content);
    }
}

