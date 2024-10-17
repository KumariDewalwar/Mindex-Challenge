using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using challenge.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using challenge.Data;

namespace challenge.Repositories
{
    public class EmployeeRespository : IEmployeeRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<IEmployeeRepository> _logger;

        public EmployeeRespository(ILogger<IEmployeeRepository> logger, EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
            _logger = logger;
        }

        public Employee Add(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));
            employee.EmployeeId = Guid.NewGuid().ToString();

            _employeeContext.Employees.Add(employee);
            _logger.LogInformation($"Added new employee with ID {employee.EmployeeId}");
            return employee;
        }

        public Employee GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("employee ID cannot be null or NoWhiteSpace", nameof(id));
            var employee = _employeeContext.Employees.Include(e => e.DirectReports).SingleOrDefault(e => e.EmployeeId == id);
            if (employee == null)
            {
                _logger.LogWarning($"Employee with ID {id} not found");
            }
            LoadDirectReports(employee);
            return employee;
        }

        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }

        public Employee Remove(Employee employee)
        {
            return _employeeContext.Remove(employee).Entity;
        }
        private void LoadDirectReports(Employee employee)
        {
            if (employee != null)
            {
                _employeeContext.Entry(employee).Collection(e => e.DirectReports).Load();

                foreach (var directReport in employee.DirectReports)
                {

                    LoadDirectReports(directReport);
                }
            }
        }
        public ReportingStructure GetReportStructure(string id)
        {
            var employee = new ReportingStructure();
            var starterEmployee = _employeeContext.Employees.Include(e => e.DirectReports).SingleOrDefault(e => e.EmployeeId == id);
            LoadDirectReports(starterEmployee);
            employee.Employee = starterEmployee;
            employee.NumberOfReports = GetReportCount(starterEmployee);
            return employee;

        }

        public int GetReportCount(Employee employee)
        {
            int intCount = 0;
            if (employee != null)
            {
                if (employee.DirectReports != null)
                {
                    foreach (var directReport in employee.DirectReports)
                    {
                        intCount += 1 + GetReportCount(directReport);
                    }

                }
            }
            return intCount;
        }
    }
    }
