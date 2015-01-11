 


using System;
using System.Collections.ObjectModel;
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
using Newtonsoft.Json;

namespace SQLite.TheBall.Payments { 
		
	internal interface ITheBallDataContextStorable
	{
		void PrepareForStoring();
	}


		public class TheBallDataContext : DataContext
		{

            public TheBallDataContext(IDbConnection connection) : base(connection)
		    {

		    }

            public override void SubmitChanges(ConflictMode failureMode)
            {
                var changeSet = GetChangeSet();
                var requiringBeforeSaveProcessing = changeSet.Inserts.Concat(changeSet.Updates).Cast<ITheBallDataContextStorable>().ToArray();
                foreach (var itemToProcess in requiringBeforeSaveProcessing)
                    itemToProcess.PrepareForStoring();
                base.SubmitChanges(failureMode);
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
		[Column(IsPrimaryKey = true)]
		public string ID { get; set; }


		[Column]
		public string PlanName { get; set; }
		// private string _unmodified_PlanName;

		[Column]
		public string Description { get; set; }
		// private string _unmodified_Description;
        [Column(Name = "GroupIDs")] public string GroupIDsData;

        private bool _IsGroupIDsRetrieved = false;
        private bool _IsGroupIDsChanged = false;
        private ObservableCollection<string> _GroupIDs = null;
        public ObservableCollection<string> GroupIDs
        {
            get
            {
                if (!_IsGroupIDsRetrieved)
                {
                    if (GroupIDsData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<string[]>(GroupIDsData);
                        _GroupIDs = new ObservableCollection<string>(arrayData);
                    }
                    else
                    {
                        _GroupIDs = new ObservableCollection<string>();
						GroupIDsData = Guid.NewGuid().ToString();
						_IsGroupIDsChanged = true;
                    }
                    _IsGroupIDsRetrieved = true;
                    _GroupIDs.CollectionChanged += (sender, args) =>
						{
							GroupIDsData = Guid.NewGuid().ToString();
							_IsGroupIDsChanged = true;
						};
                }
                return _GroupIDs;
            }
            set 
			{ 
				_GroupIDs = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsGroupIDsRetrieved = true;
                GroupIDsData = Guid.NewGuid().ToString();
                _IsGroupIDsChanged = true;

			}
        }

        public void PrepareForStoring()
        {
		
            if (_IsGroupIDsChanged)
            {
                var dataToStore = _GroupIDs.ToArray();
                GroupIDsData = JsonConvert.SerializeObject(dataToStore);
            }

		}
	}
    [Table(Name = "CustomerAccount")]
	public class CustomerAccount : ITheBallDataContextStorable
	{
		[Column(IsPrimaryKey = true)]
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
        [Column(Name = "ActivePlans")] public string ActivePlansData;

        private bool _IsActivePlansRetrieved = false;
        private bool _IsActivePlansChanged = false;
        private ObservableCollection<string> _ActivePlans = null;
        public ObservableCollection<string> ActivePlans
        {
            get
            {
                if (!_IsActivePlansRetrieved)
                {
                    if (ActivePlansData != null)
                    {
                        var arrayData = JsonConvert.DeserializeObject<string[]>(ActivePlansData);
                        _ActivePlans = new ObservableCollection<string>(arrayData);
                    }
                    else
                    {
                        _ActivePlans = new ObservableCollection<string>();
						ActivePlansData = Guid.NewGuid().ToString();
						_IsActivePlansChanged = true;
                    }
                    _IsActivePlansRetrieved = true;
                    _ActivePlans.CollectionChanged += (sender, args) =>
						{
							ActivePlansData = Guid.NewGuid().ToString();
							_IsActivePlansChanged = true;
						};
                }
                return _ActivePlans;
            }
            set 
			{ 
				_ActivePlans = value; 
                // Reset the data field to unique value
                // to trigger change on object, just in case nothing else changed
                _IsActivePlansRetrieved = true;
                ActivePlansData = Guid.NewGuid().ToString();
                _IsActivePlansChanged = true;

			}
        }

        public void PrepareForStoring()
        {
		
            if (_IsActivePlansChanged)
            {
                var dataToStore = _ActivePlans.ToArray();
                ActivePlansData = JsonConvert.SerializeObject(dataToStore);
            }

		}
	}
 } 
