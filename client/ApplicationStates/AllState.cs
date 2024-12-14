namespace client.ApplicationStates
{
    public class AllState
    {
        //scope action
        public Action? Action { get; set; }

        // General Department
        public bool ShowGeneralDepartment { get; set; } = false;
        public void GeneralDepartmentClicked()
        {
            ResetAllDepartment();
            ShowGeneralDepartment = true;
            Action?.Invoke();
        }

        public bool ShowDepartment { get; set; } = false;
        public void DepartmentClicked()
        {
            ResetAllDepartment();
            ShowDepartment = true;
            Action?.Invoke();
        }
        public bool ShowBranch { get; set; } = false;
        public void BranchClicked()
        {
            ResetAllDepartment();
            ShowBranch = true;
            Action?.Invoke();
        }
        public bool ShowCountry { get; set; } = false;
        public void CountryClicked()
        {
            ResetAllDepartment();
            ShowCountry = true;
            Action?.Invoke();
        }
        public bool ShowCity { get; set; } = false;
        public void CityClicked()
        {
            ResetAllDepartment();
            ShowCity = true;
            Action?.Invoke();
        }
        public bool ShowTown { get; set; } = false;
        public void TownClicked()
        {
            ResetAllDepartment();
            ShowTown = true;
            Action?.Invoke();
        }
        public bool ShowUser { get; set; } = false;
        public void UserClicked()
        {
            ResetAllDepartment();
            ShowUser = true;
            Action?.Invoke();
        }
        public bool ShowEmployee { get; set; } = true;
        public void EmployeeClicked()
        {
            ResetAllDepartment();
            ShowEmployee = true;
            Action?.Invoke();
        }


        private void ResetAllDepartment()
        {
            ShowGeneralDepartment = false;
            ShowBranch = false;
            ShowCity = false;
            ShowCountry = false;
            ShowDepartment = false;
            ShowEmployee = false;
            ShowTown = false;
            ShowUser = false;
        }
    }
}
