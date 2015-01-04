using System.Collections.Specialized;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

		namespace TheBall.Payments { 
		
		public class ActivateAndPayGroupSubscriptionPlan 
		{
				public static void Execute()
		{
						
					INT.PaymentToken PaymentToken = ActivateAndPayGroupSubscriptionPlanImplementation.GetTarget_PaymentToken();	
				ActivateAndPayGroupSubscriptionPlanImplementation.ExecuteMethod_ValidateMatchingEmail(PaymentToken);		
				string AccountID = ActivateAndPayGroupSubscriptionPlanImplementation.GetTarget_AccountID();	
				CustomerAccount CustomerAccount = ActivateAndPayGroupSubscriptionPlanImplementation.GetTarget_CustomerAccount(AccountID);	
				string PlanName = ActivateAndPayGroupSubscriptionPlanImplementation.GetTarget_PlanName(PaymentToken);	
				ActivateAndPayGroupSubscriptionPlanImplementation.ExecuteMethod_ProcessPayment(PaymentToken, CustomerAccount);		
				ActivateAndPayGroupSubscriptionPlanImplementation.ExecuteMethod_GrantAccessToAccount(PlanName, AccountID);		
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
				CancelGroupSubscriptionPlanImplementation.ExecuteMethod_RevokeAccessFromAccount(parameters.PlanName, parameters.AccountID);		
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

		    public class RemovePlanAccessFromAccountParameters 
		{
				public string PlanName ;
				public string AccountID ;
				}
		
		public class RemovePlanAccessFromAccount 
		{
				private static void PrepareParameters(RemovePlanAccessFromAccountParameters parameters)
		{
					}
				public static void Execute(RemovePlanAccessFromAccountParameters parameters)
		{
						PrepareParameters(parameters);
					GroupSubscriptionPlan GroupSubscriptionPlan = RemovePlanAccessFromAccountImplementation.GetTarget_GroupSubscriptionPlan(parameters.PlanName);	
				RemovePlanAccessFromAccountImplementation.ExecuteMethod_RemoveAccessFromAccountForPlanGroups(parameters.AccountID, GroupSubscriptionPlan);		
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