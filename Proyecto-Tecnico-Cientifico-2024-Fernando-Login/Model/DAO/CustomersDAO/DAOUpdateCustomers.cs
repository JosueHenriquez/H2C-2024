﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PTC2024.Model.DTO.CustomersDTO;

namespace PTC2024.Model.DAO.CustomersDAO
{
 class DAOUpdateCustomers:DTOUpdateCustomers
    {
        
        readonly SqlCommand command = new SqlCommand();

        public int updateCustomers()
        {
            try
            {
                command.Connection = getConnection();

                string query = "UPDATE tbCustomer SET DUI = @DUI, names = @Names, lastNames = @LastNames, phone = @Phone, email= @Email, address = @Address WHERE IdCustomer = @Id";

                SqlCommand cmd = new SqlCommand(query, command.Connection);
                cmd.Parameters.AddWithValue("@DUI", Dui);
                cmd.Parameters.AddWithValue("@Names", Name);
                cmd.Parameters.AddWithValue("@LastNames",LastNames);
                cmd.Parameters.AddWithValue("@Phone", Phone);
                cmd.Parameters.AddWithValue("@Email", Email);
                cmd.Parameters.AddWithValue("@Address", Address);
                cmd.Parameters.AddWithValue("@Id", IdClient);
                int respuesta = cmd.ExecuteNonQuery();

                if (respuesta == 1) {
                    return respuesta;
                }

                else
                {
                    return 0;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return -1;

                
            }

            finally
            {
                command.Connection.Close();
            }
        }
   
    }

    
}
