
using Microsoft.JSInterop;
using webRestaurant.Model;

namespace webRestaurant.Pages
{
    public partial class Login
    {
        private LoginModel loginModel = new LoginModel();
        private string userRulesDisplay = "";

        private async Task HandleLogin()
        {
            // var responseModel = await EmployeeService.LoginAsync(loginModel);
            if (loginModel.Password == "111")
            {
                // Set shared employee data
                // SharedEmployeeService.EmployeeName = responseModel.employeeName;
                // SharedEmployeeService.EmployeeId = responseModel.employeeId;
                // SharedEmployeeService.EmployeeLevel = responseModel.level;
                // await SharedEmployeeService.SaveDataAsync();

                // Set shared user rules data
                // SharedUserule.UserRuleNames = (await UserRuleService.GetRuleNamesByEmployeeIdAsync(responseModel.employeeId)).ToArray();
                // await SharedUserule.SaveDataAsync();

                // Show success message and navigate to home
                await localStorage.SetItemAsync("UserName", loginModel.Username); await JS.InvokeVoidAsync("showAlert.successAlert", "Inform", "Login successful.");
                StateHasChanged();
                Navigation.NavigateTo("/home", true);
            }
            else
            {
                // Show error message if login fails
                await JS.InvokeVoidAsync("customAlert.showAlert", "Warning", "Login failed.");
            }
        }
    }
}
