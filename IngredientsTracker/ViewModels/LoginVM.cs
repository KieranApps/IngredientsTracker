using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IngredientsTracker.ViewModels
{
    public partial class LoginVM : BindableObject
    {

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        public LoginVM() { }

        public ICommand LoginCommand => new Command(async () => await Login());


        private async Task Login()
        {
            Debug.WriteLine("TESTING");
            if (_email == null) { }
            if (_password == null) { }
        }
    }
}
