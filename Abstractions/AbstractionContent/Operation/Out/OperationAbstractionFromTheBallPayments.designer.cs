 

using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

		namespace TheBall.Payments { 
				public class ProcessStripeWebHookParameters 
		{
				public INT.StripeWebhookData JSONObject ;
				}
		
		public class ProcessStripeWebHook 
		{
				private static void PrepareParameters(ProcessStripeWebHookParameters parameters)
		{
					}
				public static void Execute(ProcessStripeWebHookParameters parameters)
		{
						PrepareParameters(parameters);
					}
				}
				public class ValidatePlanContainingGroupsParameters 
		{
				public string PlanName ;
				}
		
		public class ValidatePlanContainingGroups 
		{
				private static void PrepareParameters(ValidatePlanContainingGroupsParameters parameters)
		{
					}
				public static void Execute(ValidatePlanContainingGroupsParameters parameters)
		{
						PrepareParameters(parameters);
					GroupSubscriptionPlan GroupSubscriptionPlan = ValidatePlanContainingGroupsImplementation.GetTarget_GroupSubscriptionPlan(parameters.PlanName);	
				ValidatePlanContainingGroupsImplementation.ExecuteMethod_ValidateGroupsInPlan(GroupSubscriptionPlan);		
				}
				}
		
		public class ActivateAndPayGroupSubscriptionPlan 
		{
				public static void Execute()
		{
						
					INT.PaymentToken PaymentToken = ActivateAndPayGroupSubscriptionPlanImplementation.GetTarget_PaymentToken();	
				ActivateAndPayGroupSubscriptionPlanImplementation.ExecuteMethod_ValidateMatchingEmail(PaymentToken);		
				string AccountID = ActivateAndPayGroupSubscriptionPlanImplementation.GetTarget_AccountID();	
				CustomerAccount CustomerAccount = ActivateAndPayGroupSubscriptionPlanImplementation.GetTarget_CustomerAccount(AccountID);	
				ActivateAndPayGroupSubscriptionPlanImplementation.ExecuteMethod_UpdateStripeCustomerData(CustomerAccount, PaymentToken);		
				string StripeCustomerID = ActivateAndPayGroupSubscriptionPlanImplementation.GetTarget_StripeCustomerID(CustomerAccount);	
				string PlanName = ActivateAndPayGroupSubscriptionPlanImplementation.GetTarget_PlanName(PaymentToken);	
				
		{ // Local block to allow local naming
			ValidatePlanContainingGroupsParameters operationParameters = ActivateAndPayGroupSubscriptionPlanImplementation.ValidatePlanGroups_GetParameters(PlanName);
			ValidatePlanContainingGroups.Execute(operationParameters);
									
		} // Local block closing
				ActivateAndPayGroupSubscriptionPlanImplementation.ExecuteMethod_ValidateStripePlanName(PlanName);		
				Stripe.StripeSubscription[] CustomersActiveSubscriptions = ActivateAndPayGroupSubscriptionPlanImplementation.GetTarget_CustomersActiveSubscriptions(StripeCustomerID);	
				string[] CustomersActivePlanNames = ActivateAndPayGroupSubscriptionPlanImplementation.GetTarget_CustomersActivePlanNames(CustomersActiveSubscriptions);	
				ActivateAndPayGroupSubscriptionPlanImplementation.ExecuteMethod_SyncCurrentCustomerActivePlans(CustomerAccount, CustomersActivePlanNames);		
				ActivateAndPayGroupSubscriptionPlanImplementation.ExecuteMethod_ProcessPayment(StripeCustomerID, PlanName, CustomersActivePlanNames, PaymentToken);		
				ActivateAndPayGroupSubscriptionPlanImplementation.ExecuteMethod_AddPlanAsActiveToCustomer(CustomerAccount, PlanName);		
				ActivateAndPayGroupSubscriptionPlanImplementation.ExecuteMethod_StoreObjects(CustomerAccount);		
				
		{ // Local block to allow local naming
			GrantPlanAccessToAccountParameters operationParameters = ActivateAndPayGroupSubscriptionPlanImplementation.GrantAccessToPaidPlan_GetParameters(CustomerAccount, PlanName);
			GrantPlanAccessToAccount.Execute(operationParameters);
									
		} // Local block closing
				}
				}
				public class CancelGroupSubscriptionPlanParameters 
		{
				public string PlanName ;
				public string AccountID ;
				}
		
		public class CancelGroupSubscriptionPlan 
		{
				private static void PrepareParameters(CancelGroupSubscriptionPlanParameters parameters)
		{
					}
				public static void Execute(CancelGroupSubscriptionPlanParameters parameters)
		{
						PrepareParameters(parameters);
					CustomerAccount CustomerAccount = CancelGroupSubscriptionPlanImplementation.GetTarget_CustomerAccount(parameters.AccountID);	
				CancelGroupSubscriptionPlanImplementation.ExecuteMethod_CancelSubscriptionPlan(parameters.PlanName, CustomerAccount);		
				
		{ // Local block to allow local naming
			RevokePlanAccessFromAccountParameters operationParameters = CancelGroupSubscriptionPlanImplementation.RevokeAccessToCanceledPlan_GetParameters(parameters.PlanName, parameters.AccountID);
			RevokePlanAccessFromAccount.Execute(operationParameters);
									
		} // Local block closing
				}
				}
				public class GrantPlanAccessToAccountParameters 
		{
				public string PlanName ;
				public string AccountID ;
				}
		
		public class GrantPlanAccessToAccount 
		{
				private static void PrepareParameters(GrantPlanAccessToAccountParameters parameters)
		{
					}
				public static void Execute(GrantPlanAccessToAccountParameters parameters)
		{
						PrepareParameters(parameters);
					GroupSubscriptionPlan GroupSubscriptionPlan = GrantPlanAccessToAccountImplementation.GetTarget_GroupSubscriptionPlan(parameters.PlanName);	
				GrantPlanAccessToAccountImplementation.ExecuteMethod_GrantAccessToAccountForPlanGroups(parameters.AccountID, GroupSubscriptionPlan);		
				}
				}
				public class GrantPaidAccessToGroupParameters 
		{
				public string GroupID ;
				public string AccountID ;
				}
		
		public class GrantPaidAccessToGroup 
		{
				private static void PrepareParameters(GrantPaidAccessToGroupParameters parameters)
		{
					}
				public static void Execute(GrantPaidAccessToGroupParameters parameters)
		{
						PrepareParameters(parameters);
					GrantPaidAccessToGroupImplementation.ExecuteMethod_AddAccountToGroup(parameters.GroupID, parameters.AccountID);		
				}
				}
				public class RevokePlanAccessFromAccountParameters 
		{
				public string PlanName ;
				public string AccountID ;
				}
		
		public class RevokePlanAccessFromAccount 
		{
				private static void PrepareParameters(RevokePlanAccessFromAccountParameters parameters)
		{
					}
				public static void Execute(RevokePlanAccessFromAccountParameters parameters)
		{
						PrepareParameters(parameters);
					GroupSubscriptionPlan GroupSubscriptionPlan = RevokePlanAccessFromAccountImplementation.GetTarget_GroupSubscriptionPlan(parameters.PlanName);	
				RevokePlanAccessFromAccountImplementation.ExecuteMethod_RevokeAccessFromAccountForPlanGroups(parameters.AccountID, GroupSubscriptionPlan);		
				}
				}
				public class RevokePaidAccessFromGroupParameters 
		{
				public string GroupID ;
				public string AccountID ;
				}
		
		public class RevokePaidAccessFromGroup 
		{
				private static void PrepareParameters(RevokePaidAccessFromGroupParameters parameters)
		{
					}
				public static void Execute(RevokePaidAccessFromGroupParameters parameters)
		{
						PrepareParameters(parameters);
					RevokePaidAccessFromGroupImplementation.ExecuteMethod_RemoveAccountFromGroup(parameters.GroupID, parameters.AccountID);		
				}
				}
		
		public class ProcessPayment 
		{
				public static void Execute()
		{
						
					INT.PaymentToken PaymentToken = ProcessPaymentImplementation.GetTarget_PaymentToken();	
				ProcessPaymentImplementation.ExecuteMethod_ValidateMatchingEmail(PaymentToken);		
				CustomerAccount CustomerAccount = ProcessPaymentImplementation.GetTarget_CustomerAccount();	
				ProcessPaymentImplementation.ExecuteMethod_ProcessPayment(PaymentToken, CustomerAccount);		
				}
				}
				public class FetchCustomersFromStripeParameters 
		{
				public string GroupID ;
				}
		
		public class FetchCustomersFromStripe 
		{
				private static void PrepareParameters(FetchCustomersFromStripeParameters parameters)
		{
					}
				public static void Execute(FetchCustomersFromStripeParameters parameters)
		{
						PrepareParameters(parameters);
					TheBall.CORE.IContainerOwner Owner = FetchCustomersFromStripeImplementation.GetTarget_Owner(parameters.GroupID);	
				Stripe.StripeCustomer[] StripeCustomers = FetchCustomersFromStripeImplementation.GetTarget_StripeCustomers();	
				CustomerAccountCollection CurrentCustomers = FetchCustomersFromStripeImplementation.GetTarget_CurrentCustomers(Owner);	
				CustomerAccount[] NewCustomersToCreate = FetchCustomersFromStripeImplementation.GetTarget_NewCustomersToCreate(Owner, StripeCustomers, CurrentCustomers);	
				FetchCustomersFromStripeImplementation.ExecuteMethod_StoreObjects(Owner, NewCustomersToCreate);		
				}
				}
				public class AssociatePaymentToGroupParameters 
		{
				public string GroupID ;
				public string PaymentID ;
				}
		
		public class AssociatePaymentToGroup 
		{
				private static void PrepareParameters(AssociatePaymentToGroupParameters parameters)
		{
					}
				public static void Execute(AssociatePaymentToGroupParameters parameters)
		{
						PrepareParameters(parameters);
					TheBall.CORE.IContainerOwner GroupAsOwner = AssociatePaymentToGroupImplementation.GetTarget_GroupAsOwner(parameters.GroupID);	
				}
				}
		 } 