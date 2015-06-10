 

using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

		namespace TheBall.Payments { 
				public class GetAccountFromStripeCustomerParameters 
		{
				public string StripeCustomerID ;
				}
		
		public class GetAccountFromStripeCustomer 
		{
				private static void PrepareParameters(GetAccountFromStripeCustomerParameters parameters)
		{
					}
				public static GetAccountFromStripeCustomerReturnValue Execute(GetAccountFromStripeCustomerParameters parameters)
		{
						PrepareParameters(parameters);
					CustomerAccount[] AllCustomerAccounts = GetAccountFromStripeCustomerImplementation.GetTarget_AllCustomerAccounts();	
				CustomerAccount Account = GetAccountFromStripeCustomerImplementation.GetTarget_Account(parameters.StripeCustomerID, AllCustomerAccounts);	
				GetAccountFromStripeCustomerReturnValue returnValue = GetAccountFromStripeCustomerImplementation.Get_ReturnValue(Account);
		return returnValue;
				}
				}

		    public class GetAccountFromStripeCustomerReturnValue 
		{
				public CustomerAccount ResultAccount ;
				}
				public class ProcessStripeWebhookParameters 
		{
				public INT.StripeWebhookData JSONObject ;
				}
		
		public class ProcessStripeWebhook 
		{
				private static void PrepareParameters(ProcessStripeWebhookParameters parameters)
		{
					}
				public static void Execute(ProcessStripeWebhookParameters parameters)
		{
						PrepareParameters(parameters);
					string EventID = ProcessStripeWebhookImplementation.GetTarget_EventID(parameters.JSONObject);	
				Stripe.StripeEvent EventData = ProcessStripeWebhookImplementation.GetTarget_EventData(EventID);	
				ProcessStripeWebhookImplementation.ExecuteMethod_ProcessStripeEvent(EventData);		
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
			SyncEffectivePlanAccessesToAccountParameters operationParameters = ActivateAndPayGroupSubscriptionPlanImplementation.GrantAccessToPaidPlan_GetParameters(AccountID);
			SyncEffectivePlanAccessesToAccount.Execute(operationParameters);
									
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
			SyncEffectivePlanAccessesToAccountParameters operationParameters = CancelGroupSubscriptionPlanImplementation.RevokeAccessFromCanceledPlan_GetParameters(parameters.AccountID);
			SyncEffectivePlanAccessesToAccount.Execute(operationParameters);
									
		} // Local block closing
				}
				}
				public class SyncEffectivePlanAccessesToAccountParameters 
		{
				public string AccountID ;
				}
		
		public class SyncEffectivePlanAccessesToAccount 
		{
				private static void PrepareParameters(SyncEffectivePlanAccessesToAccountParameters parameters)
		{
					}
				public static void Execute(SyncEffectivePlanAccessesToAccountParameters parameters)
		{
						PrepareParameters(parameters);
					CustomerAccount Account = SyncEffectivePlanAccessesToAccountImplementation.GetTarget_Account(parameters.AccountID);	
				GroupSubscriptionPlan[] CurrentPlansBeforeSync = SyncEffectivePlanAccessesToAccountImplementation.GetTarget_CurrentPlansBeforeSync(Account);	
				GroupSubscriptionPlan[] ActivePlansFromStripe = SyncEffectivePlanAccessesToAccountImplementation.GetTarget_ActivePlansFromStripe(Account);	
				string[] GroupsToHaveAccessTo = SyncEffectivePlanAccessesToAccountImplementation.GetTarget_GroupsToHaveAccessTo(ActivePlansFromStripe);	
				string[] CurrentGroupAccesses = SyncEffectivePlanAccessesToAccountImplementation.GetTarget_CurrentGroupAccesses(CurrentPlansBeforeSync);	
				string[] GroupsToAddAccessTo = SyncEffectivePlanAccessesToAccountImplementation.GetTarget_GroupsToAddAccessTo(GroupsToHaveAccessTo, CurrentGroupAccesses);	
				string[] GroupsToRevokeAccessFrom = SyncEffectivePlanAccessesToAccountImplementation.GetTarget_GroupsToRevokeAccessFrom(GroupsToHaveAccessTo, CurrentGroupAccesses);	
				SyncEffectivePlanAccessesToAccountImplementation.ExecuteMethod_GrantAccessToGroups(parameters.AccountID, GroupsToAddAccessTo);		
				SyncEffectivePlanAccessesToAccountImplementation.ExecuteMethod_RevokeAccessFromGroups(parameters.AccountID, GroupsToRevokeAccessFrom);		
				SyncEffectivePlanAccessesToAccountImplementation.ExecuteMethod_SyncCurrentStripePlansToAccount(Account, CurrentPlansBeforeSync, ActivePlansFromStripe);		
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