using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

public class AddressBook
{
    private readonly string _connectionString;

    public AddressBook(string dbPath)
    {
        _connectionString = $"Data Source={dbPath}";
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        string sql = @"CREATE TABLE IF NOT EXISTS Contacts (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Name TEXT NOT NULL,
            Email TEXT,
            Phone TEXT
        );";
        using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        cmd.ExecuteNonQuery();
    }

    public void AddContact(Contact contact)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT INTO Contacts (Name, Email, Phone) VALUES (@Name, @Email, @Phone)";
        cmd.Parameters.AddWithValue("@Name", contact.Name);
        cmd.Parameters.AddWithValue("@Email", contact.Email);
        cmd.Parameters.AddWithValue("@Phone", contact.Phone);
        cmd.ExecuteNonQuery();
    }

    public List<Contact> GetAllContacts()
    {
        var contacts = new List<Contact>();
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Id, Name, Email, Phone FROM Contacts";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            contacts.Add(new Contact
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Email = reader.GetString(2),
                Phone = reader.GetString(3)
            });
        }
        return contacts;
    }

    public List<Contact> SearchContacts(string name)
    {
        var contacts = new List<Contact>();
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Id, Name, Email, Phone FROM Contacts WHERE Name LIKE @Name";
        cmd.Parameters.AddWithValue("@Name", "%" + name + "%");
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            contacts.Add(new Contact
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Email = reader.GetString(2),
                Phone = reader.GetString(3)
            });
        }
        return contacts;
    }
}
