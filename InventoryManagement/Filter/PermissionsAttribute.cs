﻿using InventoryManagement.Helper;
using InventoryManagement.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Authentication;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace InventoryManagement.Filter
{
    [AttributeUsageAttribute(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class PermissionsAttribute : AuthorizeAttribute
    {
        public string Permission { get; set; }
        public string Action { get; set; }
        public PermissionsAttribute()
        {
        }
        public PermissionsAttribute(string Permission)
        {
            this.Permission = Permission;
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var Usermanager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var currentuser = Usermanager.FindByName(HttpContext.Current.User.Identity.Name);
            var lst = httpContext.Request.RequestContext.RouteData.Values.Values.ToList();
            object actionname = lst[1];
            bool check = false;
            check = Commonhelper.checkpermission(currentuser.Id, Action, Permission);
            return true;
            //check your permissions
        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (AuthorizeCore(filterContext.HttpContext))
            {
                // ** IMPORTANT **
                // Since we're performing authorization at the action level, the authorization code runs
                // after the output caching module. In the worst case this could allow an authorized user
                // to cause the page to be cached, then an unauthorized user would later be served the
                // cached page. We work around this by telling proxies not to cache the sensitive page,
                // then we hook our custom authorization code into the caching mechanism so that we have
                // the final say on whether a page should be served from the cache.

                HttpCachePolicyBase cachePolicy = filterContext.HttpContext.Response.Cache;
                cachePolicy.SetProxyMaxAge(new TimeSpan(0));
                cachePolicy.AddValidationCallback(CacheValidateHandler, null /* data */);
            }
            else
            {
                if (filterContext.HttpContext.User.Identity.IsAuthenticated)
                {

                    filterContext.Result = new RedirectToRouteResult(
                                new RouteValueDictionary(
                                    new
                                    {
                                        controller = "Account",
                                        action = "Unauthorised"
                                    })
                                );
                }
                else
                {
                    base.HandleUnauthorizedRequest(filterContext);
                }
            }
        }
        private void CacheValidateHandler(HttpContext context, object data, ref HttpValidationStatus validationStatus)
        {
            validationStatus = OnCacheAuthorization(new HttpContextWrapper(context));
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {

            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {

                filterContext.Result = new RedirectToRouteResult(
                            new RouteValueDictionary(
                                new
                                {
                                    controller = "Account",
                                    action = "Unauthorised"
                                })
                            );
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
        }


    }
    }

   