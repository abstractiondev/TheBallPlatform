 


using System;
using System.Data;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace SQLite.TheBall.Payments { 
		
	internal interface ITheBallDataContextStorable
	{
		void PrepareForStoring();
	}


		public class TheBallDataContext : DataContext
		{
		    public override void SubmitChanges(ConflictMode failureMode)
		    {
		        var changeSet = GetChangeSet();
		        var requiringBeforeSaveProcessing =
		            changeSet.Inserts.Concat(changeSet.Updates).Cast<ITheBallDataContextStorable>().ToArray();
                foreach(var itemToProcess in requiringBeforeSaveProcessing)
                    itemToProcess.PrepareForStoring();
		        base.SubmitChanges(failureMode);
		    }

		    public TheBallDataContext(IDbConnection connection) : base(connection)
		    {

		    }

			public Table<GroupSubscriptionPlan> GroupSubscriptionPlanTable {
				get {
					return this.GetTable<GroupSubscriptionPlan>();
				}
			}
			public Table<CustomerAccount> CustomerAccountTable {
				get {
					return this.GetTable<CustomerAccount>();
				}
			}
        }

    [Table(Name = "GroupSubscriptionPlan")]
	public class GroupSubscriptionPlan : ITheBallDataContextStorable
	{
		[Column]
		public string ID { get; set; }


		[Column]
		public string PlanName { get; set; }
		// private string _unmodified_PlanName;

		[Column]
		public string Description { get; set; }
		// private string _unmodified_Description;
        [Column(Name = "GroupIDs")] public byte[] GroupIDsData;

		private bool _IsGroupIDsUsed = false;
        private List<string> _GroupIDs = null;
        public List<string> GroupIDs
        {
            get
            {
                if (_GroupIDs == null && GroupIDsData != null)
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    string[] objectArray;
                    using (MemoryStream memStream = new MemoryStream(GroupIDsData))
                        objectArray = (string[]) bf.Deserialize(memStream);
                    _GroupIDs = new List<string>(objectArray);
					_IsGroupIDsUsed = true;
                }
                return _GroupIDs;
            }
            set { _GroupIDs = value; }
        }

        public void PrepareForStoring()
        {
		
            if (_IsGroupIDsUsed)
            {
                if (_GroupIDs == null)
                    GroupIDsData = null;
                else
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    var dataToStore = _GroupIDs.ToArray();
                    using (MemoryStream memStream = new MemoryStream())
                    {
                        bf.Serialize(memStream, dataToStore);
                        GroupIDsData = memStream.ToArray();
                    }
                }
            }

		}
	}
    [Table(Name = "CustomerAccount")]
	public class CustomerAccount : ITheBallDataContextStorable
	{
		[Column]
		public string ID { get; set; }


		[Column]
		public string StripeID { get; set; }
		// private string _unmodified_StripeID;

		[Column]
		public string EmailAddress { get; set; }
		// private string _unmodified_EmailAddress;

		[Column]
		public string Description { get; set; }
		// private string _unmodified_Description;
        [Column(Name = "ActivePlans")] public byte[] ActivePlansData;

		private bool _IsActivePlansUsed = false;
        private List<string> _ActivePlans = null;
        public List<string> ActivePlans
        {
            get
            {
                if (_ActivePlans == null && ActivePlansData != null)
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    string[] objectArray;
                    using (MemoryStream memStream = new MemoryStream(ActivePlansData))
                        objectArray = (string[]) bf.Deserialize(memStream);
                    _ActivePlans = new List<string>(objectArray);
					_IsActivePlansUsed = true;
                }
                return _ActivePlans;
            }
            set { _ActivePlans = value; }
        }

        public void PrepareForStoring()
        {
		
            if (_IsActivePlansUsed)
            {
                if (_ActivePlans == null)
                    ActivePlansData = null;
                else
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    var dataToStore = _ActivePlans.ToArray();
                    using (MemoryStream memStream = new MemoryStream())
                    {
                        bf.Serialize(memStream, dataToStore);
                        ActivePlansData = memStream.ToArray();
                    }
                }
            }

		}
	}
 } 
