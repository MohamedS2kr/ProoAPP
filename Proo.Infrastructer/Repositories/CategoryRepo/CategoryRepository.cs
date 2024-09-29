using Proo.Core.Contract.CategoryOfVehicleInterface;
using Proo.Core.Entities;
using Proo.Infrastructer.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Infrastructer.Repositories.CategoryRepo
{
    public class CategoryRepository:GenaricRepository<CategoryOfVehicle>,ICategoryOfVehicleRepository
    {
        public CategoryRepository(ApplicationDbContext context):base(context)
        {

        }
    }
}

