﻿using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using FileStorageApp.Data.InfoStorage.Models;
using JwtAuth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace FileStorageAPI
{
    /// <summary>
    /// Атрибут для контроля доступа пользователей к API.
    /// </summary>
    public class RightsFilterAttribute : ActionFilterAttribute
    {
        private readonly int[] _accesses;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="RightsFilterAttribute"/>.
        /// </summary>
        /// <param name="accesses">Требуемые доступы</param>
        public RightsFilterAttribute(params Accesses[] accesses)
        {
            _accesses = accesses.Cast<int>().ToArray();
        }

        /// <summary>
        /// Фильтрация.
        /// </summary>
        /// <param name="filterContext">Контекст</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var authHeader = filterContext.HttpContext.Request.Headers[HeaderNames.Authorization];
            var userToken = authHeader.ToString().Split(' ')[1];
            var principal = TokenHelper.GetPrincipalFromToken(userToken, Settings.Key);
            var accessClaims = principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role);
            if (accessClaims is null)
            {
                filterContext.Result = new ForbidResult();
                return;
            }

            var accessArray = JsonConvert.DeserializeObject<List<int>>(accessClaims!.Value);
            if (accessArray is null || !accessArray.Intersect(_accesses).Any())
                filterContext.Result = new ForbidResult();
        }
    }
}