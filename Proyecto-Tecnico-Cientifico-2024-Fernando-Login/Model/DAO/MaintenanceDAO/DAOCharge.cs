﻿using PTC2024.Model.DTO.MaintenanceDTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTC2024.Model.DAO.MaintenanceDAO
{
    internal class DAOCharge : DTOCharge
    {
        readonly SqlCommand Command = new SqlCommand();
        public int AddCharge()
        {
            try
            {
                Command.Connection = getConnection();
                string query = "INSERT INTO tbBusinessP (businessPosition, positionBonus) VALUES (@businessPosition, @positionBonus)";
                SqlCommand cmd = new SqlCommand(query, Command.Connection);
                cmd.Parameters.AddWithValue("businessPosition", NameCharge);
                cmd.Parameters.AddWithValue("positionBonus", BonusCharge);
                int answer = cmd.ExecuteNonQuery();
                return answer;
            }
            catch (Exception)
            {
                return -1;
            }
            finally
            {
                Command.Connection = getConnection();
            }
        }
        public DataSet GetCharge()
        {
            try
            {
                Command.Connection= getConnection();
                string query = "SELECT IdBusinessP, businessPosition AS Cargo, positionBonus AS Bono FROM tbBusinessP";
                SqlCommand cmd = new SqlCommand(query, Command.Connection);
                cmd.ExecuteNonQuery();
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adp.Fill(ds, "tbBusinessP");
                return ds;

            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                getConnection().Close();
            }
        }
        //public int UpdateCharge()
        //{
        //    try
        //    {
        //        Command.Connection = getConnection();
        //        string query = "UPDATE tbBusinessP SET" + "businessPosition = @param1" + "positionBonus = @param2" + "WHERE IdBusinessP = @param3";
        //        SqlCommand cmd = new SqlCommand(query,Command.Connection);
        //        cmd.Parameters.AddWithValue("param1", NameCharge);
        //        cmd.Parameters.AddWithValue("param2", BonusCharge);
        //        cmd.Parameters.AddWithValue("param3", IdCharge);
        //        int answerd = cmd.ExecuteNonQuery();
        //        return answerd;
        //    }
        //    catch (Exception)
        //    {
        //        //Se retorna -1 en caso que en el segmento del try haya ocurrido algún error.
        //        return -1;
        //    }
        //    finally
        //    {
        //        //Independientemente se haga o no el proceso cerramos la conexión
        //        getConnection().Close();
        //    }
        //}
        public int DeleteCharge()
        {
            try
            {
                Command.Connection = getConnection();
                string query = "DELETE tbBusinessP WHERE IdBusinessP = @param1";
                SqlCommand cmd = new SqlCommand(query, Command.Connection);
                cmd.Parameters.AddWithValue("param1", IdCharge);
                int answer = cmd.ExecuteNonQuery();
                return answer;
            }
            catch (Exception)
            {

                return -1;
            }
            finally
            {
                getConnection().Close();
            }
        }

    }
}
