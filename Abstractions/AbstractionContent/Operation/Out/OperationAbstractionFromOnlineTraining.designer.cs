 

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
				public class SaveMemberParameters 
		{
				public INT.Member MemberData ;
				}
		
		public class SaveMember 
		{
				private static void PrepareParameters(SaveMemberParameters parameters)
		{
					}
				public static async Task ExecuteAsync(SaveMemberParameters parameters)
		{
						PrepareParameters(parameters);
					Member MemberToSave =  await SaveMemberImplementation.GetTarget_MemberToSaveAsync(parameters.MemberData);	
				 await SaveMemberImplementation.ExecuteMethod_StoreObjectAsync(MemberToSave);		
				}
				}
				public class DeleteMemberParameters 
		{
				public INT.Member MemberData ;
				}
		
		public class DeleteMember 
		{
				private static void PrepareParameters(DeleteMemberParameters parameters)
		{
					}
				public static async Task ExecuteAsync(DeleteMemberParameters parameters)
		{
						PrepareParameters(parameters);
					Member MemberToDelete =  await DeleteMemberImplementation.GetTarget_MemberToDeleteAsync(parameters.MemberData);	
				 await DeleteMemberImplementation.ExecuteMethod_DeleteObjectAsync(MemberToDelete);		
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