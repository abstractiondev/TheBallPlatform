 

using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

		namespace Footvoter.Services { 
				public class UpdateUserProfileParameters 
		{
				public INT.UserProfile ProfileData ;
				}
		
		public class UpdateUserProfile 
		{
				private static void PrepareParameters(UpdateUserProfileParameters parameters)
		{
					}
				public static async Task ExecuteAsync(UpdateUserProfileParameters parameters)
		{
						PrepareParameters(parameters);
					INT.UserProfile UserProfile = UpdateUserProfileImplementation.GetTarget_UserProfile();	
				UpdateUserProfileImplementation.ExecuteMethod_SetUserProfileFields(UserProfile);		
				 await UpdateUserProfileImplementation.ExecuteMethod_StoreObjectsAsync(UserProfile);		
				}
				}
				public class DoVoteParameters 
		{
				public INT.VoteData VoteData ;
				}
		
		public class DoVote 
		{
				private static void PrepareParameters(DoVoteParameters parameters)
		{
					}
				public static async Task ExecuteAsync(DoVoteParameters parameters)
		{
						PrepareParameters(parameters);
					INT.VotingSummary UserVotedSummary = DoVoteImplementation.GetTarget_UserVotedSummary();	
				DoVoteImplementation.ExecuteMethod_PerformVoting(parameters.VoteData, UserVotedSummary);		
				 await DoVoteImplementation.ExecuteMethod_StoreObjectsAsync(UserVotedSummary);		
				}
				}
				public class SetCompanyFollowParameters 
		{
				public INT.CompanyFollowData FollowDataInput ;
				}
		
		public class SetCompanyFollow 
		{
				private static void PrepareParameters(SetCompanyFollowParameters parameters)
		{
					}
				public static async Task ExecuteAsync(SetCompanyFollowParameters parameters)
		{
						PrepareParameters(parameters);
					INT.CompanyFollowData FollowData = SetCompanyFollowImplementation.GetTarget_FollowData();	
				SetCompanyFollowImplementation.ExecuteMethod_SetCompanyFollowData(parameters.FollowDataInput, FollowData);		
				 await SetCompanyFollowImplementation.ExecuteMethod_StoreObjectsAsync(FollowData);		
				}
				}
				public class GetCompaniesParameters 
		{
				public INT.CompanySearchCriteria CompanySearchCriteria ;
				}
		
		public class GetCompanies 
		{
				private static void PrepareParameters(GetCompaniesParameters parameters)
		{
					}
				public static async Task ExecuteAsync(GetCompaniesParameters parameters)
		{
						PrepareParameters(parameters);
					 await GetCompaniesImplementation.ExecuteMethod_PerformSearchAsync(parameters.CompanySearchCriteria);		
				}
				}
		 } 