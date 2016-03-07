 

using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

		namespace ProBroz.OnlineTraining { 
				public class CreateMemberParameters 
		{
				public INT.Member MemberData ;
				}
		
		public class CreateMember 
		{
				private static void PrepareParameters(CreateMemberParameters parameters)
		{
					}
				public static async Task ExecuteAsync(CreateMemberParameters parameters)
		{
						PrepareParameters(parameters);
					Member MemberToCreate = CreateMemberImplementation.GetTarget_MemberToCreate(parameters.MemberData);	
				 await CreateMemberImplementation.ExecuteMethod_StoreObjectAsync(MemberToCreate);		
				}
				}
				public class SubscribeToPlanParameters 
		{
				public string MemberID ;
				public string PlanID ;
				public string PaymentOptionID ;
				}
		
		public class SubscribeToPlan 
		{
				private static void PrepareParameters(SubscribeToPlanParameters parameters)
		{
					}
				public static async Task ExecuteAsync(SubscribeToPlanParameters parameters)
		{
						PrepareParameters(parameters);
					Member Member =  await SubscribeToPlanImplementation.GetTarget_MemberAsync(parameters.MemberID);	
				MembershipPlan Plan =  await SubscribeToPlanImplementation.GetTarget_PlanAsync(parameters.PlanID);	
				PaymentOption PaymentOption =  await SubscribeToPlanImplementation.GetTarget_PaymentOptionAsync(parameters.PaymentOptionID, Plan);	
				}
				}
		} 