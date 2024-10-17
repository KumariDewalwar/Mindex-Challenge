using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using challenge.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using challenge.Data;
using System.Threading;

namespace challenge.Repositories
{
    public class CompensationRespository : ICompensationRepository

    {
        private readonly EmployeeContext _employeeContext;
        private readonly CompensationContext _compensationContext;
        private readonly ILogger<ICompensationRepository> _logger;

        public CompensationRespository(ILogger<ICompensationRepository> logger, CompensationContext compensationContext, EmployeeContext employeeContext)
        {
            _compensationContext = compensationContext;
            _logger = logger;
            _employeeContext = employeeContext;
        }


        public Compensation Add(Compensation compensation, string id)
        {
            {
                var employee = _employeeContext.Employees.SingleOrDefault(e => e.EmployeeId == id);
                Compensation employeeCompensation = _compensationContext.Compensations.SingleOrDefault(c => c.Employee.EmployeeId == id);
                if (employee == null)
                    return null;
                if (employeeCompensation == null)
                {
                    Compensation newRecord = new Compensation
                    {
                        Employee = employee,
                        Salary = compensation.Salary,
                        EffectiveDate = compensation.EffectiveDate
                    };
                    employeeCompensation = newRecord;
                    _compensationContext.Compensations.Add(employeeCompensation);
                }
                else 
                {
                    employeeCompensation.Salary = compensation.Salary;
                    employeeCompensation.EffectiveDate = compensation.EffectiveDate;
                }
                return employeeCompensation;
            }
        }
        

        public Compensation GetById(string id)
        {
            return _compensationContext.Compensations.SingleOrDefault(e => e.Employee.EmployeeId == id);
        }

        public Task SaveAsync()
        {
            return _compensationContext.SaveChangesAsync();
        }

        public Compensation Remove(Compensation compensation)
        {
            return _compensationContext.Remove(compensation).Entity;
        }

    }
}