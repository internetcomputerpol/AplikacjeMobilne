using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

public class ContactsViewModel : INotifyPropertyChanged
{
    private readonly FirebaseService _firebaseService;
    public ObservableCollection<Contact> Contacts { get; set; }
    public ICommand AddContactCommand { get; set; }
    public ICommand DeleteContactCommand { get; set; }

    private string _lastName;
    public string LastName
    {
        get => _lastName;
        set
        {
            _lastName = value;
            OnPropertyChanged(nameof(LastName));
        }
    }

    private string _phoneNumber;
    public string PhoneNumber
    {
        get => _phoneNumber;
        set
        {
            _phoneNumber = value;
            OnPropertyChanged(nameof(PhoneNumber));
        }
    }

    private string _email;
    public string Email
    {
        get => _email;
        set
        {
            _email = value;
            OnPropertyChanged(nameof(Email));
        }
    }

    public ContactsViewModel()
    {
        _firebaseService = new FirebaseService();
        Contacts = new ObservableCollection<Contact>();
        AddContactCommand = new Command(async () => await AddContact());
        DeleteContactCommand = new Command<string>(async (id) => await DeleteContact(id));
        LoadContacts();
    }

    private async void LoadContacts()
    {
        try
        {
            var contacts = await _firebaseService.GetContacts();
            Contacts.Clear();
            foreach (var contact in contacts)
            {
                Contacts.Add(contact);
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Błąd",
                $"Nie udało się załadować kontaktów: {ex.Message}",
                "OK");
        }
    }

    private async Task AddContact()
    {
        if (string.IsNullOrWhiteSpace(LastName) ||
            string.IsNullOrWhiteSpace(PhoneNumber) ||
            string.IsNullOrWhiteSpace(Email))
        {
            await Application.Current.MainPage.DisplayAlert(
                "Błąd",
                "Wypełnij wszystkie pola",
                "OK");
            return;
        }

        try
        {
            var contact = new Contact
            {
                LastName = LastName,
                PhoneNumber = PhoneNumber,
                Email = Email
            };

            
            var id = await _firebaseService.AddContact(contact);
            contact.Id = id; 

            Contacts.Add(contact);

            LastName = string.Empty;
            PhoneNumber = string.Empty;
            Email = string.Empty;
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Błąd",
                $"Nie udało się dodać kontaktu: {ex.Message}",
                "OK");
        }
    }

    private async Task DeleteContact(string id)
    {
        var contact = Contacts.FirstOrDefault(c => c.Id == id);
        if (contact != null)
        {
            try
            {
                // Najpierw usuwamy z Firebase
                await _firebaseService.DeleteContact(id);
                // Jeśli usunięcie z Firebase się powiodło, usuwamy z lokalnej kolekcji
                Contacts.Remove(contact);
            }
            catch (Exception ex)
            {
                // Jeśli wystąpił błąd przy usuwaniu z Firebase, wyświetlamy komunikat
                await Application.Current.MainPage.DisplayAlert(
                    "Błąd",
                    $"Nie udało się usunąć kontaktu: {ex.Message}",
                    "OK");
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}