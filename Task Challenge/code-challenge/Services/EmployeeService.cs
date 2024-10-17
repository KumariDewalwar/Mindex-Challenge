using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using challenge.Models;
using Microsoft.Extensions.Logging;
using challenge.Repositories;

namespace challenge.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ICompensationRepository _compensationRepository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(ILogger<EmployeeService> logger, IEmployeeRepository employeeRepository, ICompensationRepository compensationRepository)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
            _compensationRepository = compensationRepository;
        }

        public Employee Create(Employee employee)
        {
            if(employee != null)
            {
                _employeeRepository.Add(employee);
                _employeeRepository.SaveAsync().Wait();
            }

            return employee;
        }

        public Employee GetById(string id)
        {
            if(!String.IsNullOrEmpty(id))
            {
                return _employeeRepository.GetById(id);
            }

            return null;
        }

        public Employee Replace(Employee originalEmployee, Employee newEmployee)
        {
            if(originalEmployee != null)
            {
                _employeeRepository.Remove(originalEmployee);
                if (newEmployee != null)
                {
                    // ensure the original has been removed, otherwise EF will complain another entity w/ same id already exists
                    _employeeRepository.SaveAsync().Wait();

                    _employeeRepository.Add(newEmployee);
                    // overwrite the new id with previous employee id
                    newEmployee.EmployeeId = originalEmployee.EmployeeId;
                }
                _employeeRepository.SaveAsync().Wait();
            }

            return newEmployee;
        }
        public ReportingStructure GetReportStructure(string id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                return _employeeRepository.GetReportStructure(id);
            }
            return null;
        }
        public Compensation CreateCompensation(Compensation employee)
        {
            if (employee != null)
            {
                Compensation employeeCompesnation = _compensationRepository.Add(employee, employee.EmployeeId);
                if (employeeCompesnation != null)
                {
                    _compensationRepository.SaveAsync().Wait();
                    return employeeCompesnation;
                }

            }

            return null;
        }
        public Compensation GetCompensation(string id)
        {

            {
                if (String.IsNullOrEmpty(id))
                {
                    return null;
                }
                var compensation = _compensationRepository.GetById(id);
                if (compensation == null)
                {
                    return null;
                }
                compensation.Employee = _employeeRepository.GetById((string)compensation.EmployeeId);
                return compensation;
            }
            
        }
    }
}
