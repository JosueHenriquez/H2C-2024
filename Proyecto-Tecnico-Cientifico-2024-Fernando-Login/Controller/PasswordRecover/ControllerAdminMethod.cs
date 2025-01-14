﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using PTC2024.Controller.Helper;
using PTC2024.formularios.login;
using PTC2024.Model.DAO.PasswordRecoverDAO;
using PTC2024.View.Alerts;
using PTC2024.View.PasswordRecover;

namespace PTC2024.Controller.PasswordRecover
{
    internal class ControllerAdminMethod
    {
        FrmAdminMethod objAdminMethod;

        //Constructor
        public ControllerAdminMethod(FrmAdminMethod View)
        {
            objAdminMethod = View;
            objAdminMethod.btnVolver.Click += new EventHandler(Back);
            objAdminMethod.btnConfirm.Click += new EventHandler(ChangePassword);
            objAdminMethod.btnReset.Click += new EventHandler(ResetPassword);
            objAdminMethod.txtAdminUser.MouseDown += new MouseEventHandler(DisableContextMenu);
            objAdminMethod.txtAdminPass.MouseDown += new MouseEventHandler(DisableContextMenu);
            objAdminMethod.txtUser.MouseDown += new MouseEventHandler(DisableContextMenu);
            objAdminMethod.txtUserNewPass.MouseDown += new MouseEventHandler(DisableContextMenu);
            objAdminMethod.txtUserConfirmPass.MouseDown += new MouseEventHandler(DisableContextMenu);
            objAdminMethod.txtAdminUser.TextChanged += new EventHandler(UsernameMask2);
            objAdminMethod.txtUser.TextChanged += new EventHandler(UsernameMask);
        }

        public void ChangePassword(object sender, EventArgs e)
        {
            //Creamos instancia del DAO
            DAOAdminMethod daoAdminM = new DAOAdminMethod();

            //Creamos instancia del commonClasses
            CommonClasses common = new CommonClasses();

            //Validamos primeramente los campos vacíos de los datos del admin
            if (!(string.IsNullOrEmpty(objAdminMethod.txtAdminUser.Text.Trim()) ||
                 string.IsNullOrEmpty(objAdminMethod.txtAdminPass.Text.Trim())
                ))
            {
                //Si estan llenos estos dos campos, entonces el primer proceso es validar si se trata del CEO
                //Damos valor a los getters para el método a utilizar
                daoAdminM.AdminUsername = objAdminMethod.txtAdminUser.Text.Trim();
                daoAdminM.AdminPassword = common.ComputeSha256Hash(objAdminMethod.txtAdminPass.Text.Trim());
                //Ejecutamos el método
                bool answerValidateCEO = daoAdminM.ValidateCEOCredentials();
                if (answerValidateCEO == true)
                {
                    //si la respuesta es true, significa que si se trata del CEO
                    //Ahora pasamos a validar que los campos de las credenciales del usuario estén llenos.
                    if (!(string.IsNullOrEmpty(objAdminMethod.txtUser.Text.Trim()) ||
                          string.IsNullOrEmpty(objAdminMethod.txtUserNewPass.Text.Trim()) ||
                          string.IsNullOrEmpty(objAdminMethod.txtUserConfirmPass.Text.Trim())
                        ))
                    {
                        //Validación para saber si ese usuario si existe en el sistema.
                        //damos valor al getter para el método del DAO
                        daoAdminM.EmployeeUsername = objAdminMethod.txtUser.Text.Trim();
                        //ejecutamos el método del DAO
                        bool checkUser = daoAdminM.CheckUser();
                        if (checkUser == true)
                        {
                            //validación de la longitud de la nueva contraseña
                            if (ValidatePasswordLength() == true)
                            {
                                if (common.IsValid(objAdminMethod.txtUserNewPass.Text.Trim()) == true && common.IsValid(objAdminMethod.txtUserConfirmPass.Text.Trim()) == true)
                                {
                                    //Validación de la confirmación de contraseña
                                    if (objAdminMethod.txtUserNewPass.Text.Trim() == objAdminMethod.txtUserConfirmPass.Text.Trim())
                                    {
                                        //Todas las validaciones son correctas
                                        //damos valor a los getters una vez más.
                                        daoAdminM.EmployeeUsername = objAdminMethod.txtUser.Text.Trim();
                                        daoAdminM.EmployeePassword = common.ComputeSha256Hash(objAdminMethod.txtUserNewPass.Text.Trim());
                                        daoAdminM.TemporalPass = false;
                                        //ejecutamos el método del dao.
                                        int passwordUpdt = daoAdminM.UpdatePersonalizedPass();
                                        //Evaluamos la respuesta del método
                                        if (passwordUpdt == 1)
                                        {
                                            MessageBox.Show("La contraseña del usuario fue actualizada con éxito.", "Proceso completado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            objAdminMethod.txtAdminUser.Clear();
                                            objAdminMethod.txtAdminPass.Clear();
                                            objAdminMethod.txtUser.Clear();
                                            objAdminMethod.txtUserNewPass.Clear();
                                            objAdminMethod.txtUserConfirmPass.Clear();
                                        }
                                        else
                                        {
                                            MessageBox.Show("La contraseña de usuario no pudo ser actualizada, inténtelo de nuevo", "Proceso fallido", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("La confirmación de la nueva contraseña es incorrecta, intente de nuevo.", "Confirmación de contraseña", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Fromato incorrecto: La contraseña debe tener al menos 8 caracteres, un numero, una mayuscula, una minuscula y un simbolo,", "Formato inccorecto", MessageBoxButtons.OK, MessageBoxIcon.Error);

                                }
                            }


                            else
                            {
                                MessageBox.Show("La nueva contraseña del usuario debe de tener al menos 6 caracteres.", "Contraseña corta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        else
                        {
                            MessageBox.Show("El usuario al que se le quiere cambiar la contraseña no está registrado en el sistema.", "Usuario inexistente", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("LLene todos los campos de las credenciales del usuario para poder continuar.", "Campos vacíos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    //si no se trata del CEO, entonces verificamos si se trata de un administrador
                    //Damos valor a los getters para el método a utilizar
                    daoAdminM.AdminUsername = objAdminMethod.txtAdminUser.Text.Trim();
                    daoAdminM.AdminPassword = common.ComputeSha256Hash(objAdminMethod.txtAdminPass.Text.Trim());
                    //ejecutamos el método
                    bool answerValidateAdmin = daoAdminM.ValidateAdminCredentials();
                    //Validamos la respuesta
                    if (answerValidateAdmin == true)
                    {
                        //si la respuesta es true, significa que si se trata de un administrador
                        //Ahora pasamos a validar que los campos de las credenciales del usuario estén llenos.
                        if (!(string.IsNullOrEmpty(objAdminMethod.txtUser.Text.Trim()) ||
                              string.IsNullOrEmpty(objAdminMethod.txtUserNewPass.Text.Trim()) ||
                              string.IsNullOrEmpty(objAdminMethod.txtUserConfirmPass.Text.Trim())
                            ))
                        {
                            //Validación para saber si ese usuario si existe en el sistema.
                            //damos valor al getter para el método del DAO
                            daoAdminM.EmployeeUsername = objAdminMethod.txtUser.Text.Trim();
                            //ejecutamos el método del DAO
                            bool checkUser = daoAdminM.CheckUser();
                            if (checkUser == true)
                            {

                                //Validación para saber si se trata de un usuario de tipo empleado o de un administrador
                                //damos valor al getter de nuevo por si acaso
                                daoAdminM.EmployeeUsername = objAdminMethod.txtUser.Text.Trim();
                                //ejecutamos el método del DAO
                                bool employeeUser = daoAdminM.CheckEmployeeUser();
                                //validamos la respuesta
                                if (employeeUser == true)
                                {
                                    if (common.IsValid(objAdminMethod.txtUserNewPass.Text.Trim()) == true && common.IsValid(objAdminMethod.txtUserConfirmPass.Text.Trim()) == true)
                                    {
                                        //Si la respuesta es true, si se trata de un empleado
                                        //validación de la longitud de la nueva contraseña
                                        if (ValidatePasswordLength() == true)
                                        {
                                            //Validación de la confirmación de contraseña
                                            if (objAdminMethod.txtUserNewPass.Text.Trim() == objAdminMethod.txtUserConfirmPass.Text.Trim())
                                            {
                                                //Todas las validaciones son correctas
                                                //damos valor a los getters una vez más.
                                                daoAdminM.EmployeeUsername = objAdminMethod.txtUser.Text.Trim();
                                                daoAdminM.EmployeePassword = common.ComputeSha256Hash(objAdminMethod.txtUserNewPass.Text.Trim());
                                                daoAdminM.TemporalPass = false;
                                                //ejecutamos el método del dao.
                                                int passwordUpdt = daoAdminM.UpdatePersonalizedPass();
                                                //Evaluamos la respuesta del método
                                                if (passwordUpdt == 1)
                                                {
                                                    MessageBox.Show("La contraseña del usuario fue actualizada con éxito.", "Proceso completado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                    objAdminMethod.txtAdminUser.Clear();
                                                    objAdminMethod.txtAdminPass.Clear();
                                                    objAdminMethod.txtUser.Clear();
                                                    objAdminMethod.txtUserNewPass.Clear();
                                                    objAdminMethod.txtUserConfirmPass.Clear();
                                                }
                                                else
                                                {
                                                    MessageBox.Show("La contraseña de usuario no pudo ser actualizada, inténtelo de nuevo", "Proceso fallido", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                }
                                            }
                                            else
                                            {
                                                MessageBox.Show("La confirmación de la nueva contraseña es incorrecta, intente de nuevo.", "Confirmación de contraseña", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show("La nueva contraseña del usuario debe de tener al menos 6 caracteres.", "Contraseña corta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        }

                                    }
                                    else
                                    {
                                        MessageBox.Show("Fromato incorrecto: La contraseña debe tener al menos 8 caracteres, un numero, una mayuscula, una minuscula y un simbolo,", "Formato inccorecto", MessageBoxButtons.OK, MessageBoxIcon.Error);

                                    }
                                }
                                else
                                {
                                    MessageBox.Show("El usuario al que se le desea cambiar la contraseña es uno de tipo administrador, por lo que esta acción no se puede realizar.", "Acción imposible", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            else
                            {
                                MessageBox.Show("El usuario al que se le quiere cambiar la contraseña no está registrado en el sistema.", "Usuario inexistente", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }

                        }
                        else
                        {
                            MessageBox.Show("LLene todos los campos de las credenciales del usuario para poder continuar.", "Campos vacíos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Las credenciales de administrador ingresadas no son válidas", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

            }
            else
            {
                MessageBox.Show("Llene los campos de las credenciales de administrador para continuar.", "Campos vacíos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //Método para solamente restablecer la contraseña
        public void ResetPassword(object sender, EventArgs e)
        {

            //Creamos instancia del DAO
            DAOAdminMethod daoAdminM = new DAOAdminMethod();

            //Creamos instancia del commonClasses
            CommonClasses common = new CommonClasses();

            //Validamos primeramente los campos vacíos de los datos del admin
            if (!(string.IsNullOrEmpty(objAdminMethod.txtAdminUser.Text.Trim()) ||
                 string.IsNullOrEmpty(objAdminMethod.txtAdminPass.Text.Trim())
                ))
            {
                //Si estan llenos estos dos campos, entonces el primer proceso es validar si se trata del CEO
                //Damos valor a los getters para el método a utilizar
                daoAdminM.AdminUsername = objAdminMethod.txtAdminUser.Text.Trim();
                daoAdminM.AdminPassword = common.ComputeSha256Hash(objAdminMethod.txtAdminPass.Text.Trim());
                //Ejecutamos el método
                bool answerValidateCEO = daoAdminM.ValidateCEOCredentials();
                if (answerValidateCEO == true)
                {
                    //Si es true, se trata del CEO
                    //Ahora pasamos a validar que el campo del usuario del empleado esté lleno, ya que es el único necesario para esta acción.
                    if (!(string.IsNullOrEmpty(objAdminMethod.txtUser.Text.Trim())))
                    {

                        daoAdminM.EmployeeUsername = objAdminMethod.txtUser.Text.Trim();
                        //ejecutamos el método del DAO
                        bool checkUser = daoAdminM.CheckUser();
                        if (checkUser == true)
                        {
                            //Todas las validaciones son correctas
                            //damos valor a los getters una vez más.
                            daoAdminM.EmployeeUsername = objAdminMethod.txtUser.Text.Trim();
                            daoAdminM.TemporalPass = true;
                            string result = "Hubo un error al intentar restablecer la contraseña.";
                            //Ejecutamos el método
                            result = daoAdminM.RecoverPassword();

                            MessageBox.Show(result, "Recuperación de contraseña", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            objAdminMethod.txtAdminUser.Clear();
                            objAdminMethod.txtAdminPass.Clear();
                            objAdminMethod.txtUser.Clear();
                            objAdminMethod.txtUserNewPass.Clear();
                            objAdminMethod.txtUserConfirmPass.Clear();
                        }
                        else
                        {
                            MessageBox.Show("El usuario al que se le quiere cambiar la contraseña no está registrado en el sistema.", "Usuario inexistente", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                    }
                    else
                    {
                        MessageBox.Show("LLene todos los campos de las credenciales del usuario para poder continuar.", "Campos vacíos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    //Si no era el CEO, entonces pasamos a verificar si se trata de un administrador.
                    //Damos valor a los getters para el método a utilizar
                    daoAdminM.AdminUsername = objAdminMethod.txtAdminUser.Text.Trim();
                    daoAdminM.AdminPassword = common.ComputeSha256Hash(objAdminMethod.txtAdminPass.Text.Trim());
                    //ejecutamos el método
                    bool answerValidateAdmin = daoAdminM.ValidateAdminCredentials();
                    //Validamos la respuesta
                    if (answerValidateAdmin == true)
                    {
                        //si la respuesta es true, significa que si se trata de un administrador
                        //Ahora pasamos a validar que el campo del usuario del empleado esté lleno, ya que es el único necesario para esta acción.
                        if (!(string.IsNullOrEmpty(objAdminMethod.txtUser.Text.Trim())))
                        {

                            //Validación para saber si ese usuario si existe en el sistema.
                            //damos valor al getter para el método del DAO
                            daoAdminM.EmployeeUsername = objAdminMethod.txtUser.Text.Trim();
                            //ejecutamos el método del DAO
                            bool checkUser = daoAdminM.CheckUser();
                            if (checkUser == true)
                            {
                                //Validación para saber si se trata de un usuario de tipo empleado o de un administrador
                                //damos valor al getter de nuevo por si acaso
                                daoAdminM.EmployeeUsername = objAdminMethod.txtUser.Text.Trim();
                                //ejecutamos el método del DAO
                                bool employeeUser = daoAdminM.CheckEmployeeUser();
                                //validamos la respuesta
                                if (employeeUser == true)
                                {
                                    //Si la respuesta es true, si se trata de un empleado
                                    //Todas las validaciones son correctas
                                    //damos valor a los getters una vez más.
                                    daoAdminM.EmployeeUsername = objAdminMethod.txtUser.Text.Trim();
                                    daoAdminM.TemporalPass = true;
                                    string result = "Hubo un error al intentar restablecer la contraseña.";
                                    //Ejecutamos el método
                                    result = daoAdminM.RecoverPassword();

                                    MessageBox.Show(result, "Recuperación de contraseña", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    objAdminMethod.txtAdminUser.Clear();
                                    objAdminMethod.txtAdminPass.Clear();
                                    objAdminMethod.txtUser.Clear();
                                    objAdminMethod.txtUserNewPass.Clear();
                                    objAdminMethod.txtUserConfirmPass.Clear();
                                }
                                else
                                {
                                    MessageBox.Show("El usuario al que se le desea cambiar la contraseña es uno de tipo administrador, por lo que esta acción no se puede realizar.", "Acción imposible", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }

                            else
                            {
                                MessageBox.Show("El usuario al que se le quiere cambiar la contraseña no está registrado en el sistema.", "Usuario inexistente", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }

                        }
                        else
                        {
                            MessageBox.Show("LLene todos los campos de las credenciales del usuario para poder continuar.", "Campos vacíos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Las credenciales de administrador ingresadas no son válidas", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

            }
            else
            {
                MessageBox.Show("Llene los campos de las credenciales de administrador para continuar.", "Campos vacíos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void Back(object sender, EventArgs e)
        {
            FrmRecoverPMethods openForm = new FrmRecoverPMethods();
            //Cerramos el formulario y abrimos nuevamente el formulario para seleccionar los métodos
            objAdminMethod.Close();
            openForm.Show();
        }

        public bool ValidatePasswordLength()
        {
            if (objAdminMethod.txtUserNewPass.Text.Trim().Length >= 6)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void DisableContextMenu(object sender, MouseEventArgs e)
        {
            // Desactiva el menú contextual al hacer clic derecho
            if (e.Button == MouseButtons.Right)
            {
                ((Bunifu.UI.WinForms.BunifuTextBox)sender).ContextMenu = new ContextMenu();  // Asigna un menú vacío
            }
        }
        public void UsernameMask(object sender, EventArgs e)
        {
            //Almacena la posición original del cursor
            int cursorPosition = objAdminMethod.txtUser.SelectionStart;

            //Filtra el texto del TextBox para eliminar caracteres especiales
            objAdminMethod.txtUser.Text = new string(objAdminMethod.txtUser.Text.Where(c => char.IsLetterOrDigit(c)).ToArray());

            //Restaura la posición del cursor
            objAdminMethod.txtUser.SelectionStart = cursorPosition;
        }
        public void UsernameMask2(object sender, EventArgs e)
        {
            //Almacena la posición original del cursor
            int cursorPosition = objAdminMethod.txtAdminUser.SelectionStart;

            //Filtra el texto del TextBox para eliminar caracteres especiales
            objAdminMethod.txtAdminUser.Text = new string(objAdminMethod.txtAdminUser.Text.Where(c => char.IsLetterOrDigit(c)).ToArray());

            //Restaura la posición del cursor
            objAdminMethod.txtAdminUser.SelectionStart = cursorPosition;
        }
        public bool ValidarTexto()
        {
            string input = objAdminMethod.txtUserNewPass.Text;

            // Verifica que haya al menos 8 caracteres
            if (input.Length < 8)
            {
                return false;
            }

            // Patrón para validar la contraseña
            string pattern = @"^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]).{8,}$";

            if (Regex.IsMatch(input, pattern))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
