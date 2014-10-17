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

        public LoginCommand LoginCmd { get; set; }

        public Windows.UI.Xaml.Controls.Page Page { get; set; }

        public AuthenticationController()
        {
            this.LoginModel = new DataModel.LoginModel();
            this.LoginCmd = new LoginCommand(this);
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
