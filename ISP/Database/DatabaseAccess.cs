using ISP.Database.Models;
using ISP.Exceptions;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace ISP.Database {
    public static class DatabaseAccess {
        private static readonly string DATABASE_FILENAME = "isp.db";
        private static readonly string USERS_TABLE_NAME = "Users";
        private static readonly string RATES_TABLE_NAME = "Rates";
        private static readonly string SERVICES_TABLE_NAME = "Services";
        private static readonly string CLIENTS_TABLE_NAME = "Clients";
        private static readonly string BLOGS_TABLE_NAME = "Blogs";
        private static readonly string HISTORY_TABLE_NAME = "History";
        private static readonly string ACTIVITY_TYPES_TABLE_NAME = "ActivityTypes";
        private static readonly string ORDERS_TABLE_NAME = "Orders";

        private static readonly string[] initialSqlCommands = {
            $@"CREATE TABLE IF NOT EXISTS {USERS_TABLE_NAME} (
  	            Id INTEGER PRIMARY KEY,
	            FirstName VARCHAR(20),
  	            LastName VARCHAR(20),
  	            Login VARCHAR(16) NOT NULL UNIQUE,
                Password VARCHAR(255) NOT NULL,
	            Bdate DATE NOT NULL NOT NULL,
  	            Address VARCHAR(60) NOT NULL,
  	            Phone VARCHAR(10) NOT NULL,
  	            Email VARCHAR(30),
  	            RegistrationDate DATE NOT NULL,
  	            IsAdmin BOOLEAN
            );",

            $@"CREATE TABLE IF NOT EXISTS {BLOGS_TABLE_NAME} ( 
  	            Id INTEGER PRIMARY KEY,
	            AdminId INTEGER,
  	            Title VARCHAR(35),
  	            Content VARCHAR(2048),
  	            PublishDate DATE,
  	            Likes INTEGER,
  
  	            FOREIGN KEY(AdminId) REFERENCES {USERS_TABLE_NAME}(Id)
            );",

            $@"CREATE TABLE IF NOT EXISTS {RATES_TABLE_NAME} (
	            Id INTEGER PRIMARY KEY,
  	            Title VARCHAR(25) UNIQUE,
  	            Fee DECIMAL,
  	            Speed INTEGER
            );",

            $@"CREATE TABLE IF NOT EXISTS {SERVICES_TABLE_NAME} (
                Id INTEGER PRIMARY KEY,
                Title VARCHAR(25) UNIQUE,
                Price DECIMAL
            );",

            $@"CREATE TABLE IF NOT EXISTS {ACTIVITY_TYPES_TABLE_NAME} (
                Id INTEGER PRIMARY KEY,
                Title VARCHAR(25) UNIQUE
            );",

            $@"CREATE TABLE IF NOT EXISTS {HISTORY_TABLE_NAME} (
                Id INTEGER PRIMARY KEY,
                UserId INTEGER,
                ActivityId INTEGER,
                ActivityDate DATE,
                ActivityDescription VARCHAR(35),
  
	            FOREIGN KEY(UserId) REFERENCES {USERS_TABLE_NAME}(Id),
	            FOREIGN KEY(ActivityId) REFERENCES {ACTIVITY_TYPES_TABLE_NAME}(Id)
            );",

            $@"CREATE TABLE IF NOT EXISTS {CLIENTS_TABLE_NAME} (
                Id INTEGER PRIMARY KEY,
                UserId INTEGER NOT NULL UNIQUE,
                RateId INTEGER,
                Balance DECIMAL DEFAULT 0,
                LastPaymentDate DATE,
                LastPaymentAmount DECIMAL,
                TotalPaymentAmount DECIMAL,

                FOREIGN KEY(UserId) REFERENCES {USERS_TABLE_NAME}(Id) ON DELETE CASCADE,
                FOREIGN KEY(RateId) REFERENCES {RATES_TABLE_NAME}(Id) ON DELETE CASCADE
            );",

            $@"CREATE TABLE IF NOT EXISTS {ORDERS_TABLE_NAME} (
                Id INTEGER PRIMARY KEY,
                ServiceId INTEGER,
                ClientId INTEGER,
                OrderDate DATE,
                IsDone BOOLEAN,
                CompletionDate DATE,

                FOREIGN KEY(ClientId) REFERENCES {USERS_TABLE_NAME}(Id) ON DELETE CASCADE,
                FOREIGN KEY(ServiceId) REFERENCES {SERVICES_TABLE_NAME}(Id) ON DELETE CASCADE
            );"
        };

        private static string connectionString;

        public async static Task InitializeDatabase() {
            await ApplicationData.Current.LocalFolder.CreateFileAsync(DATABASE_FILENAME, CreationCollisionOption.OpenIfExists);
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, DATABASE_FILENAME);

            connectionString = $"Filename={dbpath}";

            await InitTables();
        }

        private async static Task InitTables() {
            using (var databaseConnection = new SqliteConnection(connectionString)) {
                await databaseConnection.OpenAsync();

                foreach (string command in initialSqlCommands) {
                    var createTable = new SqliteCommand(command, databaseConnection);
                    await createTable.ExecuteNonQueryAsync();
                }
            }
        }

        #region Authorization
        public async static Task<UserModel> Login(string login, string password) {
            using (var databaseConnection = new SqliteConnection(connectionString)) {
                await databaseConnection.OpenAsync();

                var command = new SqliteCommand($@"
                    SELECT * FROM {USERS_TABLE_NAME} 
                    WHERE Login = @login AND Password = @password 
                    LIMIT 1
                ", databaseConnection);

                command.Parameters.AddWithValue("@login", login);
                command.Parameters.AddWithValue("@password", password);

                var reader = await command.ExecuteReaderAsync();
                var mayContinue = await reader.ReadAsync();

                if (!mayContinue) throw new AuthorizationException(
                    AuthorizationException.AuthErrorType.IncorrectLoginOrPassword,
                    "Incorrect login or password"
                );

                var user = new UserModel();
                return user.Create(reader);
            }
        }

        public async static Task<int> Register(UserModel user) {
            using (var databaseConnection = new SqliteConnection(connectionString)) {
                try {
                    await databaseConnection.OpenAsync();

                    var signupCommand = new SqliteCommand($@"INSERT INTO {USERS_TABLE_NAME} (
                        FirstName, LastName,
  	                    Login, Password,
	                    Bdate, Address,
  	                    Phone, Email, RegistrationDate, IsAdmin
                    ) VALUES (@firstName, @lastName, @login, @password, @bdate, @address, @phone, @email, @rdate, @is_admin); SELECT last_insert_rowid()", databaseConnection);

                    signupCommand.Parameters.AddWithValue("@firstName", user.FirstName);
                    signupCommand.Parameters.AddWithValue("@lastName", user.LastName);
                    signupCommand.Parameters.AddWithValue("@login", user.Login);
                    signupCommand.Parameters.AddWithValue("@password", user.Password);
                    signupCommand.Parameters.AddWithValue("@bdate", user.Bdate);
                    signupCommand.Parameters.AddWithValue("@address", user.Address);
                    signupCommand.Parameters.AddWithValue("@phone", user.Phone);
                    signupCommand.Parameters.AddWithValue("@email", user.Email);
                    signupCommand.Parameters.AddWithValue("@rdate", DateTime.Now);
                    signupCommand.Parameters.AddWithValue("@is_admin", false);

                    object data = await signupCommand.ExecuteScalarAsync();
                    if (data == null) throw new AuthorizationException(AuthorizationException.AuthErrorType.LoginAlreadyExists, "This login already exists");

                    return Convert.ToInt32(data);
                } catch (Exception exception) {
                    Console.WriteLine(exception);
                    throw new AuthorizationException(AuthorizationException.AuthErrorType.Unknown, "Internal error");
                }
            }
        }

        #endregion Authorization

        public async static Task<int> CreateClient(int userId) {
            using (var connection = new SqliteConnection(connectionString)) {
                return await CreateClient(userId, connection);
            }
        }

        private async static Task<int> CreateClient(int userId, SqliteConnection sqliteConnection) {
            var clientCommand = new SqliteCommand($@"INSERT INTO {CLIENTS_TABLE_NAME} (
                UserId, RateId, Balance,
                LastPaymentDate, LastPaymentAmount, 
                TotalPaymentAmount
            ) VALUES (@userId, null, null, 0, 0); SELECT last_insert_rowid();", sqliteConnection);

            clientCommand.Parameters.AddWithValue("@userId", userId);
            object data = await clientCommand.ExecuteScalarAsync();
            if (data == null) throw new DatabaseOperationException("Cannot to add the client");

            return Convert.ToInt32(data);
        }

        public async static Task<bool> AddService(ServiceModel service) {
            using (var databaseConnection = new SqliteConnection(connectionString)) {
                await databaseConnection.OpenAsync();

                var command = new SqliteCommand($@"INSERT INTO {SERVICES_TABLE_NAME} (Title, Price) VALUES (@title, @price)", databaseConnection);
                command.Parameters.AddWithValue("@title", service.Title);
                command.Parameters.AddWithValue("@price", service.Price);

                int affected = await command.ExecuteNonQueryAsync();

                return affected > 0;
            }
        }

        public async static Task<int> AddRate(RateModel rate) {
            using (var databaseConnection = new SqliteConnection(connectionString)) {
                await databaseConnection.OpenAsync();

                var command = new SqliteCommand($@"INSERT INTO {RATES_TABLE_NAME} (Title, Fee, Speed) VALUES (@title, @fee, @speed); SELECT last_insert_rowid();", databaseConnection);
                command.Parameters.AddWithValue("@title", rate.Title);
                command.Parameters.AddWithValue("@fee", rate.Fee);
                command.Parameters.AddWithValue("@speed", rate.Speed);

                object data = await command.ExecuteScalarAsync();
                if (data == null) return 0;

                return Convert.ToInt32(data);
            }
        }

        public async static Task<bool> DeleteRate(int id) {
            using (var databaseConnection = new SqliteConnection(connectionString)) {
                await databaseConnection.OpenAsync();

                var command = new SqliteCommand($@"
                    -- delete all childs table rows 
                    update {CLIENTS_TABLE_NAME} 
                    set RateId = NULL
                    where RateId = @rateId;
                    
                    -- delete rates with the corresponding id 
                    delete from {RATES_TABLE_NAME} 
                    where Id = @rateId;
                ", databaseConnection);

                command.Parameters.AddWithValue("@rateId", id);

                int affected = await command.ExecuteNonQueryAsync();

                return affected > 0;
            }
        }

        public async static Task<bool> DeleteService(int id) {
            using (var databaseConnection = new SqliteConnection(connectionString)) {
                await databaseConnection.OpenAsync();

                var command = new SqliteCommand($@"
                    delete from {ORDERS_TABLE_NAME} where ServiceId = @id;
                    delete from {SERVICES_TABLE_NAME} where Id = @id;
                ", databaseConnection);

                command.Parameters.AddWithValue("@id", id);

                int affected = await command.ExecuteNonQueryAsync();

                return affected > 0;
            }
        }

        public async static Task<int> RegisterOrder(int clientId, int serviceId) {
            using (var connection = new SqliteConnection(connectionString)) {
                await connection.OpenAsync();

                var command = new SqliteCommand($@"
                    insert into {ORDERS_TABLE_NAME} (
	                    ServiceId, ClientId, OrderDate,
                    ) select * from (
                      select @serviceId AS serviceId, @clientId as clientId, @orderDate
                    ) as localOrder 
                    where not exists (
                        select ServiceId, ClientId, IsDone FROM {ORDERS_TABLE_NAME} 
  	                    where ServiceId = localOrder.serviceId AND ClientId = localOrder.clientId AND IsDone = false
                    ) limit 1
                    select last_instert_rowid() where changes() > 0;
                ");

                command.Parameters.AddWithValue("@serviceId", serviceId);
                command.Parameters.AddWithValue("@clientId", clientId);
                command.Parameters.AddWithValue("@orderDate", DateTime.Now);

                object data = await command.ExecuteScalarAsync();
                if (data == null) throw new DatabaseOperationException("This order already exists");

                return Convert.ToInt32(data);
            }
        }

        public async static Task<bool> ReleaseOrder(int orderId) {
            using (var connection = new SqliteConnection(connectionString)) {
                await connection.OpenAsync();

                var command = new SqliteCommand($@"
                    update {ORDERS_TABLE_NAME}
                    set IsDone = true,
	                    CompletionDate = Date()
                    where IsDone = false and id = @orderId;
                ", connection);

                command.Parameters.AddWithValue("@orderId", orderId);

                int affected = await command.ExecuteNonQueryAsync();

                return affected > 0;
            }
        }

        public async static Task<ClientModel> GetClient(int id) {
            using (var databaseConnection = new SqliteConnection(connectionString)) {
                await databaseConnection.OpenAsync();

                var command = new SqliteCommand($"SELECT * FROM {CLIENTS_TABLE_NAME} WHERE id = @id LIMIT 1", databaseConnection);
                command.Parameters.AddWithValue("@id", id);

                var reader = await command.ExecuteReaderAsync();
                var mayContinue = await reader.ReadAsync();

                if (!mayContinue) return null;

                var client = new ClientModel();
                return client.Create(reader);
            }
        }

        public async static Task<ClientModel> GetClientOfUser (int userId) {
            
        }

        public async static Task<List<OrderModel>> FindOrderOfClient(int clientId, int serviceId) {
            var list = new List<OrderModel>();
            using (var connection = new SqliteConnection(connectionString)) {
                try {
                    var command = new SqliteCommand($@"SELECT * FROM {ORDERS_TABLE_NAME} WHERE ServiceId = @serviceId AND ClientId = @clientId AND IsDone = false");

                    command.Parameters.AddWithValue("@serviceId", serviceId);
                    command.Parameters.AddWithValue("@clientId", clientId);

                    var reader = await command.ExecuteReaderAsync();

                    while (await reader.ReadAsync()) {
                        var order = new OrderModel();
                        list.Add(order);
                    }

                } catch (Exception exception) {
                    Console.WriteLine(exception);
                }
            }
            return list;
        }

        public async static Task<UserModel> FindUser(int id) {
            using (var databaseConnection = new SqliteConnection(connectionString)) {
                await databaseConnection.OpenAsync();

                var command = new SqliteCommand($@"SELECT * FROM {USERS_TABLE_NAME} WHERE id = @id", databaseConnection);
                command.Parameters.AddWithValue("@id", id);

                var reader = await command.ExecuteReaderAsync();
                bool mayContinue = await reader.ReadAsync();
                if (!mayContinue) return null;

                var user = new UserModel();
                return user.Create(reader);
            }
        }

        public static Task<List<BlogModel>> FetchBlogs() {
            var model = new BlogModel();
            return FetchTable(model, BLOGS_TABLE_NAME);
        }

        public static Task<List<OrderModel>> FetchOrders() {
            var model = new OrderModel();
            return FetchTable(model, ORDERS_TABLE_NAME);
        }

        public static Task<List<RateModel>> FetchRates() {
            var model = new RateModel();
            return FetchTable(model, RATES_TABLE_NAME);
        }

        public static Task<List<ServiceModel>> FetchServices() {
            var model = new ServiceModel();
            return FetchTable(model, SERVICES_TABLE_NAME);
        }

        private async static Task<List<T>> FetchTable<T>(DatabaseFactory<T> model, string tableName) {
            using (var databaseConnection = new SqliteConnection(connectionString)) {
                await databaseConnection.OpenAsync();

                var commandText = $@"SELECT * FROM {tableName}";
                var command = new SqliteCommand(commandText, databaseConnection);
                var reader = await command.ExecuteReaderAsync();

                return GetModelList(model, reader);
            }
        }

        private static List<T> GetModelList<T>(DatabaseFactory<T> factory, SqliteDataReader reader) {
            var list = new List<T>();

            while (reader.Read()) {
                var model = factory.Create(reader);
                list.Add(model);
            }

            return list;
        }
    }
}
