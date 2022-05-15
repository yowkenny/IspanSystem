﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using MSITTeam1.Models;
using MSITTeam1.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace MSITTeam1.ViewComponent
{
	[Microsoft.AspNetCore.Mvc.ViewComponent]
	public class QuestionBankListViewComponent : Microsoft.AspNetCore.Mvc.ViewComponent
	{
		private readonly helloContext _context;

		[ActivatorUtilitiesConstructor]
		public QuestionBankListViewComponent(helloContext context)
		{
			_context = context;
		}
		public IViewComponentResult Invoke(string keyword, string Subjects, string Type)
		{
			ViewBag.Name = CDictionary.username;
			ViewBag.Type = CDictionary.memtype;
			ViewBag.account = CDictionary.account;

			// 從資料庫讀取題目
			List<CQuestionBankViewModel> quesList = new List<CQuestionBankViewModel>();
			IQueryable<CQuestionBankViewModel> quesQuery = from choice in _context.TQuestionDetails
														   join ques in _context.TQuestionLists on new { choice.FSubjectId, choice.FQuestionId } equals new { ques.FSubjectId, ques.FQuestionId }
														   where ques.FState == 2 || (ques.FState == 1 && ques.FSubmitterId == CDictionary.account)
														   orderby ques.FUpdateTime/*,ques.FSubjectId,ques.FQuestionId*/ descending
														   select new CQuestionBankViewModel
														   {
															   FSn = choice.FSn,
															   FCSubjectId = choice.FSubjectId,
															   FSubjectId = ques.FSubjectId,
															   FCQuestionId = choice.FQuestionId,
															   FQuestionId = ques.FQuestionId,
															   FQuestion = ques.FQuestion,
															   FLevel = ques.FLevel,
															   FQuestionTypeId = ques.FQuestionTypeId,
															   FChoice = choice.FChoice,
															   FCorrectAnswer = choice.FCorrectAnswer
														   };
		
			// 篩選題目
			quesQuery = this.FilterByClass(quesQuery, Subjects);
			quesQuery = this.FilterByKeyWord(quesQuery, keyword);
			quesQuery = this.FilterByLevel(quesQuery, Type);

			foreach (var q in quesQuery)
			{
				quesList.Add(q);
			}
			return View(quesList);
		}
		private IQueryable<CQuestionBankViewModel> FilterByLevel(IQueryable<CQuestionBankViewModel> table, string questype)
		{
			if (!string.IsNullOrEmpty(questype))
			{
				int tempType = int.Parse(questype);
				return table.Where(q => q.FQuestionTypeId == tempType);
			}
			else
			{
				return table;
			}
		}

		private IQueryable<CQuestionBankViewModel> FilterByKeyWord(IQueryable<CQuestionBankViewModel> table, string keyword)
		{
			if (!string.IsNullOrEmpty(keyword))
			{
				return table.Where(q =>
						q.FQuestion.Contains(keyword)
						|| q.FChoice.Contains(keyword)
						|| q.FSubjectId.Contains(keyword));
			}
			else
			{
				return table;
			}
		}

		private IQueryable<CQuestionBankViewModel> FilterByClass(IQueryable<CQuestionBankViewModel> table, string className)
		{
			if (!string.IsNullOrEmpty(className))
			{
				return table.Where(q => q.FSubjectId.Trim().Equals(className.Trim()));
			}
			else
			{
				return table;
			}
		}
	}
}
