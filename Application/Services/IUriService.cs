using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnetbase.Application.Filter;

namespace dotnetbase.Application.Services
{
    public interface IUriService
    {
        public Uri GetPageUri(PaginationFilter filter, string route);
    }
}