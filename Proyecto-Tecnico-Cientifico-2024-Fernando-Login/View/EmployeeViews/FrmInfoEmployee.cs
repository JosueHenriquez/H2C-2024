﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PTC2024.Controller.EmployeesController;

namespace PTC2024.View.EmployeeViews
{
    public partial class FrmInfoEmployee : Form
    {
        
        public FrmInfoEmployee(string employee, string dui, DateTime birthDate, string adress, string phone, string email, DateTime hireDate, string maritalStatus, string typeEmployee, string statusEmployee, double salary, int affiliationNumber, string bankAccount, string username, string businessP, string department, string bank)
        {
            InitializeComponent();
            ControllerInfoEmployee objInfoEmployee = new ControllerInfoEmployee(this, employee, dui, birthDate, adress, phone, email, hireDate, maritalStatus, typeEmployee, statusEmployee, salary, affiliationNumber, bankAccount, username, businessP, department, bank);

        }
    }
}
