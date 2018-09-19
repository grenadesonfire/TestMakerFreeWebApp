﻿using System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TestMakerFreeWebApp.ViewModels;
using System.Collections.Generic;
using TestMakerFreeWebApp.Data;
using System.Linq;
using Mapster;
using TestMakerFreeWebApp.Data.Models;

namespace TestMakerFreeWebApp.Controllers
{
    [Route("api/[controller]")]
    public class AnswerController : BaseApiController
    {
        #region Constructor
        public AnswerController(ApplicationDbContext context) : base(context)
        {
            
        }
        #endregion

        #region RESTful conventions methods
        /// <summary>
        /// Retrieves the Answer with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing Answer</param>
        /// <returns>the Answer with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var answer = DbContext.Answers.Where(i => i.Id == id).FirstOrDefault();

            // hanlde requests asking for non-existing answers
            if(answer == null)
            {
                return NotFound(
                    new
                    {
                        Error = $"Answer ID {id} has not been found",
                    });
            }

            return new JsonResult(
                answer.Adapt<AnswerViewModel>(),
                JsonSettings);
        }

        /// <summary>
        /// Adds a new Answer to the Database
        /// </summary>
        /// <param name="model">The AnswerViewModel containing the data to insert</param>
        [HttpPut]
        public IActionResult Put([FromBody]AnswerViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return new StatusCodeResult(500);

            // retrieve the answer to edit
            var answer =
                DbContext
                    .Answers
                    .Where(q => q.Id == model.Id)
                    .FirstOrDefault();

            // handle requests asking for non-existing answers
            if(answer == null)
            {
                return NotFound(
                    new
                    {
                        Error = $"Answer ID {model.Id} has not been found",
                    });
            }

            // handle the update (without object-mapping)
            answer.QuestionId = model.QuestionId;
            answer.Text = model.Text;
            answer.Value = model.Value;
            answer.Notes = model.Notes;

            answer.LastModifiedDate = DateTime.Now;

            DbContext.SaveChanges();

            // return the updated Quiz to the client.
            return new JsonResult(
                answer.Adapt<AnswerViewModel>(),
                JsonSettings);
        }

        /// <summary>
        /// Edit the Answer with the given {id}
        /// </summary>
        /// <param name="model">The AnswerViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody]AnswerViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return new StatusCodeResult(500);

            // map the ViewModel to the Model
            var answer = model.Adapt<Answer>();

            // override those properties
            //   that should be set from the server-side only
            answer.QuestionId = model.QuestionId;
            answer.Text = model.Text;
            answer.Notes = model.Notes;

            // properties set from server-side
            answer.CreatedDate = DateTime.Now;
            answer.LastModifiedDate = answer.CreatedDate;

            // add the new answer
            DbContext.Answers.Add(answer);
            // persist the changes into the Database.
            DbContext.SaveChanges();

            // return the newly-created Answer to the client.
            return new JsonResult(
                answer.Adapt<AnswerViewModel>(),
                JsonSettings);
        }

        /// <summary>
        /// Deletes the Answer with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Answer</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the answer from the Database
            var answer = DbContext.Answers.Where(i => i.Id == id).FirstOrDefault();

            // handle requests asking for non-existing answers
            if(answer == null)
            {
                return NotFound(
                    new
                    {
                        Error = $"Answer ID {id} has not been found",
                    });
            }

            // remove the quiz from the DbContext
            DbContext.Answers.Remove(answer);
            // persist the changes into the Database.
            DbContext.SaveChanges();

            // return an HTTP Status 200 (OK).
            return new OkResult();
        }
        #endregion

        #region Web MVC Methods
        // GET api/answer/all
        [HttpGet("All/{questionId}")]
        public IActionResult All(int questionId)
        {
            var answers = 
                DbContext
                    .Answers
                    .Where(q => q.QuestionId == questionId)
                    .ToArray();

            return new JsonResult(
                answers.Adapt<AnswerViewModel[]>(),
                JsonSettings);
        }
        #endregion
    }
}

