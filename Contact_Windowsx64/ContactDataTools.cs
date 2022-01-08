using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.Data;
using Microsoft.Data.Sqlite;
using System.IO;

namespace Contact_Windowsx64
{
    public struct TelInfo
    {
        public string TelNumber { get; set; }
        public bool IsFavourate { get; set; }

        public TelInfo(string telNumber)
        {
            TelNumber = telNumber;
            IsFavourate = false;
        }

        public TelInfo(string telNumber, bool isFavourate)
        {
            TelNumber = telNumber;
            IsFavourate = isFavourate;
        }
    }

    public struct EmailInfo
    {
        public string EmailAddress { get; set; }
        public bool IsFavourate { get; set; }

        public EmailInfo(string emailAddress)
        {
            EmailAddress = emailAddress;
            IsFavourate = false;
        }

        public EmailInfo(string emailAddress, bool isFavourate)
        {
            EmailAddress = emailAddress;
            IsFavourate = isFavourate;
        }
    }

    public struct ContactEditor // ID字段必须在初始化时完成指定，insert类不知道id的用-1表示，知道id的则填写
    {
        private Int32 id;
        private DateTime? birthday;
        private bool profileImage;

        public Int32 ID { get { return id; } }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get { return string.Join(" ", new string[] { LastName, FirstName }); } }
        public List<EmailInfo> EmailInfos { get; set; }
        public List<TelInfo> TelInfos { get; set; }
        public string Addr { get; set; }
        public string Company { get; set; }
        public string Job { get; set; }
        public string Birthday { get { return birthday is null ? "" : ((DateTime)birthday).ToString("yyyy-MM-dd"); } set { birthday = value == "" ? (DateTime?)null : DateTime.Parse(value); } }
        public string ProfileImage { get { return profileImage ? ApplicationData.Current.LocalFolder.Path + $@"\{id}.png" : @".\Assets\GrayBackground.png"; } set { profileImage = value.Length != 0; } }
        public bool IsFavourate { get; set; }

        public ContactEditor(Int32 id_init)
        {
            id = id_init;
            FirstName = string.Empty;
            LastName = string.Empty;
            EmailInfos = new List<EmailInfo>();
            TelInfos = new List<TelInfo>();
            Addr = string.Empty;
            Company = string.Empty;
            Job = string.Empty;
            birthday = null;
            profileImage = false;
            IsFavourate = false;
        }

        public void UpdateID(Int32 id_new)
        {
            id = id_new;
        }

        public void UpdateProfileImage(bool prim)
        {
            profileImage = prim;
        }
    }

    public struct ContactFavourate // ID字段必须在初始化时完成指定，insert类不知道id的用-1表示，知道id的则填写
    {
        private Int32 id;
        private DateTime? birthday;
        private bool profileImage;

        public Int32 ID { get { return id; } }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get { return string.Join(" ", new string[] { LastName, FirstName }); } }
        public string EmailInfos { get; set; }
        public string TelInfos { get; set; }
        public string Addr { get; set; }
        public string Company { get; set; }
        public string Job { get; set; }
        public string Birthday { get { return birthday == null ? "" : ((DateTime)birthday).ToString("yyyy-MM-dd"); } set { birthday = value == "" ? (DateTime?)null : DateTime.Parse(value); } }
        public string ProfileImage { get { return profileImage ? ApplicationData.Current.LocalFolder.Path + $@"\{id}.png" : @".\Assets\GrayBackground.png"; } set { profileImage = value.Length != 0; } }
        public bool IsFavourate { get; set; }

        public ContactFavourate(Int32 id_init)
        {
            id = id_init;
            FirstName = string.Empty;
            LastName = string.Empty;
            EmailInfos = string.Empty;
            TelInfos = string.Empty;
            Addr = string.Empty;
            Company = string.Empty;
            Job = string.Empty;
            birthday = null;
            profileImage = false;
            IsFavourate = true;
        }

        public override string ToString()
        {
            return LastName + FirstName;
        }

        public void UpdateID(Int32 id_new)
        {
            id = id_new;
        }
        
        public void UpdateProfileImage(bool prim)
        {
            profileImage = prim;
        }
    }

    public struct ContactDatas
    {
        private UInt32 counter;
        private List<ContactEditor> editors;

        public UInt32 Counter { get { return counter; } }
        public List<ContactEditor> Editors { get { return editors ?? new List<ContactEditor>(); } }

        public ContactDatas(List<ContactEditor> editorsInput)
        {
            editors = editorsInput;
            counter = (UInt32)editors.Count;
        }

        public ContactDatas(ContactEditor[] editorsInput)
        {
            editors = new List<ContactEditor>(editorsInput);
            counter = (UInt32)editors.Count;
        }

        public bool Add(ref ContactEditor newOne)
        {
            if (editors == null)
            {
                editors=new List<ContactEditor>();
            }
            if (newOne.ID != -1)
            {
                foreach (var e in editors)
                {
                    if (e.ID == newOne.ID)
                    {
                        return false;
                    }
                }
            }
            editors.Add(newOne);
            counter++;
            return true;
        }
    }

    public struct ContactFavourateDatas
    {
        private UInt32 counter;
        private List<ContactFavourate> favourateEditors;

        public UInt32 Counter { get { return counter; } }
        public List<ContactFavourate> FavourateEditors { get { return favourateEditors ?? new List<ContactFavourate>(); } }

        public ContactFavourateDatas(ContactFavourate[] editorsInput)
        {
            favourateEditors = new List<ContactFavourate>(editorsInput);
            counter = (UInt32)favourateEditors.Count;
        }

        public ContactFavourateDatas(List<ContactFavourate> editorsInput)
        {
            favourateEditors = editorsInput;
            counter = (UInt32)favourateEditors.Count;
        }

        public bool Add(ref ContactFavourate newOne)
        {
            if(favourateEditors == null)
            {
                favourateEditors = new List<ContactFavourate>();
            }
            if (newOne.ID != -1)
            {
                foreach (var e in favourateEditors)
                {
                    if (e.ID == newOne.ID)
                    {
                        return false;
                    }
                }
            }
            favourateEditors.Add(newOne);
            counter++;
            return true;
        }
    }

    public class ContactDatabase
    {
        //StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
        //StorageFile contaceDatabase = await storageFolder.CreateFileAsync("contact.sqlite", CreationCollisionOption.ReplaceExisting);
        //StorageFile contactDatabase = await StorageFolder.GetFileAsync("contact.sqlite");
        //await FileIO.WriteTextAsync(ContactDatabase,"Some text.");

        private static ContactDatabase _contactDatabase = null;
        private static SqliteConnection _databaseConnection = null;
        private static bool error = false;
        private static int counter = 0;

        public static ContactDatabase CurrentInstance
        {
            get { return _contactDatabase; }
        }

        static ContactDatabase()
        {
            _contactDatabase = new ContactDatabase();


            // start a SQLite connection with Data Source "contactDatabase.db"
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            string dataSource = "Data Source=" + storageFolder.Path + @"\contactDatabase.db";
            _databaseConnection = new SqliteConnection(dataSource);
            _databaseConnection.Open();
            error = _databaseConnection.State == ConnectionState.Broken;
            if (error)
            {
                _databaseConnection.Close();
                return;
            }

            // check whether tables are existing. create table if they are not exist
            var command = _databaseConnection.CreateCommand();
            command.CommandText =
            @"
                CREATE TABLE IF NOT EXISTS ContactInfo
                (
                    id INTEGER PRIMARY KEY ASC AUTOINCREMENT,
                    firstName TEXT,
                    lastName TEXT,
                    address TEXT,
                    company TEXT,
                    job TEXT,
                    birthday TEXT,
                    profileImage BOOLEAN NOT NULL DEFAULT FALSE,
                    isFavourate BOOLEAN NOT NULL DEFAULT FALSE
                );

                CREATE TABLE IF NOT EXISTS TelInfo
                (
                    id_intel INTEGER PRIMARY KEY ASC AUTOINCREMENT,
                    id INTEGER NOT NULL REFERENCES ContactInfo (id) ON DELETE CASCADE ON UPDATE CASCADE,
                    telNumber TEXT NOT NULL,
                    isFavourate BOOLEAN NOT NULL DEFAULT FALSE
                );

                CREATE TABLE IF NOT EXISTS EmailInfo
                (
                    id_intel INTEGER PRIMARY KEY ASC AUTOINCREMENT,
                    id INTEGER NOT NULL REFERENCES ContactInfo (id) ON DELETE CASCADE ON UPDATE CASCADE,
                    emailAddr TEXT NOT NULL,
                    isFavourate BOOLEAN NOT NULL DEFAULT FALSE
                );
            ";
            command.ExecuteNonQuery();
            error = _databaseConnection.State == ConnectionState.Broken;
            if (error)
            {
                _databaseConnection.Close();
                return;
            }
        }

        ~ContactDatabase()
        {
            if (_contactDatabase == null)
            {
                counter--;
                return;
            }
            else if (counter == 1)
            {
                counter--;
                while (_databaseConnection.State == ConnectionState.Fetching || _databaseConnection.State == ConnectionState.Executing) ;
                _databaseConnection.Close();
            }
        }

        public bool InsertOrUpdate(ref ContactEditor newContact)
        {
            // if newContact.id == -1, insert and update id
            if(newContact.ID == -1)
            {
                var command = _databaseConnection.CreateCommand();
                command.CommandText =
                @"
                    INSERT INTO ContactInfo (firstName,lastName,address,company,job,birthday,profileImage,isFavourate)
                    VALUES(@firstName,@lastName,@address,@company,@job,@birthday,@profileImage,@isFavourate);
                    SELECT last_insert_rowid();
                ";
                command.Parameters.AddWithValue("firstName", newContact.FirstName);
                command.Parameters.AddWithValue("lastName", newContact.LastName);
                command.Parameters.AddWithValue("address", newContact.Addr);
                command.Parameters.AddWithValue("company", newContact.Company);
                command.Parameters.AddWithValue("job", newContact.Job);
                command.Parameters.AddWithValue("birthday", newContact.Birthday);
                command.Parameters.AddWithValue("profileImage", newContact.ProfileImage.EndsWith("GrayBackground.png") ? false : true).SqliteType = SqliteType.Integer;
                command.Parameters.AddWithValue("isFavourate", newContact.IsFavourate).SqliteType = SqliteType.Integer;
                var reader = command.ExecuteReader();
                if(reader.Read())
                {
                    newContact.UpdateID(reader.GetInt32(0));
                }
                reader.Close();
                command.Parameters.Clear();
            }
            // if newContact.id != -1, update info
            else
            {
                var command = _databaseConnection.CreateCommand();
                command.CommandText =
                @"
                    UPDATE ContactInfo
                    SET firstName=@firstName, lastName=@lastName, address=@address,
                        company=@company, job=@job, birthday=@birthday,
                        profileImage=@profileImage, isFavourate=@isFavourate
                    WHERE id=@id;
                ";
                command.Parameters.AddWithValue("id", newContact.ID).SqliteType = SqliteType.Integer;
                command.Parameters.AddWithValue("firstName", newContact.FirstName);
                command.Parameters.AddWithValue("lastName", newContact.LastName);
                command.Parameters.AddWithValue("address", newContact.Addr);
                command.Parameters.AddWithValue("company", newContact.Company);
                command.Parameters.AddWithValue("job", newContact.Job);
                command.Parameters.AddWithValue("birthday", newContact.Birthday);
                command.Parameters.AddWithValue("profileImage", newContact.ProfileImage.EndsWith("GrayBackground.png") ? false : true).SqliteType = SqliteType.Integer;
                command.Parameters.AddWithValue("isFavourate", newContact.IsFavourate).SqliteType = SqliteType.Integer;
                command.ExecuteNonQuery();
                command.Parameters.Clear();
            }
            // update telInfo and emailInfo
            {
                var command = _databaseConnection.CreateCommand();
                command.CommandText =
                @"
                    DELETE FROM EmailInfo WHERE id=@id1;
                    DELETE FROM TelInfo WHERE id=@id2;
                ";
                command.Parameters.AddWithValue("id1", newContact.ID).SqliteType = SqliteType.Integer;
                command.Parameters.AddWithValue("id2", newContact.ID).SqliteType = SqliteType.Integer;
                command.ExecuteNonQuery();
                command.Parameters.Clear();

                command.CommandText =
                @"
                    INSERT INTO TelInfo (id,telNumber,isFavourate)
                    VALUES(@id,@telNumber,@isFavourate);
                ";
                foreach(var telInfo in newContact.TelInfos)
                {
                    command.Parameters.AddWithValue("id", newContact.ID).SqliteType = SqliteType.Integer;
                    command.Parameters.AddWithValue("telNumber", telInfo.TelNumber);
                    command.Parameters.AddWithValue("isFavourate", telInfo.IsFavourate).SqliteType = SqliteType.Integer;
                    command.ExecuteNonQuery();
                    command.Parameters.Clear();
                }

                command.CommandText =
                @"
                    INSERT INTO EmailInfo (id,emailAddr,isFavourate)
                    VALUES(@id,@emailAddr,@isFavourate);
                ";
                foreach (var emailInfo in newContact.EmailInfos)
                {
                    command.Parameters.AddWithValue("id", newContact.ID).SqliteType = SqliteType.Integer;
                    command.Parameters.AddWithValue("emailAddr", emailInfo.EmailAddress);
                    command.Parameters.AddWithValue("isFavourate", emailInfo.IsFavourate).SqliteType = SqliteType.Integer;
                    command.ExecuteNonQuery();
                    command.Parameters.Clear();
                }
            }
            return true;
        }

        public bool DeleteContact(Int32 id)
        {
            if (id != -1)
            {
                var command = _databaseConnection.CreateCommand();
                command.CommandText =
                @"
                    DELETE FROM ContactInfo
                    WHERE id=@id;
                ";
                command.Parameters.AddWithValue("id", id).SqliteType = SqliteType.Integer;
                command.ExecuteNonQuery();
                command.Parameters.Clear();
            }
            return true;
        }

        public bool InsertOrUpdata(ref ContactDatas newContacts)
        {
            foreach (var editor in newContacts.Editors)
            {
                ContactEditor _editor = editor;
                InsertOrUpdate(ref _editor);
                editor.UpdateID(_editor.ID);
            }

            return true;
        }

        public ContactFavourateDatas GetFavourateContacts()
        {
            ContactFavourateDatas tmp = new ContactFavourateDatas();
            if(error)
            {
                return tmp;
            }

            var command = _databaseConnection.CreateCommand();
            command.CommandText =
            @"
                SELECT * FROM ContactInfo WHERE ContactInfo.isFavourate = true; 
            ";
            SqliteDataReader reader = command.ExecuteReader();
            while(reader.Read())
            {
                int id = reader.GetInt32(0);
                ContactFavourate editor = new ContactFavourate(id);
                editor.FirstName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                editor.LastName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
                editor.Addr = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                editor.Company = reader.IsDBNull(4) ? string.Empty : reader.GetString(4);
                editor.Job = reader.IsDBNull(5) ? string.Empty : reader.GetString(5);
                editor.Birthday = reader.IsDBNull(6) ? "" : reader.GetString(6);
                editor.UpdateProfileImage(reader.GetBoolean(7));
                editor.IsFavourate = reader.GetBoolean(8);
                
                var command_sub = _databaseConnection.CreateCommand();
                command_sub.CommandText =
                @"
                    SELECT telNumber,isFavourate FROM TelInfo WHERE id=@id and isFavourate = true;
                ";
                command_sub.Parameters.AddWithValue("id", editor.ID).SqliteType = SqliteType.Integer;
                SqliteDataReader reader_sub = command_sub.ExecuteReader();
                if(reader_sub.Read())
                    editor.TelInfos = reader_sub.GetString(0);
                while (reader_sub.Read()) ;
                reader_sub.Close();
                command_sub.Parameters.Clear();

                command_sub.CommandText =
                @"
                    SELECT emailAddr,isFavourate FROM EmailInfo WHERE id=@id and isFavourate = true;
                ";
                command_sub.Parameters.AddWithValue("id",editor.ID).SqliteType = SqliteType.Integer;
                reader_sub = command_sub.ExecuteReader();
                if (reader_sub.Read())
                    editor.EmailInfos = reader_sub.GetString(0);
                while (reader_sub.Read()) ;
                reader_sub.Close();

                tmp.Add(ref editor);
            }
            reader.Close();

            return tmp;
        }

        public ContactFavourateDatas GetContactsList()
        {
            ContactFavourateDatas tmp = new ContactFavourateDatas();
            if (error)
            {
                return tmp;
            }

            var command = _databaseConnection.CreateCommand();
            command.CommandText =
            @"
                SELECT id,firstName,lastName,company,profileImage FROM ContactInfo; 
            ";
            SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Int32 id = reader.GetInt32(0);
                ContactFavourate editor = new ContactFavourate(id);
                editor.FirstName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                editor.LastName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
                editor.Company = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                editor.UpdateProfileImage(reader.GetBoolean(4));

                tmp.Add(ref editor);
            }
            reader.Close();

            return tmp;
        }

        public ContactFavourateDatas GetAutoSuggestItems(string searchkey)
        {
            ContactFavourateDatas tmp = new ContactFavourateDatas();
            if (error)
            {
                return tmp;
            }

            var command = _databaseConnection.CreateCommand();
            command.CommandText =
            @"
                SELECT id,firstName,lastName,company,profileImage FROM ContactInfo WHERE lastName||firstName like @searchkey; 
            ";
            command.Parameters.AddWithValue("searchkey", "%"+searchkey+"%");
            SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Int32 id = reader.GetInt32(0);
                ContactFavourate editor = new ContactFavourate(id);
                editor.FirstName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                editor.LastName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
                editor.Company = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                editor.UpdateProfileImage(reader.GetBoolean(4));

                tmp.Add(ref editor);
            }
            reader.Close();

            return tmp;
        }

        public ContactEditor GetContactEditor(Int32 id2search)
        {
            ContactEditor editor = new ContactEditor();
            if (error)
            {
                return editor;
            }

            var command = _databaseConnection.CreateCommand();
            command.CommandText =
            @"
                SELECT * FROM ContactInfo WHERE id=@id; 
            ";
            command.Parameters.AddWithValue("id", id2search);
            SqliteDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                int id = reader.GetInt32(0);
                editor = new ContactEditor(id);
                editor.FirstName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                editor.LastName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
                editor.Addr = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                editor.Company = reader.IsDBNull(4) ? string.Empty : reader.GetString(4);
                editor.Job = reader.IsDBNull(5) ? string.Empty : reader.GetString(5);
                editor.Birthday = reader.IsDBNull(6) ? "" : reader.GetString(6);
                editor.UpdateProfileImage(reader.GetBoolean(7));
                editor.IsFavourate = reader.GetBoolean(8);

                var command_sub = _databaseConnection.CreateCommand();
                command_sub.CommandText =
                @"
                    SELECT telNumber,isFavourate FROM TelInfo WHERE id=@id;
                ";
                command_sub.Parameters.AddWithValue("id", editor.ID).SqliteType = SqliteType.Integer;
                SqliteDataReader reader_sub = command_sub.ExecuteReader();
                while (reader_sub.Read())
                {
                    TelInfo telInfo = new TelInfo(reader_sub.GetString(0), reader_sub.GetBoolean(1));
                    editor.TelInfos.Add(telInfo);
                }
                reader_sub.Close();
                command_sub.Parameters.Clear();

                command_sub.CommandText =
                @"
                    SELECT emailAddr,isFavourate FROM EmailInfo WHERE id=@id;
                ";
                command_sub.Parameters.AddWithValue("id", editor.ID).SqliteType = SqliteType.Integer;
                reader_sub = command_sub.ExecuteReader();
                while (reader_sub.Read())
                {
                    EmailInfo emailInfo = new EmailInfo(reader_sub.GetString(0), reader_sub.GetBoolean(1));
                    editor.EmailInfos.Add(emailInfo);
                }
                while (reader_sub.Read()) ;
                reader_sub.Close();
            }
            while (reader.Read()) ;
            reader.Close();

            return editor;
        }
    }

    public class ImageHandler
    {
        public static byte[] ImageToByte(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            Byte[] pic_byte = new Byte[fs.Length];
            fs.Read(pic_byte, 0, pic_byte.Length);
            fs.Close();
            return pic_byte;
        }

        public static string SQLByteToFile(ref SqliteDataReader reader, int index, string name)
        {
            byte[] img = reader[index] as byte[];
            StorageFolder storagefolder = ApplicationData.Current.LocalFolder;
            FileStream ms = new FileStream(storagefolder.Path + @"\" + name, FileMode.OpenOrCreate);

            if (img != null)
            {
                ms.Write(img, 0, img.Length);
                ms.Flush();
            }
            ms.Close();
            return storagefolder.Path + @"\" + name;
        }

        public static MemoryStream SaveImageToAPP(Stream file)
        {
            MemoryStream des = new MemoryStream();
            file.CopyTo(des);
            des.Flush();
            file.Close();
            return des;
        }
    }
}
