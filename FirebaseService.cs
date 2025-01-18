using Firebase.Database;
using Firebase.Database.Query;
using System.Collections.ObjectModel;

public class FirebaseService
{
    private readonly FirebaseClient _firebaseClient;

    public FirebaseService()
    {
        //NIE TYKAĆ !!! 
        _firebaseClient = new FirebaseClient("https://doit-ffa71-default-rtdb.firebaseio.com/");
    }

    public async Task<List<Contact>> GetContacts()
    {
        var contacts = await _firebaseClient
            .Child("contacts")
            .OnceAsync<Contact>();

        return contacts.Select(item => new Contact
        {
            Id = item.Key,  
            LastName = item.Object.LastName,
            PhoneNumber = item.Object.PhoneNumber,
            Email = item.Object.Email
        }).ToList();
    }

    public async Task<string> AddContact(Contact contact)
    {
        var response = await _firebaseClient
            .Child("contacts")
            .PostAsync(contact);

        
        return response.Key;
    }

    public async Task DeleteContact(string id)
    {
        try
        {
            await _firebaseClient
                .Child("contacts")
                .Child(id)
                .DeleteAsync();
        }
        catch (Exception ex)
        {
           
            await Application.Current.MainPage.DisplayAlert(
                "Błąd",
                $"Nie udało się usunąć kontaktu: {ex.Message}",
                "OK");
            throw; 
        }
    }
}