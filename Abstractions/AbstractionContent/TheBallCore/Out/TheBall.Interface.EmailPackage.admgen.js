 

var EmailPackage {
	RecipientAccountIDs: string[];
	Subject: string;
	BodyText: string;
	BodyHtml: string;
	Attachments: EmailAttachment[];

    constructor() {
					this.RecipientAccountIDs = ko.observableArray(this.RecipientAccountIDs);
			this.Subject = ko.observable(this.Subject);
			this.BodyText = ko.observable(this.BodyText);
			this.BodyHtml = ko.observable(this.BodyHtml);
			this.Attachments = ko.observableArray(this.Attachments);
    }
}

