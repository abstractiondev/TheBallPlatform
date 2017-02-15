 


export class Member {
	public ID: string;
	public ETag: string;
	public FirstName: string;
	public LastName: string;
	public MiddleName: string;
	public BirthDay: Date;
	public Email: string;
	public PhoneNumber: string;
	public Address: string;
	public Address2: string;
	public ZipCode: string;
	public PostOffice: string;
	public Country: string;
	public FederationLicense: string;
	public PhotoPermission: boolean;
	public VideoPermission: boolean;
	public constructor(init?:Partial<Member>) {
		Object.assign(this, init);
	}
}

