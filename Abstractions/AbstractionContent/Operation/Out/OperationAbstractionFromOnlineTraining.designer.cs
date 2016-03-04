 

using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

		namespace ProBroz.OnlineTraining { 
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
				public static void Execute(SubscribeToPlanParameters parameters)
		{
						PrepareParameters(parameters);
					Member Member = SubscribeToPlanImplementation.GetTarget_Member(parameters.MemberID);	
				MembershipPlan Plan = SubscribeToPlanImplementation.GetTarget_Plan(parameters.PlanID);	
				PaymentOption PaymentOption = SubscribeToPlanImplementation.GetTarget_PaymentOption(parameters.PaymentOptionID, Plan);	
				}
				}
		 } 