﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MSITTeam1.Models;
using MSITTeam1.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSITTeam1.Controllers
{
	public class QuestionBankController : Controller
	{
		private readonly helloContext _context;
		public QuestionBankController(helloContext context)
		{
			_context = context;
		}

		public IActionResult List()
		{
			return View();
		}
		[HttpPost]
		public IActionResult List([FromBody] CQuestionQueryViewModel query)
		{
			ViewBag.Name = CDictionary.username;
			ViewBag.Type = CDictionary.memtype;
			ViewBag.account = CDictionary.account;

			return ViewComponent("QuestionBankList", new { keyword = query.keyword, Subjects = query.Subjects, Type = query.Type });
		}

		public IActionResult MyQuesList([FromBody] CQuestionQueryViewModel query)
		{
			return ViewComponent("QMyQuestionList");
		}
		private IQueryable<CQuestionBankViewModel> FilterByType(IQueryable<CQuestionBankViewModel> table, string level)
		{
			if (!string.IsNullOrEmpty(level))
			{
				int tempLevel = int.Parse(level);
				return table.Where(q => q.FLevel == tempLevel);
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

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Create([FromBody] CQuestionBankViewModel newques)
		{
			if (CDictionary.account != null)
			{
				newques.FSubmitterId = CDictionary.account;
			}
			else
			{
				return RedirectToAction("Index", "Student_Login");
			}

			TQuestionList quesQuery = _context.TQuestionLists.FirstOrDefault(q => q.FSubjectId.Equals(newques.FSubjectId));
			if (quesQuery == null)
			{
				newques.FQuestionId = 1;
				newques.FCQuestionId = 1;
			}
			else
			{
				var searchLastId = from q in _context.TQuestionLists
								   where q.FSubjectId.Equals(newques.FSubjectId)
								   orderby q.FQuestionId descending
								   select q;

				int lastId = searchLastId.First().FQuestionId;
				newques.FQuestionId = lastId + 1;
			}

			newques.FState = 1;
			_context.TQuestionLists.Add(newques.question);
			if (newques.question.FQuestionId >= 1 && !string.IsNullOrEmpty(newques.question.FSubjectId))
			{
				_context.TQuestionLists.Add(newques.question);
				foreach (var ans in newques.FChoiceList)
				{
					newques.FCSubjectId = newques.FSubjectId;
					newques.FCQuestionId = newques.FQuestionId;
					newques.FChoice = ans.Fchoice;
					newques.FCorrectAnswer = ans.FCorrect;
					newques.FSn = 0;
					_context.TQuestionDetails.Add(newques.choice);
					_context.SaveChanges();
				}
				_context.SaveChanges();
			}
			return Content("新增成功");
		}

		public IActionResult DetailByVC([FromBody] CQuestionQueryViewModel query)
		{
			int questionID = 0;
			if (query != null)
			{
				questionID = Convert.ToInt32(query.questionID);
			}
			return ViewComponent("QuestionBankDetail", new { subjectID = query.Subjects, questionID = questionID });
		}

		public IActionResult EditByVC([FromBody] CQuestionQueryViewModel query)
		{
			int questionID = 0;
			if (query != null)
			{
				questionID = Convert.ToInt32(query.questionID);
			}
			return ViewComponent("QuestionBankEdit", new { subjectID = query.Subjects, questionID = questionID });
		}

		[HttpPost]
		public IActionResult Edit([FromBody] CQuestionBankViewModel quesList)
		{
			if (quesList != null)
			{
				string subject = quesList.FSubjectId;
				int questionId = quesList.FQuestionId;
				List<int> editChoiceSnList = new List<int>();
				List<string> newChoiceStrList = new List<string>();

				TQuestionList quesSel = _context.TQuestionLists.FirstOrDefault(q => q.FSubjectId.Equals(subject) && q.FQuestionId == questionId);
				TQuestionDetail choSel = null;
				var tchoSel = _context.TQuestionDetails.Where(c => c.FSubjectId.Equals(subject) && c.FQuestionId == questionId);

				if (quesSel != null)
				{
					quesSel.FQuestion = quesList.FQuestion;
					quesSel.FQuestionTypeId = Convert.ToInt32(quesList.FQuestionTypeId);
					quesSel.FLevel = quesList.FLevel;
					foreach (var ans in quesList.FChoiceList)
					{
						if (ans.FSN != 0)
						{
							// 原有選項修改
							choSel = _context.TQuestionDetails.FirstOrDefault(c => c.FSn == ans.FSN);
							choSel.FChoice = ans.Fchoice;
							choSel.FCorrectAnswer = ans.FCorrect;
							editChoiceSnList.Add(ans.FSN);
						}
						else
						{
							// 新增選項
							quesList.FCSubjectId = quesList.FSubjectId;
							quesList.FCQuestionId = quesList.FQuestionId;
							quesList.FChoice = ans.Fchoice;
							quesList.FCorrectAnswer = ans.FCorrect;
							quesList.FSn = 0;
							_context.TQuestionDetails.Add(quesList.choice);
							newChoiceStrList.Add(quesList.FChoice);
						}
						_context.SaveChanges();
					}
				}

				foreach (var t in tchoSel)
				{
					// 判斷被刪除的選項
					bool isContainSn = editChoiceSnList.Contains(t.FSn);
					bool isContainStr = newChoiceStrList.Contains(t.FChoice);
					if (!isContainSn && !isContainStr)
					{
						_context.TQuestionDetails.Remove(t);
					}
				}
				_context.SaveChanges();
			}
			return Content("修改完成");
		}

		public IActionResult Delete(string subjectID, int questionID)
		{
			if (subjectID != null && questionID > 0)
			{
				var paperTable = _context.TTestPapers.Where(quesInPaper => quesInPaper.FSubjectId.Equals(subjectID) && quesInPaper.FQuestionId == questionID);
				if (paperTable.Count() > 0)
				{
					return Content("當前有試卷引用此題，無法刪除", "text/plain", System.Text.Encoding.UTF8);
				}
				var ques = _context.TQuestionLists.FirstOrDefault(q => q.FSubjectId.Equals(subjectID) && q.FQuestionId == questionID);
				if (ques != null)
				{
					ques.FState = 0;
					_context.SaveChanges();
					return Content("刪除成功", "text/plain", System.Text.Encoding.UTF8);
				}
			}
			return Content("刪除成功");
		}

		public IActionResult DeleteTest(string subjectID, int questionID)
		{
			return Content($"測試成功，技能名稱{subjectID} 題目編號{questionID}", "text/plain", System.Text.Encoding.UTF8);
		}

		public IActionResult Subject()
		{
			var subjects = _context.TStudioInformations.Select(s => new
			{
				s.FClassCategory
			}).Distinct().OrderBy(s => s.FClassCategory);
			return Json(subjects);
		}
		public IActionResult SubjectExistQuestion()
		{
			var subject = _context.TQuestionLists.Where(s => s.FState == 2 ||
			(s.FState == 1 && s.FSubmitterId == CDictionary.account)).Select(s => new
			{
				s.FSubjectId
			}).Distinct().OrderBy(s => s.FSubjectId);
			return Json(subject);
		}

		public IActionResult TypeExist(string subjectID)
		{
			var type = _context.TQuestionLists.Where(t => t.FSubjectId == subjectID).Select(t => new
			{
				t.FQuestionTypeId
			}).Distinct();
			return Json(type);
		}

		public IActionResult Classes()
		{
			var subjects = _context.TStudioInformations.Select(s => new
			{
				s.FClassSkill
			}).Distinct().OrderBy(s => s.FClassSkill);
			return Json(subjects);
		}
	}
}
