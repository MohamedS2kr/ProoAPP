﻿using Proo.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
#nullable enable
namespace Proo.Core.Specifications
{
    public class BaseSpecifications<T> : ISpecifications<T> where T : BaseEntity
    {
        public Expression<Func<T, bool>>? Criteria { get; set ; }
        public List<Expression<Func<T, object>>> Includes { get; set; } = new List<Expression<Func<T, object>>>();
        public Expression<Func<T, object>> OrderBy { get; set ; }
        public Expression<Func<T, object>> OrderByDesc { get; set; }

        public BaseSpecifications()
        {
            
        }

        public BaseSpecifications(Expression<Func<T , bool>> expression)
        {
            Criteria = expression;
        }

        public void AddOrderBy(Expression<Func<T , object>> expression)
        {
            OrderBy = expression;
        }
        public void AddOrderByDesc(Expression<Func<T , object>> expression)
        {
            OrderByDesc = expression;
        }
    }
}
