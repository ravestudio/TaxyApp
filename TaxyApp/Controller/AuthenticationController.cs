using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TaxyApp.Controller
{
    public class AuthenticationController
    {
        public DataModel.LoginModel LoginModel { get; set; }
        public DataModel.RegistrationModel RegistrationModel { get; set; }

        public LoginCommand LoginCmd { get; set; }
        public RegisterCommand RegisterCmd { get; set; }
        public GotoLoginCommand GotoLoginCmd { get; set; }

        public Windows.UI.Xaml.Controls.Page Page { get; set; }

        public AuthenticationController()
        {
            this.LoginModel = new DataModel.LoginModel();
            this.RegistrationModel = new DataModel.RegistrationModel();
            this.LoginCmd = new LoginCommand(this);
            this.RegisterCmd = new RegisterCommand(this);
            this.GotoLoginCmd = new GotoLoginCommand(this);
        }

    }

    public class GotoLoginCommand : System.Windows.Input.ICommand
    {
        private AuthenticationController _controller = null;

        public GotoLoginCommand(AuthenticationController controller)
        {
            this._controller = controller;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            Windows.Foundation.IAsyncAction action =
                this._controller.Page.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    int thread = Environment.CurrentManagedThreadId;

                    Frame frame = _controller.Page.Frame;

                    frame.Navigate(typeof(AuthenticationPage));
                });
        }
    }

    public class RegisterCommand : System.Windows.Input.ICommand
    {
        private AuthenticationController _controller = null;

        public RegisterCommand(AuthenticationController controller)
        {
            this._controller = controller;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            TaxyApp.Core.WebApiClient client = new TaxyApp.Core.WebApiClient();

            TaxyApp.Core.Repository.UserRepository userRepository = new Core.Repository.UserRepository(client);

            DataModel.RegistrationModel regModel = _controller.RegistrationModel;

            userRepository.RegisterUser(regModel.PhoneNumber).ContinueWith(t =>
                {
                    string res = t.Result;
                });
        }
    }

    public class LoginCommand : System.Windows.Input.ICommand
    {
        private AuthenticationController _controller = null;

        public LoginCommand(AuthenticationController controller)
        {
            this._controller = controller;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            TaxyApp.Core.WebApiClient client = new TaxyApp.Core.WebApiClient();

            TaxyApp.Core.Repository.UserRepository userRepository = new Core.Repository.UserRepository(client);

            DataModel.LoginModel model = _controller.LoginModel;

            userRepository.GetUser(model.PhoneNumber, model.PIN).ContinueWith(t =>
                {
                    TaxyApp.Core.Session.Instance.SetUSer(t.Result);

                    Windows.Foundation.IAsyncAction action =
                    this._controller.Page.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        int thread = Environment.CurrentManagedThreadId;

                        Frame frame = _controller.Page.Frame;

                        frame.Navigate(typeof(PivotPage));
                    });
                });
            
        }
    }
}
