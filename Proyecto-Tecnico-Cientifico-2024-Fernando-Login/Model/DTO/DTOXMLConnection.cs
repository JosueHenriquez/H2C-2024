﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTC2024.Model.DTO.HelperDTO
{
    internal class DTOXMLConnection
    {
        //PARA EL ARCHIVO DE CONEXIÓN
        private static string server;
        private static string database;
        private static string user;
        private static string password;

        public static string Server { get => server; set => server = value; }
        public static string Database { get => database; set => database = value; }
        public static string User { get => user; set => user = value; }
        public static string Password { get => password; set => password = value; }
    }
}