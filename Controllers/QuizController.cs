﻿using System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TestMakerFreeWebApp.ViewModels;
using System.Collections.Generic;
using System.Linq;
using TestMakerFreeWebApp.Data;
using Mapster;
using TestMakerFreeWebApp.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace TestMakerFreeWebApp.Controllers
{
    [Route("api/[controller]")]
    public class QuizController : BaseApiController
    {
        #region Constructor
        public QuizController(
            ApplicationDbContext adc,
            RoleManager<IdentityRole> rm,
            UserManager<ApplicationUser> um,
            IConfiguration config) : base(adc, rm, um, config)
        {

        }
        #endregion
        #region RESTful conventions methods
        /// <summary>
        /// GET: api/quiz/{id}
        /// Retrieves the Quiz with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing Quiz</param>
        /// <returns>the Quiz with the given {id}</returns>
        [HttpGet("{id}")]
        [Authorize]
        public IActionResult Get(int id)
        {
            // create a sample quiz to match the given request
            var v = DbContext.Quizzes.Where(i => i.Id == id).FirstOrDefault();

            //handle requests asking for non-existing quizzes
            if(v == null)
            {
                return NotFound(new
                {
                    Error = $"Quiz ID {id} has not been found!",
                });
            }

            // output the result in JSON format
            return new JsonResult(
                v.Adapt<QuizViewModel>(),
                JsonSettings);
        }

        /// <summary>
        /// Adds a new Quiz to the Database
        /// </summary>
        /// <param name="model">The QuizViewModel containing the data to insert</param>
        [HttpPut]
        [Authorize]
        public IActionResult Put([FromBody]QuizViewModel model)
        {
            if (model == null) return new StatusCodeResult(500);

            var quiz = DbContext.Quizzes.Where(q => q.Id == model.Id).FirstOrDefault();

            if(quiz == null)
            {
                return NotFound(new
                {
                    Error = $"Quiz ID {model.Id} has not been found",
                });
            }

            quiz.Title = model.Title;
            quiz.Description = model.Description;
            quiz.Text = model.Text;
            quiz.Notes = model.Notes;

            quiz.LastModifiedDate = DateTime.Now;

            quiz.UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            DbContext.SaveChanges();

            return new JsonResult(
                quiz.Adapt<QuizViewModel>(),
                JsonSettings);
        }

        /// <summary>
        /// Edit the Quiz with the given {id}
        /// </summary>
        /// <param name="model">The QuizViewModel containing the data to update</param>
        [HttpPost]
        [Authorize]
        public IActionResult Post([FromBody]QuizViewModel model)
        {
            if (model == null) return new StatusCodeResult(500);

            var quiz = new Quiz();

            quiz.Title = model.Title;
            quiz.Description = model.Description;
            quiz.Text = model.Text;
            quiz.Notes = model.Notes;

            quiz.CreatedDate = DateTime.Now;
            quiz.LastModifiedDate = quiz.CreatedDate;

            quiz.UserId = DbContext.Users.Where(u => u.UserName == "Admin").FirstOrDefault().Id;

            DbContext.Quizzes.Add(quiz);
            DbContext.SaveChanges();

            return new JsonResult(
                    quiz.Adapt<QuizViewModel>(),
                    JsonSettings);
        }

        /// <summary>
        /// Deletes the Quiz with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Test</param>
        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete(int id)
        {
            var quiz = DbContext.Quizzes.Where(q => q.Id == id).FirstOrDefault();

            if (quiz == null)
            {
                return NotFound(new
                {
                    Error = $"Quiz ID {id} has not been found",
                });
            }

            DbContext.Quizzes.Remove(quiz);
            DbContext.SaveChanges();

            //return new OkResult();

            //Changed the result type to correctly give a parseable object back with the quiz id and remnant information.
            // OkResult would not get handled by the angular api calls correctly as it was expecting a quiz json object.
            return new JsonResult(
                quiz.Adapt<QuizViewModel>(),
                JsonSettings);
        }
        #endregion

        #region Attribute-based routing methods
        /// <summary>
        /// GET: api/quiz/latest
        /// Retrieves the {num} latest Quizzes
        /// </summary>
        /// <param name="num">the number of quizzes to retrieve</param>
        /// <returns>the {num} latest Quizzes</returns>
        [HttpGet("Latest/{num:int?}")]
        public IActionResult Latest(int num = 10)
        {
            var latest = DbContext.Quizzes.OrderByDescending(q => q.CreatedDate).Take(num).ToArray();

            // output the result in JSON format
            return new JsonResult(
                latest.Adapt<QuizViewModel[]>(),
                JsonSettings);
        }

        /// <summary>
        /// GET: api/quiz/ByTitle
        /// Retrieves the {num} Quizzes sorted by Title (A to Z)
        /// </summary>
        /// <param name="num">the number of quizzes to retrieve</param>
        /// <returns>{num} Quizzes sorted by Title</returns>
        [HttpGet("ByTitle/{num:int?}")]
        public IActionResult ByTitle(int num = 10)
        {
            var byTitle = DbContext.Quizzes.OrderBy(q => q.Title).Take(num).ToArray();

            return new JsonResult(
                byTitle.Adapt<QuizViewModel[]>(),
                JsonSettings);
        }

        /// <summary>
        /// GET: api/quiz/mostViewed
        /// Retrieves the {num} random Quizzes
        /// </summary>
        /// <param name="num">the number of quizzes to retrieve</param>
        /// <returns>{num} random Quizzes</returns>
        [HttpGet("Random/{num:int?}")]
        public IActionResult Random(int num = 10)
        {
            var random = DbContext.Quizzes.OrderBy(q => Guid.NewGuid()).Take(num).ToArray();

            return new JsonResult(
                random.Adapt<QuizViewModel[]>(),
                JsonSettings);
        }
        #endregion
    }
}
