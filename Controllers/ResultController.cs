using System;
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
    public class ResultController : BaseApiController
    {
        #region Constructor
        public ResultController(ApplicationDbContext context) : base(context)
        {
            
        }
        #endregion

        #region RESTful conventions methods
        /// <summary>
        /// Retrieves the Result with the given {id}
        /// </summary>
        /// <param name="id">The ID of an existing Result</param>
        /// <returns>the Result with the given {id}</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var result = DbContext.Results.Where(i => i.Id == id).FirstOrDefault();

            // handle requests asking for non-existing results
            if(result == null)
            {
                return NotFound(
                    new
                    {
                        Error = $"Result ID {id} has not been found",
                    });
            }

            return new JsonResult(
                result.Adapt<ResultViewModel>(),
                JsonSettings);
        }

        /// <summary>
        /// Adds a new Result to the Database
        /// </summary>
        /// <param name="model">The ResultViewModel containing the data to insert</param>
        [HttpPut]
        public IActionResult Put([FromBody]ResultViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if client payload is invalid
            if (model == null) return new StatusCodeResult(500);

            // retrieve the result to edit
            var result = DbContext.Results.Where(q => q.Id == model.Id).FirstOrDefault();

            // handle requests asking for non-existing results
            if(result == null)
            {
                return NotFound(
                    new
                    {
                        Error = $"Result ID {model.Id} has not been found",
                    });
            }

            // handle the update (without object-mapping)
            result.QuizId = model.QuizId;
            result.Text = model.Text;
            result.MinValue = model.MinValue;
            result.MaxValue = model.MaxValue;
            result.Notes = model.Notes;

            // properties set from server-side
            result.LastModifiedDate = DateTime.Now;

            // persist the changes into the Database.
            DbContext.SaveChanges();

            // return the updated Quiz to the client
            return new JsonResult(
                result.Adapt<ResultViewModel>(),
                JsonSettings);
        }

        /// <summary>
        /// Edit the Result with the given {id}
        /// </summary>
        /// <param name="model">The ResultViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody]ResultViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)
            // if the client payload is invalid.
            if (model == null) return new StatusCodeResult(500);

            // map the ViewModel to the Model
            var result = model.Adapt<Result>();

            result.CreatedDate = DateTime.Now;
            result.LastModifiedDate = result.CreatedDate;

            DbContext.Results.Add(result);
            DbContext.SaveChanges();

            return new JsonResult(
                result.Adapt<ResultViewModel>(),
                JsonSettings);
        }

        /// <summary>
        /// Deletes the Result with the given {id} from the Database
        /// </summary>
        /// <param name="id">The ID of an existing Result</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = DbContext.Results.Where(i => i.Id == id).FirstOrDefault();

            if(result == null)
            {
                return NotFound(
                    new
                    {
                        Error = $"Result ID {id} has not been found",
                    });
            }

            DbContext.Results.Remove(result);
            DbContext.SaveChanges();

            return
                new JsonResult(
                    result.Adapt<ResultViewModel>(),
                    JsonSettings);
        }
        #endregion

        // GET api/question/all
        [HttpGet("All/{quizId}")]
        public IActionResult All(int quizId)
        {
            var results = DbContext.Results.Where(q => q.QuizId == quizId).ToArray();

            // output the result in JSON format
            return new JsonResult(
                results.Adapt<ResultViewModel[]>(),
                JsonSettings);
        }
    }
}

