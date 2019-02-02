 

using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

		namespace TheBall.Payments { 
				public class GetAccountFromStripeCustomerParameters 
		{
				public string StripeCustomerID ;
				public bool IsTestAccount ;
				}
		
		public class GetAccountFromStripeCustomer 
		{
				private static void PrepareParameters(GetAccountFromStripeCustomerParameters parameters)
		{
					}
				public static async Task<GetAccountFromStripeCustomerReturnValue> ExecuteAsync(GetAccountFromStripeCustomerParameters parameters)
		{
						PrepareParameters(parameters);
					CustomerAccount[] AllCustomerAccounts =  await GetAccountFromStripeCustomerImplementation.GetTarget_AllCustomerAccountsAsync();	
				CustomerAccount Account = GetAccountFromStripeCustomerImplementation.GetTarget_Account(parameters.StripeCustomerID, parameters.IsTestAccount, AllCustomerAccounts);	
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
				public static async Task ExecuteAsync(ProcessStripeWebhookParameters parameters)
		{
						PrepareParameters(parameters);
					string EventID = ProcessStripeWebhookImplementation.GetTarget_EventID(parameters.JSONObject);	
				bool IsTestMode = ProcessStripeWebhookImplementation.GetTarget_IsTestMode(parameters.JSONObject);	
				Stripe.StripeEvent EventData = ProcessStripeWebhookImplementation.GetTarget_EventData(EventID, IsTestMode);	
				 await ProcessStripeWebhookImplementation.ExecuteMethod_ProcessStripeEventAsync(EventData, IsTestMode);		
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
				public class CancelAccountPlanParameters 
		{
				public INT.CancelSubscriptionParams CancelParameters ;
				}
		
		public class CancelAccountPlan 
		{
				private static void PrepareParameters(CancelAccountPlanParameters parameters)
		{
					}
				public static async Task ExecuteAsync(CancelAccountPlanParameters parameters)
		{
						PrepareParameters(parameters);
					string AccountID = CancelAccountPlanImplementation.GetTarget_AccountID();	
				CustomerAccount CustomerAccount =  await CancelAccountPlanImplementation.GetTarget_CustomerAccountAsync(AccountID);	
				string StripeCustomerID = CancelAccountPlanImplementation.GetTarget_StripeCustomerID(CustomerAccount);	
				bool IsTestMode = CancelAccountPlanImplementation.GetTarget_IsTestMode(CustomerAccount);	
				 await CancelAccountPlanImplementation.ExecuteMethod_RemoveCustomerPaymentSourceAsync(StripeCustomerID, IsTestMode);		
				string PlanName = CancelAccountPlanImplementation.GetTarget_PlanName(parameters.CancelParameters);	
				Stripe.StripeSubscription[] CustomersActiveSubscriptions =  await CancelAccountPlanImplementation.GetTarget_CustomersActiveSubscriptionsAsync(StripeCustomerID, IsTestMode);	
				 await CancelAccountPlanImplementation.ExecuteMethod_CancelSubscriptionAtPeriodEndAsync(StripeCustomerID, IsTestMode, PlanName, CustomersActiveSubscriptions);		
				 await CancelAccountPlanImplementation.ExecuteMethod_StoreObjectsAsync(CustomerAccount);		
				}
				}
				public class PurchaseProductParameters 
		{
				public INT.ProductPurchaseInfo PurchaseInfo ;
				}
		
		public class PurchaseProduct 
		{
				private static void PrepareParameters(PurchaseProductParameters parameters)
		{
					}
				public static async Task ExecuteAsync(PurchaseProductParameters parameters)
		{
						PrepareParameters(parameters);
					string AccountID = PurchaseProductImplementation.GetTarget_AccountID();	
				CustomerAccount CustomerAccount =  await PurchaseProductImplementation.GetTarget_CustomerAccountAsync(AccountID);	
				string StripeCustomerID = PurchaseProductImplementation.GetTarget_StripeCustomerID(CustomerAccount);	
				bool IsTestMode = PurchaseProductImplementation.GetTarget_IsTestMode(CustomerAccount);	
				string ProductName = PurchaseProductImplementation.GetTarget_ProductName(parameters.PurchaseInfo);	
				double ProductPrice = PurchaseProductImplementation.GetTarget_ProductPrice(parameters.PurchaseInfo);	
				string Currency = PurchaseProductImplementation.GetTarget_Currency(parameters.PurchaseInfo);	
				 await PurchaseProductImplementation.ExecuteMethod_ValidateStripeProductAndPriceAsync(ProductName, ProductPrice, IsTestMode);		
				 await PurchaseProductImplementation.ExecuteMethod_ProcessPaymentAsync(parameters.PurchaseInfo, StripeCustomerID, IsTestMode, ProductName, ProductPrice, Currency);		
				}
				}
				public class ActivateAccountPlanParameters 
		{
				public INT.PaymentToken PaymentToken ;
				}
		
		public class ActivateAccountPlan 
		{
				private static void PrepareParameters(ActivateAccountPlanParameters parameters)
		{
					}
				public static async Task ExecuteAsync(ActivateAccountPlanParameters parameters)
		{
						PrepareParameters(parameters);
					ActivateAccountPlanImplementation.ExecuteMethod_ValidateMatchingEmail(parameters.PaymentToken);		
				string AccountID = ActivateAccountPlanImplementation.GetTarget_AccountID();	
				bool IsTokenTestMode = ActivateAccountPlanImplementation.GetTarget_IsTokenTestMode(parameters.PaymentToken);	
				bool IsTestAccount =  await ActivateAccountPlanImplementation.GetTarget_IsTestAccountAsync(AccountID);	
				bool IsTestMode = ActivateAccountPlanImplementation.GetTarget_IsTestMode(IsTokenTestMode, IsTestAccount);	
				CustomerAccount CustomerAccount =  await ActivateAccountPlanImplementation.GetTarget_CustomerAccountAsync(AccountID, IsTestMode);	
				 await ActivateAccountPlanImplementation.ExecuteMethod_UpdateStripeCustomerDataAsync(parameters.PaymentToken, CustomerAccount, IsTestMode);		
				string StripeCustomerID = ActivateAccountPlanImplementation.GetTarget_StripeCustomerID(CustomerAccount);	
				string PlanName = ActivateAccountPlanImplementation.GetTarget_PlanName(parameters.PaymentToken);	
				 await ActivateAccountPlanImplementation.ExecuteMethod_ValidateStripePlanNameAsync(PlanName, IsTestMode);		
				Stripe.StripeSubscription[] CustomersActiveSubscriptions =  await ActivateAccountPlanImplementation.GetTarget_CustomersActiveSubscriptionsAsync(StripeCustomerID, IsTestMode);	
				 await ActivateAccountPlanImplementation.ExecuteMethod_ProcessPaymentAsync(parameters.PaymentToken, StripeCustomerID, IsTestMode, PlanName, CustomersActiveSubscriptions);		
				 await ActivateAccountPlanImplementation.ExecuteMethod_StoreObjectsAsync(CustomerAccount);		
				
		{ // Local block to allow local naming
			SyncEffectivePlanAccessesToAccountParameters operationParameters = ActivateAccountPlanImplementation.GrantAccessToPaidPlan_GetParameters(AccountID);
			 await SyncEffectivePlanAccessesToAccount.ExecuteAsync(operationParameters);
									
		} // Local block closing
				}
				}
		
		public class ActivateAndPayGroupSubscriptionPlan 
		{
				public static async Task ExecuteAsync()
		{
						
					INT.PaymentToken PaymentToken = ActivateAndPayGroupSubscriptionPlanImplementation.GetTarget_PaymentToken();	
				ActivateAndPayGroupSubscriptionPlanImplementation.ExecuteMethod_ValidateMatchingEmail(PaymentToken);		
				string AccountID = ActivateAndPayGroupSubscriptionPlanImplementation.GetTarget_AccountID();	
				CustomerAccount CustomerAccount =  await ActivateAndPayGroupSubscriptionPlanImplementation.GetTarget_CustomerAccountAsync(AccountID);	
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
			 await SyncEffectivePlanAccessesToAccount.ExecuteAsync(operationParameters);
									
		} // Local block closing
				}
				}
		
		public class CancelGroupSubscriptionPlan 
		{
				public static async Task ExecuteAsync()
		{
						
					INT.CancelSubscriptionParams CancelParams = CancelGroupSubscriptionPlanImplementation.GetTarget_CancelParams();	
				string PlanName = CancelGroupSubscriptionPlanImplementation.GetTarget_PlanName(CancelParams);	
				string AccountID = CancelGroupSubscriptionPlanImplementation.GetTarget_AccountID();	
				CustomerAccount CustomerAccount =  await CancelGroupSubscriptionPlanImplementation.GetTarget_CustomerAccountAsync(AccountID);	
				CancelGroupSubscriptionPlanImplementation.ExecuteMethod_CancelSubscriptionPlan(PlanName, CustomerAccount);		
				
		{ // Local block to allow local naming
			SyncEffectivePlanAccessesToAccountParameters operationParameters = CancelGroupSubscriptionPlanImplementation.RevokeAccessFromCanceledPlan_GetParameters(AccountID);
			 await SyncEffectivePlanAccessesToAccount.ExecuteAsync(operationParameters);
									
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
				public static async Task ExecuteAsync(SyncEffectivePlanAccessesToAccountParameters parameters)
		{
						PrepareParameters(parameters);
					CustomerAccount Account =  await SyncEffectivePlanAccessesToAccountImplementation.GetTarget_AccountAsync(parameters.AccountID);	
				bool IsTestMode = SyncEffectivePlanAccessesToAccountImplementation.GetTarget_IsTestMode(Account);	
				GroupSubscriptionPlan[] CurrentPlansBeforeSync =  await SyncEffectivePlanAccessesToAccountImplementation.GetTarget_CurrentPlansBeforeSyncAsync(Account);	
				INT.PlanStatus[] ActivePlanStatusesFromStripe =  await SyncEffectivePlanAccessesToAccountImplementation.GetTarget_ActivePlanStatusesFromStripeAsync(Account, IsTestMode);	
				GroupSubscriptionPlan[] ActivePlansFromStripe = SyncEffectivePlanAccessesToAccountImplementation.GetTarget_ActivePlansFromStripe(ActivePlanStatusesFromStripe);	
				string[] GroupsToHaveAccessTo = SyncEffectivePlanAccessesToAccountImplementation.GetTarget_GroupsToHaveAccessTo(ActivePlansFromStripe);	
				string[] CurrentGroupAccesses = SyncEffectivePlanAccessesToAccountImplementation.GetTarget_CurrentGroupAccesses(CurrentPlansBeforeSync);	
				string[] GroupsToAddAccessTo = SyncEffectivePlanAccessesToAccountImplementation.GetTarget_GroupsToAddAccessTo(GroupsToHaveAccessTo, CurrentGroupAccesses);	
				string[] GroupsToRevokeAccessFrom = SyncEffectivePlanAccessesToAccountImplementation.GetTarget_GroupsToRevokeAccessFrom(GroupsToHaveAccessTo, CurrentGroupAccesses);	
				SyncEffectivePlanAccessesToAccountImplementation.ExecuteMethod_GrantAccessToGroups(parameters.AccountID, GroupsToAddAccessTo);		
				SyncEffectivePlanAccessesToAccountImplementation.ExecuteMethod_RevokeAccessFromGroups(parameters.AccountID, GroupsToRevokeAccessFrom);		
				 await SyncEffectivePlanAccessesToAccountImplementation.ExecuteMethod_SyncCurrentStripePlansToAccountAsync(Account, CurrentPlansBeforeSync, ActivePlansFromStripe);		
				 await SyncEffectivePlanAccessesToAccountImplementation.ExecuteMethod_UpdateStatusesOnAccountAsync(parameters.AccountID, ActivePlanStatusesFromStripe);		
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
				public static async Task ExecuteAsync()
		{
						
					INT.PaymentToken PaymentToken = ProcessPaymentImplementation.GetTarget_PaymentToken();	
				ProcessPaymentImplementation.ExecuteMethod_ValidateMatchingEmail(PaymentToken);		
				CustomerAccount CustomerAccount =  await ProcessPaymentImplementation.GetTarget_CustomerAccountAsync();	
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
				public static async Task ExecuteAsync(FetchCustomersFromStripeParameters parameters)
		{
						PrepareParameters(parameters);
					TheBall.CORE.IContainerOwner Owner = FetchCustomersFromStripeImplementation.GetTarget_Owner(parameters.GroupID);	
				Stripe.StripeCustomer[] StripeCustomers = FetchCustomersFromStripeImplementation.GetTarget_StripeCustomers();	
				CustomerAccountCollection CurrentCustomers =  await FetchCustomersFromStripeImplementation.GetTarget_CurrentCustomersAsync(Owner);	
				CustomerAccount[] NewCustomersToCreate = FetchCustomersFromStripeImplementation.GetTarget_NewCustomersToCreate(Owner, StripeCustomers, CurrentCustomers);	
				 await FetchCustomersFromStripeImplementation.ExecuteMethod_StoreObjectsAsync(Owner, NewCustomersToCreate);		
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