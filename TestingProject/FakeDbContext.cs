using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task3.Data;

namespace TestingProject
{
    public class FakeDbContext : AppDbContext
    {
        public FakeDbContext() : base(new DbContextOptions<AppDbContext>())
        {
        }
    }
}