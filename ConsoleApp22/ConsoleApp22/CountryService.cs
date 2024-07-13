using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp22

{
    public static class CountryService
    {
        public static void GetAllCountries()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString.Default))
            {
                connection.Open();

                var command = new SqlCommand("SELECT * FROM COUNTRIES", connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["Id"]);
                        string name = Convert.ToString(reader["Name"]);
                        decimal area = Convert.ToDecimal(reader["Area"]);

                        Message.PrintMessage("Id", id.ToString());
                        Message.PrintMessage("Name", name);
                        Message.PrintMessage("Area", area.ToString());


                    }

                }
            }
        }
        public static void AddCountry()
        {
            Message.InputMessage("country name");
            string name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name))
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString.Default))
                {
                    connection.Open();
                    var selectCommand = new SqlCommand("SELECT * FROM WHERE Name=@name, connection");
                    selectCommand.Parameters.AddWithValue("name", name);
                    try
                    {
                        int id = Convert.ToInt32(selectCommand.ExecuteScalar());
                        if (id > 0)
                            Message.AlreadyExistMessage("Country", name);
                        else
                        {

                            Message.InputMessage("contry area");
                            string areaInput = Console.ReadLine();
                            decimal area;
                            bool isSucceeded = decimal.TryParse(areaInput, out area);
                            if (isSucceeded)
                            {



                                var command = new SqlCommand("INSERT INTO Countries VALUES(@name, @area)", connection);
                                command.Parameters.AddWithValue("@name", name);
                                command.Parameters.AddWithValue("@area", area);


                                var affectedRows = command.ExecuteNonQuery();
                                if (affectedRows > 0)
                                    Message.SuccesMessage("Country", name);
                                else
                                    Message.ErrorOccuredMessage();
                            }
                        }
                    }

                    catch (Exception)
                    {
                        Message.ErrorOccuredMessage();
                    }
                }
            }
            else
                Message.InvalidInputMessage("Country name");

        }

        public static void UpdateCountry()
        {
            GetAllCountries();

            Message.InputMessage("Country name");
            string name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name))
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString.Default))
                {
                    connection.Open();
                    var command = new SqlCommand("SELECT * FROM COUNTRIES WHERE Name=@name", connection);
                    command.Parameters.AddWithValue("@name", name);
                    try
                    {
                        int id = Convert.ToInt32(command.ExecuteScalar());
                        if (id > 0)
                        {
                        NameWantToChangeSection: Message.PrintWantToChangeMessage("name");
                            bool hasUpdates;
                            var choiceForNAme = Console.ReadLine();
                            char choice;
                            bool isSucceeded = char.TryParse(choiceForNAme, out choice);
                            if (isSucceeded && choice.IsVAlidChoice())
                            {
                                string newName = string.Empty;
                                if (choice.Equals('y'))
                                {
                                InputNewNameSection: Message.InputMessage("new name");
                                    newName = Console.ReadLine();
                                    if (!string.IsNullOrWhiteSpace(newName))
                                    {
                                        var alreadyExistsCommand = new SqlCommand("SELECT * FROM Countries WHERE Name=@name AND Id !=@id", connection);
                                        alreadyExistsCommand.Parameters.AddWithValue("@name", id);
                                        alreadyExistsCommand.Parameters.AddWithValue("@id", newName);

                                        int existId = Convert.ToInt32(alreadyExistsCommand.ExecuteScalar());
                                        if (existId > 0)
                                        {
                                            Message.AlreadyExistMessage("Country", newName);
                                            goto NameWantToChangeSection;
                                        }

                                    }
                                    else
                                    {
                                        Message.InvalidInputMessage("New name");
                                        goto InputNewNameSection;
                                    }
                                }
                            AreaWantToChangeSection: Message.PrintWantToChangeMessage("area");
                                var choiceForArea = Console.ReadLine();
                                isSucceeded = char.TryParse(choiceForArea, out choice);
                                decimal newArea = default;
                                if (isSucceeded && choice.IsVAlidChoice())
                                {

                                    if (choice.Equals('y'))
                                    {
                                    InputNewArea: Message.InputMessage("New area");
                                        var newAreaInput = Console.ReadLine();
                                        isSucceeded = decimal.TryParse(newAreaInput, out newArea);
                                        if (!isSucceeded)
                                        {
                                            Message.InvalidInputMessage("New area");
                                            goto InputNewArea;
                                        }
                                    }

                                }
                                else
                                {
                                    Message.InvalidInputMessage("New area");
                                    goto AreaWantToChangeSection;
                                };

                                var updateComand = new SqlCommand("Update Countries SET ", connection);
                                if (newName != string.Empty || newArea != default)
                                {
                                    bool isRequiredcomma = false;
                                    if (newName != string.Empty)
                                    {
                                        updateComand.CommandText = updateComand.CommandText + "Name = @name";
                                        updateComand.Parameters.AddWithValue("@name", newName);
                                    }

                                    if (newArea != default)
                                    {
                                        string commaText = isRequiredcomma ? "," : "";
                                        updateComand.CommandText = updateComand.CommandText + $"{commaText}Area=@area";
                                        updateComand.Parameters.AddWithValue("@area", newArea);
                                    }
                                    updateComand.CommandText = updateComand.CommandText + " WHERE id=@id";
                                    updateComand.Parameters.AddWithValue("@id", id);
                                    int affectedRows = Convert.ToInt32(updateComand.ExecuteNonQuery());
                                    if (affectedRows > 0)
                                    {
                                        Message.SuccesMessage("Country", newName);
                                    }
                                    else
                                        Message.ErrorOccuredMessage();
                                }
                            }
                            else
                                Message.InvalidInputMessage("Choice");

                        }
                        else
                            Message.NotFoundMessage("Country", name);
                    }
                    catch
                    {
                        Message.ErrorOccuredMessage();
                    }
                }
            }
            else
                Message.InvalidInputMessage("Country name");
        }

        public static void DeleteCountry()
        {
            GetAllCountries();

            string name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name))
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString.Default))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT * FROM WHERE Name=@name", connection);
                    command.Parameters.AddWithValue("@name", name);

                    try
                    {
                        int id = Convert.ToInt32(command.ExecuteScalar());
                        if (id > 0)
                        {
                            SqlCommand deleteCommand = new SqlCommand("DELETE Countries WHERE Id=@id", connection);
                            deleteCommand.Parameters.AddWithValue("@id", id);

                            int affectedRows = deleteCommand.ExecuteNonQuery();
                            if (affectedRows > 0)
                            {
                                Message.SuccesDeleteMessage("Country", name);
                            }
                            else
                            {
                                Message.ErrorOccuredMessage();
                            }
                        }
                        else
                            Message.NotFoundMessage("Country", name);
                    }
                    catch (Exception)
                    {
                        Message.ErrorOccuredMessage();
                    }
                }

            }
            else
                Message.InvalidInputMessage("Country Name");
        }

        public static void DetailsofCountry()
        {
            GetAllCountries();

            string name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name))
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString.Default))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT * FROM Countries WHERE Name=@name", connection);
                    command.Parameters.AddWithValue("@name", name);
                    try
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();

                                Message.PrintMessage("Id", Convert.ToString(reader["Id"]));
                                Message.PrintMessage("Name", Convert.ToString(reader["Name"]));
                                Message.PrintMessage("Area", Convert.ToString(reader["Area"]));
                            }
                        }
                    }
                    catch (Exception)
                    {

                        Message.ErrorOccuredMessage();
                    }

                }
            }
            else
                Message.InvalidInputMessage("name");
        }
    }
}