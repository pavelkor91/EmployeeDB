using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EmployeeClient.ServiceReference1;

namespace EmployeeClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private EmployeesEntities proxy;
        private void Form1_Load(object sender, EventArgs e)
        {
             this.proxy = new EmployeesEntities
                (new Uri("http://localhost:2671/EmployeeService.svc/"));
             this.iniNavigator();
             this.createColumns();
             this.fillDataGrid();

        }
        private void createColumns()
        {
            this.employeeDataGridView.Columns.Add("PersonId", "Порядковый номер");
            this.employeeDataGridView.Columns.Add("Name", "Имя");
            this.employeeDataGridView.Columns.Add("Sename", "Фамилия");
            this.employeeDataGridView.Columns.Add("Patronymic", "Отчество");
            this.employeeDataGridView.Columns.Add("Date", "Дата");
        }

        private void fillDataGrid()
        {
            this.employeeDataGridView.Rows.Clear();
            var list = this.proxy.Employee.ToArray();
            foreach (var row in list)
            {

                this.employeeDataGridView.Rows.Add(row.PersonId, row.Name, 
                    row.Sename, row.Patronymic, row.Date);
            }

        }
        

        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {

            this.employeeDataGridView.Rows.Add();
        }
        private void MakeRead()
        {
            this.employeeDataGridView.AllowUserToAddRows = true;
            this.employeeDataGridView.AllowUserToDeleteRows = true;
            this.employeeDataGridView.ReadOnly = false;
        }
        private void iniNavigator()
        {
            this.bindingNavigatorDeleteItem.Enabled = true;
            this.bindingNavigatorAddNewItem.Enabled = true;
            this.MakeRead();
        }
        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in this.employeeDataGridView.SelectedRows)
            {

                var _employee = this.getEmployeeFromGridCell(row.Cells[0].Value);
                if (_employee != null)
                {
                    this.proxy.DeleteObject(_employee);
                }

                this.employeeDataGridView.Rows.Remove(row);

            }
            this.proxy.SaveChanges();
            this.fillDataGrid();
        }

        private void employeeBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in this.employeeDataGridView.Rows)
            {
                if (row.Index == this.employeeDataGridView.Rows.Count - 1)
                {
                    continue;
                }

                var _employee = this.getEmployeeFromGridCell(row.Cells[0].Value);

                if (_employee != null)
                {
                    _employee.Name = row.Cells[1].Value.ToString();
                    _employee.Sename = row.Cells[2].Value.ToString();
                    _employee.Patronymic = row.Cells[3].Value.ToString();
                    _employee.Date = DateTime.Parse(row.Cells[4].Value.ToString());
                    this.proxy.UpdateObject(_employee);
                }
                else
                {
                    Employee newEmployee = new Employee();
                    newEmployee.Name = row.Cells[1].Value.ToString();
                    newEmployee.Sename = row.Cells[2].Value.ToString();
                    newEmployee.Patronymic = row.Cells[3].Value.ToString();
                    newEmployee.Date = DateTime.Parse(row.Cells[4].Value.ToString());
                    this.proxy.AddToEmployee(newEmployee);
                }
            }
            this.proxy.SaveChanges();

            this.fillDataGrid();
            
        }
        private Employee getEmployeeFromGridCell(Object gridId)
        {
            int id;
            if (gridId == null)
                return null;
            if (!int.TryParse(gridId.ToString(), out id))
                return null;
            var query = from o in this.proxy.Employee
                        where o.PersonId == id
                        select o;
            var _employee = query.First();

            return _employee;
        }


        
    }
}
