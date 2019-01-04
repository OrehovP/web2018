﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Vacancies.Models;

namespace Vacancies.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            VacanciesViewModel model = GetVacancies(120000, 15000);
            return View(model);
        }

        //public async Task SendRequest()
        //{
        //    using (
        //        HttpClient client = new HttpClient())
        //    {
        //        var request = new HttpRequestMessage()
        //        {
        //            RequestUri = new Uri("https://api.hh.ru/vacancies"),
        //            Method = HttpMethod.Get,
        //        };
        //        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        var response = await client.SendAsync(request);
        //        if (response.StatusCode == HttpStatusCode.OK)
        //        {
        //            HttpContent responseContent = response.Content;
        //            var json = await responseContent.ReadAsStringAsync();
        //            //var vacancies = JsonConvert.DeserializeObject(json);
        //        }
        //    }
        //}

        private const string host = "https://api.hh.ru";
        private const string resource = "/vacancies";
        private const int vacanciesPerPage = 100;
        private const int firstPage = 0;

        private readonly IRestClient client = new RestClient(host);

        public VacanciesViewModel GetVacancies(int bigSalary, int lowSalary)
        {
            VacanciesViewModel model = new VacanciesViewModel(bigSalary, lowSalary);
            IRestResponse response = RequestVacancies(firstPage);
            int pagesCount = (int)JObject.Parse(response.Content)["pages"];
            JArray vacancies = JObject.Parse(response.Content)["items"] as JArray;
            for (int i = firstPage; i < 50; i++)
            {
                foreach (var vacancy in vacancies)
                {
                    if (vacancy["salary"].Type == JTokenType.Null)
                        continue;
                    var salaryFrom = vacancy["salary"]["from"];
                    var salaryTo = vacancy["salary"]["to"];
                    var salaryCurr = vacancy["salary"]["currency"];
                    var salaryFromType = salaryFrom.Type;
                    var salaryToType = salaryTo.Type;
                    double salary = -1D;
                    if ((string)salaryCurr != "RUR")
                        continue;
                    else if (salaryFromType != JTokenType.Null && salaryToType != JTokenType.Null)
                        salary = ((double)salaryFrom + (double)salaryTo) / 2;
                    else if (salaryFromType == JTokenType.Null && salaryToType != JTokenType.Null)
                        salary = (double)salaryTo;
                    else if (salaryFromType != JTokenType.Null && salaryToType == JTokenType.Null)
                        salary = (double)salaryFrom;
                    if (salary >= bigSalary)
                    {
                        model.ProfessionsWithBigSalary.Add((string)vacancy["name"]);
                        var details = JObject.Parse(RequestVacancyDetails((string)vacancy["id"]).Content);
                        JArray keySkills = details["key_skills"] as JArray;
                        if (keySkills.HasValues)
                        {
                            foreach (var keySkill in keySkills)
                            {
                                model.SkillsForBigSalary.Add((string)keySkill["name"]);
                            }
                        }
                    }
                    else if (salary > 0 && salary < lowSalary)
                    {
                        model.ProfessionsWithLowSalary.Add((string)vacancy["name"]);
                        var details = JObject.Parse(RequestVacancyDetails((string)vacancy["id"]).Content);
                        JArray keySkills = details["key_skills"] as JArray;
                        if (keySkills.HasValues)
                        {
                            foreach (JToken keySkill in keySkills)
                            {
                                model.SkillsForLowSalary.Add((string)keySkill["name"]);
                            }
                        }
                    }
                }
                response = RequestVacancies(firstPage + i + 1);
                vacancies = JObject.Parse(response.Content)["items"] as JArray;
            }
            return model;
        }

        private IRestResponse RequestVacancies(int page)
        {
            IRestRequest request = new RestRequest(string.Format("{0}?page={1}&per_page={2}", resource, page, vacanciesPerPage), Method.GET);
            request.AddParameter("only_with_salary", "true");
            return client.Execute(request);
        }

        private IRestResponse RequestVacancyDetails(string id)
        {
            IRestRequest request = new RestRequest(string.Format("{0}/{1}", resource, id), Method.GET);
            return client.Execute(request);
        }
    }
}
